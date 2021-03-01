using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;
using DED.Director;
using log4net;
using log4net.Config;
using DED.DPGE;


namespace DED.Decision
{
    class IntegrityUtility: Base_utility
    {
        /// <summary>
        /// dictionary of contradiction values, variable, state
        /// </summary>
        Dictionary<string, Dictionary<string, Strategy>> strategies = new Dictionary<string, Dictionary<string, Strategy>>();
        List<Strategy> optimals = new List<Strategy>();
        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrityUtility));

        ContextSubnet contextSubnet;
        string variable;
        Knowledgebase KB;
        Bayes net = new Bayes();
        List<double> contradictionValues = new List<double>();
        List<double> integrityValues = new List<double>();
        public List<double> IntegrityValues { set { this.integrityValues = value; } }
        

        public IntegrityUtility(Knowledgebase KB, ContextSubnet contextSubnet, string variable)
        {
            XmlConfigurator.Configure();
            this.contextSubnet = contextSubnet;
            this.variable = variable;
            this.KB = KB;
        }

        public List<Strategy> GetDirectStrategies()
        {           
            QueryDirectKB(this.contextSubnet);
            if (this.strategies.Count == 0) return this.optimals;
            BuildUtilNet();
            IntegrityUtilityEval();
            return this.optimals;
        }

        public override List<Strategy> GetStrategies(Goal goal)
        {
            //do we need to calculate or does the value exist
            bool NeedCalculate = false;
            QueryKB(this.contextSubnet);  
            //TODO  not in cache: query opponents kb's and store in cache
            foreach ( Dictionary<string, ContextSubnet> opponent in KB.GetOpponents.Values ) {
                ContextSubnet context = opponent[this.contextSubnet.Name];
                if (!IsValueInCache() || !IsOpponentsKBValid(context))
                { 
                    //Query the knowledgebase.
                    //QueryKB(context);              
                    NeedCalculate = true;    
                }
            }

            //if in cache and if opponents kb valid, unchanged:
            if (!NeedCalculate && InCache()) return this.optimals;
            if (this.strategies.Count == 0) return this.optimals;
            BuildUtilNet();
            IntegrityUtilityEval();
            return this.optimals;
        }

        /// <summary>
        /// TODO is the resulting value in cache
        /// </summary>
        /// <returns></returns>
        private bool InCache()
        {
            return false;
        }

        private bool IsOpponentsKBValid(ContextSubnet context) { return context.IsValid; }

        /// <summary>
        /// TODO IsValueInCache
        /// </summary>
        /// <returns></returns>
        private bool IsValueInCache()
        {
            
            return false;
        }

        private void BuildUtilNet()
        {
            IntegrityVariable();            
            DecisionVariable();
            ContradictionVariable();
            UilityVariable();
            //parents            
            this.net.AddArc(Constants.DECISION, Constants.CONTRADICTION);
            this.net.AddArc(Constants.CONTRADICTION, Constants.UTILITY);
            this.net.AddArc(Constants.INTEGRITY, Constants.UTILITY);
            //definitions
            DefineContradiction();
            DefineUtility();
            this.net.SetNodeDefinition(Constants.INTEGRITY, this.integrityValues.ToArray());
            this.net.WriteFile(string.Format("utility/{0}_{1}.xdsl", Constants.INTEGRITY, this.variable));
            
        }

        private void DefineUtility()
        {
            Double[] ar = new Double[4];
            ar[0] = 0; ar[1] = 100; ar[2] = 100; ar[3] = 100;
            this.net.SetNodeDefinition(Constants.UTILITY, ar);
        }

        private void DefineContradiction()
        {
            List<Double> contradictions = new List<double>();
            int total = this.strategies.Count * 2;

            foreach (Dictionary<string,Strategy> s in this.strategies.Values)
                foreach (Strategy ss in s.Values) contradictions.Add(ss.Value / ss.Total);
            
            Values values = new Values();
            Node n = new Node();
            n.ValueFlag = "twoRows";
            n.Values = contradictions;
            values.SetValues(n,this.net);
            this.net.SetNodeDefinition(Constants.CONTRADICTION,n.Values.ToArray());
        }

        /// <summary>
        /// Contradiction has two states true and false, Decision as parents and 
        /// values are calcuated in the class each time
        /// </summary>
        private void ContradictionVariable()
        {
            Node n = new Node();
            n.ID = Constants.CONTRADICTION;
            n.Name = Constants.CONTRADICTION;
            List<string> states = new List<string>();
            states.Add(Constants.TRUE);
            states.Add(Constants.FALSE);
            n.States = states;
            net.AddVariable(n);
        }

        private void UilityVariable()
        {
            Node n = new Node();
            n.ID = Constants.UTILITY;
            n.Name = Constants.UTILITY;
            net.AddUtility(n);
        }

        private void DecisionVariable()
        {
            Node n = new Node();
            n.ID = Constants.DECISION;
            n.Name = Constants.DECISION;
            List<string> states = new List<string>();
            foreach (Dictionary<string, Strategy> s in this.strategies.Values)
            {
                int count = 0;
                foreach (Strategy ss in s.Values) {
                    states.Add(ss.Variable + ss.State);
                    ++count;
                }
            }

            n.States = states;
            net.AddDecision(n);
        }

        /// <summary>
        /// Integrity has two states true and false, no parents and 
        /// values are passed to the class each time
        /// </summary>
        private void IntegrityVariable()
        {
            Node n = new Node();
            n.ID = Constants.INTEGRITY;
            n.Name = Constants.INTEGRITY;
            List<string> states = new List<string>();
            states.Add(Constants.TRUE);
            states.Add(Constants.FALSE);
            n.States = states;
            net.AddVariable(n);
        }

        /// <summary>
        /// TODO calculate utility with variables(opponents K, self K, integrity) and store in cache
        /// </summary>
        private void IntegrityUtilityEval()
        {
            this.net.UpdateBeliefs();
            Double[] values = this.net.GetNodeValue(Constants.DECISION);
            string[] states = this.net.GetOutcomeIds(Constants.DECISION);
            //set the strategies and values
            int count = 0;
            foreach (Dictionary<string, Strategy> s in this.strategies.Values)
                foreach (Strategy ss in s.Values)
                {
                    ss.Value = values[count];
                    this.optimals.Add(ss);
                    ++count;
                }
        }        

        /// <summary>
        /// Return all contextual sentences with values [for/against, true value]
        /// </summary>        
        void QueryKB(ContextSubnet context)
        {
            ActionFilter af = new ActionFilter();
            af.GetParentStrategies(this.strategies, this.variable, context);
        }

        /// <summary>
        /// Return all contextual sentences with values [for/against, true value]
        /// </summary>        
        void QueryDirectKB(ContextSubnet context)
        {
            ActionFilter af = new ActionFilter();
            af.GetDirectStrategies(this.strategies, this.variable, context);
        }
    }
}
