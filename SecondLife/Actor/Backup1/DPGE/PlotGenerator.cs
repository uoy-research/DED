using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DED.Utils;
using DED.Utils.Reads;
using DED.Director;
using DED.Decision;
using log4net;
using log4net.Config;
using DED.NPC;

namespace DED.DPGE
{
    class PlotGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlotGenerator));
        public PlotGenerator() { XmlConfigurator.Configure(); }

        List<Variable> variables = new List<Variable>();

        public void SetState(int state, string variable, Bayes net)
        {
            string id = variable + "_" + Constants.PREMISE;
            try
            {
                //net.WriteFile(string.Format("debug/{0}.xdsl", id));
                int size = net.TableSize(id);
                double[] d = new double[size];
                for (int i = 0; i < size; i++) d[i] = 0;
                for (int i = state; i < size; i += net.GetOutcomeCount(id)) d[i] = 1;
                //for (int i = 0; i < size; i++) log.InfoFormat("Temp: IDX '{0}', value '{1}'", i,d[i]);
                net.SetNodeDefinition(id, d);
                net.WriteFile(string.Format("debug/{0}.xdsl", id));
            }
            catch (Exception e)
            {
                log.ErrorFormat("'{0}'\n ", e.Message);
            }
        }

        void SetParents(Variable v, ContextSubnet subnet)
        {
            List<Strategy> responces = new List<Strategy>();
            Goal goal = new Goal(v.Name, 0, v.Name, Convert.ToString( v.State ));
            UtilityGreaterVariable util = new UtilityGreaterVariable(subnet, v.Name);
            foreach (Strategy s in util.GetStrategies(goal)) responces.Add(s);
            Random r = new Random();
            int n = r.Next(responces.Count - 1);
            Strategy strategy = responces[n];
            v.Name = strategy.Variable;
            v.State = strategy.State;
        }
                

        public void SetPlot(Knowledgebase kb, ReadPlot plotSettings)
        {

            foreach (Variable v in plotSettings.Variables.Values)
            {
                List<ContextSubnet> subnets = kb.GetSubnets(v.Subnet);

                int state = v.State;

                foreach (ContextSubnet s in subnets)
                {

                    if ( state == Constants.ANY || !v.Singular )
                    {
                        if (v.SetParent) SetParents(v,s);
                        else {
                            Random r = new Random();
                            //setState = Convert.ToString(s.Nodes[v.Name].States.Count - 1);                    
                            v.State = r.Next(s.Nodes[v.Name].States.Count - 1);
                        }
                    }

                    v.State_label = s.Nodes[v.Name].States[v.State];
                    //this.SetState(v.State, v.Name, s.Knowledges[v.Name].Net);
                }
            }
        }

        internal void SetSingularVariables(PlotNetwork plotNetwork, ReadPlot plotSettings)
        {
            Random r = new Random();

            
            foreach (Variable v in plotSettings.Variables.Values)
            {
                if (v.State != Constants.ANY || !v.Singular) continue;
                List<Subnet> subnets = GetSubnetsInReangeOfName(plotNetwork,v.Subnet);
                foreach (Subnet subnet in subnets)
                {
                    //= plotNetwork.dictPlot[v.Subnet];
                    //get the parent value and set it in the subnet.
                    foreach (string p in v.Parents)
                    {
                        subnet.Net.UpdateBeliefs();
                        int i = plotSettings.Variables[p].State;
                        subnet.Net.SetEvidence(p, plotSettings.Variables[p].State);
                    }
                    subnet.Net.UpdateBeliefs();
                    //set by probability of most likely    
                    //get rand flaot between 0 and 1
                    double rand = r.NextDouble();
                    double[] d = subnet.Net.GetNodeValue(v.Name);

                    double sum = 0;
                    for (int i = 0; i < d.Length; i++)
                    {
                        sum += d[i];
                        if (rand <= sum)
                        {
                            v.State = i;
                            break;
                        }
                    }

                    v.State_label = subnet.Nodes[v.Name].States[v.State];
                }     
            }
        }

        private List<Subnet> GetSubnetsInReangeOfName(PlotNetwork plotNetwork, string p)
        {
            List<Subnet> subnets = new List<Subnet>();

            foreach ( string network in plotNetwork.dictPlot.Keys ){
                if (network.Contains(p)) subnets.Add(plotNetwork.dictPlot[network]);
            }

            return subnets;
        }        
    }
}
