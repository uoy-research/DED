using System;
using System.Collections.Generic;
using System.Text;
using DED.NPC;
using DED.Utils;
using Smile;
using DED.DPGE;
using log4net;
using log4net.Config;



namespace DED.Decision
{    
    struct Choice
    {
        public Choice(string name, string position, string subnet, int state, double value)
        {
            this.name = name;
            this.position = position;
            this.state = state;
            this.value = value;
            this.weight = 0;
            this.subnet = subnet;
        }

        string name;
        public string Name      { get { return this.name;       } }

        string subnet;
        public string Subnet { get { return this.subnet; } }


        string position;
        public string Position  { get { return this.position;   } }

        int state;
        public int State        { get { return this.state;      } }

        double value;
        public double Value     { get { return this.value;      } set { this.value = value;     } }

        double weight;
        public double Weight    { get { return this.weight;     } set { this.weight = value;    } }
    }

    struct Optimal
    {
        public Optimal(string key, double value, List<Choice> choices)
        {
            this.key = key;
            this.value = value;
            this.choices = choices;
        }

        string key;
        public string Key               { get { return this.key;        } }

        double value;
        public double Value             { get { return this.value;      } }

        List<Choice> choices;
        public List<Choice> Choices     { get { return this.choices;    } }
    }

    abstract class Base_utility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Base_utility));

        protected Optimal optimal = new Optimal();
        protected Dictionary<string, Optimal> results = new Dictionary<string,Optimal>();
        protected Dictionary<string, Optimal> Results { get { return this.results; } }

        protected Dictionary<string, Optimal> reactions = new Dictionary<string, Optimal>();
        protected Dictionary<string, Optimal> Reactions { get { return this.reactions; } }

        protected Dictionary<string, List<Choice>> innerResults = new Dictionary<string, List<Choice>>();
        Bayes net = new Bayes();

        public Base_utility()
        {
            XmlConfigurator.Configure();            
        }

        public abstract List<Strategy> GetStrategies(Dictionary<string, ActorSubnet> kb, Subnet subnet, string state);
      

        public void Contradiction(Knowledgebase kb, string character, string opponent, Dictionary<string, Relation> actions)
        {
            
            foreach (Relation r in actions.Values)
            {
                Subnet subnet;
                if (kb.Actor == character)
                {
                    //this is the action! We estimate whether the character is contradicting the opponent.
                    subnet = kb.Opponents[opponent][r.submodel].Knowledges[r.name];
                }
                else
                {
                    //this is the reaction! We estimate whether the opponent is contradicting the actor.
                    subnet = kb.OpponentsOpponents[character][opponent][r.submodel].Knowledges[r.name];
                }
                Calculate(subnet, r);
            }            
        }

        public void Risk(Knowledgebase kb, string character, string opponent, Dictionary<string, Relation> actions)
        {
            
            foreach (Relation r in actions.Values)
            {
                Subnet subnet;
                if (kb.Actor == opponent)
                {
                    //this is the action! We estimate whether the character is taking a risk 
                    subnet = kb.Character[r.submodel].Knowledges[r.name];
                }
                else
                {
                    //this is the reaction! We estimate whether the opponent is taking a risk.
                    subnet = kb.Opponents[character][r.submodel].Knowledges[r.name];
                }
                Calculate( subnet, r );
            }
        }

        private void Calculate(Subnet subnet, Relation r)
        {
            Node prem = subnet.Nodes[r.name + '_' + Constants.PREMISE];
            subnet.Net.WriteFile(string.Format("utility/{0}_action.xdsl", Constants.PREMISE));
            string key = Key(prem.ID, subnet.Net);
            if (results.ContainsKey(key)) return;
            //kb.Opponents[character]                    

            for (int i = 0; i < prem.States.Count; i++)
            {
                
                //for each state calc negative and positive
                log.DebugFormat("Enter states nr '{0}' for node '{1}'", i, prem.ID);
                //set premise
                double[] v = subnet.Net.GetNodeValue(prem.ID);
                double[] d = { v[i], 1 - v[i] };
                net.SetNodeDefinition(Constants.PREMISE, d);
                //calculate
                net.UpdateBeliefs();
                //store outcome in inner results.
                SetInnerResults(r.name, i, r.submodel, new List<double>(net.GetNodeValue(Constants.DECISION)));
            }

            //results.Add(key, new Optimal(key, innerResults[0].Value, innerResults));
        }


        private void SetPremiseNode( )
        {
            Node n = new Node();
            n.ID = Constants.PREMISE;
            n.Name = Constants.PREMISE;
            n.X = 50;
            n.Y = 100;
            n.States.Add(Constants.TRUE);
            n.States.Add(Constants.FALSE);
            net.AddVariable(n);
        }

        private void SetIntegrityNode()
        {            
            Node n = new Node();
            n.ID = Constants.INTEGRITY;
            n.Name = Constants.INTEGRITY;
            n.X = 250;
            n.Y = 100;
            n.States.Add(Constants.TRUE);
            n.States.Add(Constants.FALSE);
            n.Values.Add(0.6);
            n.Values.Add(0.4);
            net.AddVariable(n);
            net.SetNodeDefinition(Constants.INTEGRITY, (double[])n.Values.ToArray());
        }

        protected void DrawReactionNodes()
        {
            net = new Bayes();
            SetIntegrityNode();
            SetPremiseNode();
            SetDecision();
            SetEvaluationNode();
            SetUtility();            
            
            net.WriteFile(string.Format("utility/{0}_reaction.xdsl", Constants.UTILITY));
        }

        protected void DrawActionNodes()
        {
            net = new Bayes();
            SetIntegrityNode();
            SetPremiseNode();
            SetDecision();
            SetEvaluationNode();
            SetUtility();

            net.WriteFile(string.Format("utility/{0}_action.xdsl", Constants.UTILITY));
        }   

        private void SetDecision()
        {
            Node n = new Node();
            n.ID = Constants.DECISION;
            n.Name = Constants.DECISION;
            n.States = new List<string>();
            n.States.Add(Constants.TRUE);
            n.States.Add(Constants.FALSE);
            n.X = 50;
            n.Y = 200;
            net.AddDecision(n);
        }

        private void SetUtility()
        {
            Node n = new Node();
            n.ID = Constants.UTILITY;
            n.Name = Constants.UTILITY;
            double[] d = { 0, 1,1,0 };
            n.Values = new List<double>(d);
            n.X = 450;
            n.Y = 200;
            net.AddUtility(n);
            net.AddArc(Constants.EVALUATION, Constants.UTILITY);
            net.AddArc(Constants.EVALUATION, Constants.ATTRIBUTE);
            net.SetNodeDefinition(Constants.UTILITY, (double[])n.Values.ToArray());            
        }

        private void SetEvaluationNode()
        {
            Node n = new Node();
            n.ID = Constants.EVALUATION;
            n.Name = Constants.EVALUATION;
            n.States.Add(Constants.TRUE);
            n.States.Add(Constants.FALSE);
            n.Parents.Add(Constants.PREMISE);
            n.Parents.Add(Constants.DECISION);
            n.X = 250;
            n.Y = 200;
            net.AddVariable(n);
            net.AddParents(n);
            Values v = new Values();
            v.SetEvalIntegrity(n);
            net.SetNodeDefinition(n.ID, (double[])n.Values.ToArray());
        }

        private void SetInnerResults(string name, int state, string subnet, List<double> d)
        {
            string keyPro = name + state + Constants.PRO;
            string keyCon = name + state + Constants.CON;

            Choice cPro = new Choice(name, Constants.PRO, subnet, state, d[0]);
            Choice cCon = new Choice(name, Constants.CON, subnet, state, d[1]);
            if (!innerResults.ContainsKey(keyPro))
            {
                innerResults.Add(keyPro, new List<Choice>());
                innerResults.Add(keyCon, new List<Choice>());
            }
            innerResults[keyPro].Add(cPro);
            innerResults[keyCon].Add(cCon);
        }

        protected void SetReactions(string action)
        {
            foreach (List<Choice> ir in innerResults.Values)
            {
                double weight = 1.0/ir.Count;
                double weightedSum = 0;
                foreach (Choice c in ir)
                {
                    weightedSum += (c.Value * weight);
                }

                Choice opt = ir[0];
                opt.Value = weightedSum;

                if ( !reactions.ContainsKey(action) )
                { reactions.Add(action, new Optimal(action, weightedSum, new List<Choice>())); }
                
                if (weightedSum >= reactions[action].Value)
                { reactions[action] = new Optimal(action, weightedSum, new List<Choice>()); }

                reactions[action].Choices.Add(opt);
                
            }
        }

        protected void SetActions()
        {
            foreach (List<Choice> ir in innerResults.Values)
            {
                double weight = 1.0 / ir.Count;
                double weightedSum = 0;
                foreach (Choice c in ir)
                { weightedSum += (c.Value * weight); }

                Choice opt = ir[0];
                opt.Value = weightedSum;

                if (optimal.Choices == null || weightedSum > optimal.Value)
                { optimal = new Optimal("", weightedSum, new List<Choice>()); }

                optimal.Choices.Add(opt);               
            }
        }

        private string Key(string id, Bayes premNet)
        {
            string key = "";
            if (!premNet.IsValueValid(id)) return key;
                
            double[] d = premNet.GetNodeValue(id);
            foreach ( double v in d )
            {
                key += Math.Round(v,2);
            }
            log.DebugFormat("The key for {1} is '{0}'", key, id);
            return key;
        }
    }
}
