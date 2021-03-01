using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;
using DED.Director;
using log4net;
using log4net.Config;

namespace DED.Decision
{
    class ActionFilter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ActionFilter));

        public ActionFilter() { XmlConfigurator.Configure(); }

        public Dictionary<string, Relation> FilterActionsBySeeds(Dictionary<string, ContextSubnet> kb, Relation seed)
        {
            Dictionary<string, Relation> actions = new Dictionary<string,Relation>();

            Subnet subnet = kb[seed.submodel].Knowledges[seed.name];
            if (!actions.ContainsKey(seed.name))
                actions.Add(seed.name,seed);
            //add all parents and heirs
            foreach (Relation r in subnet.Heirs)
            {
                if (!actions.ContainsKey(r.name))
                    actions.Add( r.name, r );
            }
            foreach (Relation r in subnet.Parents)
            {
                if (!actions.ContainsKey(r.name))
                    actions.Add(r.name, r);
            }            

            return actions;
        }


        public List<Strategy> GreaterVariable(ContextSubnet contextSubnet, string variable, Goal goal)
        {
            if (!InContext(contextSubnet, variable, goal.Variable)) return new List<Strategy>();
            Bayes net = new Bayes();
            Belief belief = new Belief(variable, goal.Name);
            Subnet variableSubnet = contextSubnet.Knowledges[variable];
            variableSubnet.IsValid = false;
            net.Calculate(contextSubnet,belief);            
            
            //initiate the return value
            List<Strategy> strategies = new List<Strategy>();
            List<Relation> parents = variableSubnet.Parents;
            foreach (Relation parent in parents)
            {
                belief.Variable = parent.name;
                GetGreaterStrategies(strategies, belief, goal);
            }
            
            return strategies;
        }

        public List<Strategy> GreaterDirectSingleVariable(ContextSubnet contextSubnet, string variable, Goal goal)
        {
            if (!InContext(contextSubnet, variable, goal.Variable)) return new List<Strategy>();
            Bayes net = new Bayes();
            Belief belief = new Belief(variable,goal.Name);
            Subnet variableSubnet = contextSubnet.Knowledges[variable];
            variableSubnet.IsValid = false;
            net.Calculate(contextSubnet,belief);

            log.InfoFormat("contextSubnet.Name: '{0}', variable '{1}', goal.name '{2}', goal.state '{3}'"
                , contextSubnet.Name, variable, goal.Name, goal.State);

            //initiate the return value
            List<Strategy> strategies = new List<Strategy>();

            GetGreaterStrategies(strategies, belief, goal);

            //Constants constant = new Constants();
            //if (belief.Goal != goal.Name) GetGreaterStrategies(strategies, belief, goal);
            //else {
            //    int GState = Convert.ToInt32(goal.State);
            //    belief.Net.UpdateBeliefs();
            //    double v = belief.Net.GetNodeValue(goal.Variable)[GState];
            //    strategies.Add(new Strategy(constant.SUBNET(goal.Variable), goal.Variable, v, GState, false));
            //}
            //if goals is a singular negative then return at this point
            return strategies;
        }

        private void GetGreaterStrategies(List<Strategy> strategies, Belief belief, Goal goal)
        {
            //IF goal is stateless then simply return the highest value of variable.
            if (goal.State == "") { getHighestVariableValue(strategies, belief); return; }
   
            Constants c = new Constants();
            try
            {
                belief.Net.UpdateBeliefs();
                double[] premValues = belief.Net.GetNodeValue(goal.Variable);
                string[] labels = belief.Net.GetOutcomeIds(belief.Variable);
                
                for (int i = 0; i < belief.Net.GetOutcomeIds(belief.Variable).Length; i++)
                {
                    belief.Net.SetEvidence(belief.Variable, i);
                    belief.Net.UpdateBeliefs();
                    double[] values = belief.Net.GetNodeValue(goal.Variable);
                    bool isDenial = false;
                    if (labels[i].ToUpper() == Constants.FALSE.ToUpper()) isDenial = true;
                    getGreater(strategies, values, premValues, i, belief.Variable, c.SUBNET(belief.Variable), Convert.ToInt32(goal.State), isDenial);
                }
                belief.Net.WriteFile("Debug/" + "P_" + belief.Variable + ".xdsl");
            }
            catch (Exception e)
            {
                log.ErrorFormat("'{0}'\n subnet '{1}', seed '{2}' ", e.Message, c.SUBNET(belief.Variable), belief.Variable);
                belief.Net.WriteFile("Debug/" + "P_" + belief.Variable + ".xdsl");
            }
        }

        private void getHighestVariableValue(List<Strategy> strategies, Belief belief)
        {
            Constants c = new Constants();
            double[] labels;
            try
            {
                labels = belief.Net.GetNodeValue(belief.Variable);
            }
            catch (Exception e)
            {
                log.ErrorFormat("'{0}'\n subnet '{1}', seed '{2}' ", e.Message, c.SUBNET(belief.Variable), belief.Variable);
                belief.Net.WriteFile("Debug/" + "P_" + belief.Variable + ".xdsl");
                return;
            }
            
            double Max = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                double value = labels[i];
                if (value > Max)
                {
                    strategies.Clear();
                    Max = value;
                }
                if (value == Max) strategies.Add(new Strategy(c.SUBNET(belief.Variable), belief.Variable, value, i, false));
            }
        }

        private bool InContext(ContextSubnet contextSubnet, string variable, string goal)
        { if (!contextSubnet.Knowledges.ContainsKey(variable) || !contextSubnet.Knowledges.ContainsKey(goal)) return false; return true; }

        public void GetParentStrategies(Dictionary<string, Dictionary<string, Strategy>> strategies, string seed_name, ContextSubnet contextSubnet)
        {
            Constants constant = new Constants();
            Subnet variableSubnet = contextSubnet.Knowledges[seed_name];
            string subnet = constant.SUBNET(variableSubnet.Name);

            //GetStrategies(strategies, seed_name, subnet, variableSubnet);
            List<Relation> parents = variableSubnet.Parents;
            foreach (Relation parent in parents)
            {
                if (contextSubnet.Knowledges.ContainsKey(parent.name))
                    GetStrategies(strategies, parent.name, subnet, contextSubnet.Knowledges[parent.name]);
            }
        }

        public void GetDirectStrategies(Dictionary<string, Dictionary<string, Strategy>> strategies, string seed_name, ContextSubnet contextSubnet)
        {
            Constants constant = new Constants();
            Subnet variableSubnet = contextSubnet.Knowledges[seed_name];
            string subnet = constant.SUBNET(variableSubnet.Name);

            GetStrategies(strategies, seed_name, subnet, variableSubnet);            
        }

        public void GetStrategies(Dictionary<string, Dictionary<string, Strategy>> strategies, string seed_name, string contextSubnet, Subnet variableSubnet)
        {
            Bayes net = new Bayes();
            variableSubnet.Net.WriteFile(string.Format("utility/tests/parents_{0}.xdsl", seed_name));
            string seed = seed_name;
            if (variableSubnet.Name == seed_name) seed = seed_name + '_' + Constants.PREMISE;
            try
            {
                variableSubnet.Net.UpdateBeliefs();
                //log.InfoFormat("The seed is {0}", seed);
                double[] values = variableSubnet.Net.GetNodeValue(seed);
                if (values.Length == 2)
                {
                    //get the contradiction between the variable state and the statement
                    getContradiction(strategies, values[0], 1, 0, seed_name, contextSubnet, false);
                    getContradiction(strategies, values[0], 0, 1, seed_name, contextSubnet, true);
                    return;
                } 
                for (int i = 0; i < values.Length; i++)
                {
                    getContradiction(strategies, values[i], 1, i, seed_name, contextSubnet, false);
                    //getContradiction(strategies, values[i], 0, i, seed_name, contextSubnet, true);
                }                             
            }
            catch (Exception e)
            {
                log.ErrorFormat("'{0}'\n subnet '{1}', seed '{2}' ", e.Message, variableSubnet.Name, seed_name);
                variableSubnet.Net.WriteFile("Debug/" + "P_" + variableSubnet.Name + ".xdsl");
            }
        }

        /// <summary>
        /// find the contradiction, negative means less and positive greater
        /// </summary>
        /// <param name="strategies"></param>
        /// <param name="value"></param>
        /// <param name="claim"></param>
        /// <param name="state"></param>
        /// <param name="variable"></param>
        /// <param name="submodel"></param>
        private void getContradiction(Dictionary<string, Dictionary<string, Strategy>> strategies, double value, int claim, int state, string variable, string submodel, bool isDenial)
        {
            Strategy strategy = new Strategy(submodel, variable, Math.Abs(claim - value), state, 1, isDenial);
            //log.InfoFormat("getContradiction: variable values[0] '{0}', claim - value '{1}', claim '{2}', contradiction = '{3}'"
              //  , value, claim - value, claim, strategy.Value);
            if (!strategies.ContainsKey(variable)) strategies.Add(variable, new Dictionary<string, Strategy>());
            if (!strategies[variable].ContainsKey(Convert.ToString(state))) strategies[variable].Add(Convert.ToString(state), strategy);
            else
            {
                strategies[variable][Convert.ToString(state)].Value += strategy.Value;
                strategies[variable][Convert.ToString(state)].Total += 1;
            }
        }

        private void getGreater(List<Strategy> strategies, double[] values, double[] premValues, int stateIDX, string variable, string submodel, int goalIDX, bool isDenial)
        {
            log.InfoFormat("Greater: variable IDX '{0}', goal IDX '{1}', A: (value '{2}') B: (value '{3}')", stateIDX, goalIDX, values[goalIDX], premValues[goalIDX]);
            if (values[goalIDX] >= premValues[goalIDX]) strategies.Add(new Strategy(submodel, variable, (values[goalIDX] - premValues[goalIDX]), stateIDX, isDenial));
        }

        private void getGreaterOrEqual(List<Strategy> strategies, double[] values, double[] premValues, int stateIDX, Relation variable, int goalIDX, bool isDenial)
        {
            if (values[goalIDX] >= premValues[goalIDX]) strategies.Add(new Strategy(variable.submodel, variable.name, (values[goalIDX] - premValues[goalIDX]), stateIDX, isDenial));
        }       
    }
}
