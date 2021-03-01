using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using DED.DPGE;
using DED.Utils.Reads;
using DED.Utils;
using DED.NPC;
using DED.Director;

namespace DED.Decision
{
    class UtilityDirectSpeech : Base_utility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrityUtility));
        Variable variable;
        Knowledgebase KB;

        public UtilityDirectSpeech(Knowledgebase KB, Variable variable)
        {
            XmlConfigurator.Configure();
            this.variable = variable;
            this.KB = KB;
        }

        public override List<Strategy> GetStrategies(Goal goal) {
            List<Strategy> strategies = new List<Strategy>();
            
            //get a rand real number and go through the values to choose the 
            Random rand = new Random();
            double val = rand.NextDouble();
            
            Bayes net = this.KB.GetSubnet(this.variable.Subnet).Knowledges[this.variable.Name].Net;
            //net.WriteFile("debug/Strat A " + this.variable.Subnet + "_" + this.variable.Name + ".xdsl");
            int count = 0;
            try
            {
                net.UpdateBeliefs();
                foreach (double value in net.GetNodeValue(this.variable.Name + "_" + Constants.PREMISE))
                {
                    if (value >= val) break;
                    ++count;
                }
            }
            catch (Exception e )
            {
                net.WriteFile("debug/error_" + this.variable.Subnet + "_" + this.variable.Name + ".xdsl");
                log.ErrorFormat("'{0}'\n ", e.Message);
            }
            this.variable.State = count;
            strategies.Add(new Strategy(this.variable));
            return strategies;            
        }
    }
}
