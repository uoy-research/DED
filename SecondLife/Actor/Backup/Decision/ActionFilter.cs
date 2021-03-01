using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;

namespace DED.Decision
{
    class ActionFilter
    {
        public ActionFilter() { }

        public Dictionary<string, Relation> FilterActionsBySeeds(Dictionary<string, ActorSubnet> kb, Relation seed)
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


        public List<Strategy> GreaterOrEqualVariable(Dictionary<string, ActorSubnet> kb, Subnet subnet, int stateIDX)
        {
            Bayes net = subnet.Net;
            //calculate net and store the prem value.
            string premName = subnet.Name + '_' + Constants.PREMISE;
            net.UpdateBeliefs();
            double[] premValue = net.GetNodeValue(premName);

            //initiate the return value
            List<Strategy> strategies = new List<Strategy>();

            List<Relation> parents = subnet.Parents;
            foreach (Relation parent in parents)
            {                
                //foreach state in the parent
                for (int i = 0; i < net.GetOutcomeIds(parent.name).Length; i++ )
                {
                    // instantiate the node and calculate
                    net.SetEvidence(parent.name, i);
                    net.UpdateBeliefs();
                    //store the prem value and state where the value is >= to the original prem value.
                    getGreaterOrEqual(strategies, net.GetNodeValue(premName), premValue, stateIDX, parent, i);
                    //clean up 
                    net.ClearAllEvidence();
                }
            }
            //need to look at children too, this reverses teh process.
            List<Relation> heirs = subnet.Heirs;
            foreach (Relation heir in heirs)
            {
                //for each heir find their subnet
                Subnet heirSubnet = kb[heir.submodel].Knowledges[heir.name];
                //foreach state in the heir
                string heirname = heir.name + '_' + Constants.PREMISE;
                for (int i = 0; i < heirSubnet.Net.GetOutcomeIds(heirname).Length; i++)
                {
                    // instantiate the node and calculate
                    heirSubnet.Net.SetEvidence(heirname, i);
                    heirSubnet.Net.UpdateBeliefs();
                    //store the prem value and state where the value is < to the original prem value.
                    getGreaterOrEqual(strategies, heirSubnet.Net.GetNodeValue(subnet.Name), premValue, stateIDX, heir, i);
                    //clean up 
                    heirSubnet.Net.ClearAllEvidence();
                }
            }

            return strategies;
        }

        public List<Strategy> LessVariable(Dictionary<string, ActorSubnet> kb, Subnet subnet, int stateIDX)
        {
            Bayes net = subnet.Net;
            //calculate net and store the prem value.
            string premName = subnet.Name + '_' + Constants.PREMISE;
            net.UpdateBeliefs();
            double[] premValue = net.GetNodeValue(premName);

            //initiate the return value
            List<Strategy> strategies = new List<Strategy>();

            List<Relation> parents = subnet.Parents;
            foreach (Relation parent in parents)
            {
                //foreach state in the parent
                for (int i = 0; i < net.GetOutcomeIds(parent.name).Length; i++)
                {
                    // instantiate the node and calculate
                    net.SetEvidence(parent.name, i);
                    net.UpdateBeliefs();
                    //store the prem value and state where the value is < to the original prem value.
                    getLess(strategies, net.GetNodeValue(premName), premValue, stateIDX, parent, i );
                    //clean up 
                    net.ClearAllEvidence();
                }
            }
            //need to look at children too, this reverses teh process.
            List<Relation> heirs = subnet.Heirs;
            foreach (Relation heir in heirs)
            {
                //for each heir find their subnet
                Subnet heirSubnet = kb[heir.submodel].Knowledges[heir.name];
                //foreach state in the heir
                string heirname = heir.name + '_' + Constants.PREMISE;
                for (int i = 0; i < heirSubnet.Net.GetOutcomeIds(heirname).Length; i++)
                {
                    // instantiate the node and calculate
                    heirSubnet.Net.SetEvidence(heirname, i);
                    heirSubnet.Net.UpdateBeliefs();
                    //store the prem value and state where the value is < to the original prem value.
                    getLess(strategies, heirSubnet.Net.GetNodeValue(subnet.Name), premValue, stateIDX, heir, i);
                    //clean up 
                    heirSubnet.Net.ClearAllEvidence();
                }
            }
            return strategies;
        }

        private void getGreaterOrEqual(List<Strategy> strategies, double[] values, double[] premValues, int stateIDX, Relation variable, int idx)
        {
            if (values[stateIDX] >= premValues[stateIDX]) strategies.Add(new Strategy(variable.submodel, variable.name, (values[stateIDX] - premValues[stateIDX]), idx));
        }

        private void getLess(List<Strategy> strategies, double[] values, double[] premValues, int stateIDX, Relation variable, int idx)
        {
            if (values[stateIDX] < premValues[stateIDX]) strategies.Add(new Strategy(variable.submodel, variable.name, (premValues[stateIDX] - values[stateIDX]), idx));
        }
    }
}
