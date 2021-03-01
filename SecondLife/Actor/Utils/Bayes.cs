using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Smile;
using DED.DPGE;
using DED.NPC;

using log4net;
using log4net.Config;

namespace DED.Utils
{
    public class Bayes : Network
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bayes));
        const int WIDTH = 130;
        const int HEIGHT = 50;
        public Bayes(string name)
        {
            XmlConfigurator.Configure();
            this.Name = name;
        }

        public Bayes()
        {
            XmlConfigurator.Configure();
        }

        public void AddVariable(Node n)
        {
            AddVariable(n.ID, n);            
        }

        public void AddVariable(string id, Node n)
        {
            if (n.States.Count < 2)
            {
                Console.WriteLine(string.Format("ERROR, trying to create a variable with less than 2 states.\n {1} has {0} states", n.States.Count, n.Name));
                return;
            }

            AddNode(NodeType.Cpt, id);
            SetNodeName(id, n.Name);
            Rectangle r = new Rectangle(n.X, n.Y, WIDTH, HEIGHT);
            SetNodePosition(id, r.X, r.Y, r.Width, r.Height);

            double part = 1 / n.States.Count;
            ArrayList al = new ArrayList();
            for (int each = 0; each < n.States.Count; each++ )
            {
                al.Add(part);
                if (each < 2) SetOutcomeId(id, each, "n"+(string)n.States[each]);
                else AddOutcome(id, "n"+(string)n.States[each]);
            }
            SetNodeDefinition(id, (double[])al.ToArray(typeof(double)));
        }

        public void AddParents(Node n, string submodel, Dictionary<string, Subnet> h, Dictionary<string, Node> l)
        {
            foreach (string pID in n.Parents){
                string[] ar = pID.Split('$');
                //if no other net declared then add the arc with in this net
                if (ar.Length == 1)
                {
                    try
                    {
                        AddArc(pID, n.ID);
                    }
                    catch (SmileException e)
                    {
                        Console.WriteLine("Exception {0}, Message: {1}", e.Data, e.Message);
                    }
                }
                //else check if it the parent node has been created and if not create it
                //then add the arc
                else
                {
                    if (!l.ContainsKey(ar[0]))
                    {
                        try
                        {
                            Node newNode = new Node(h[ar[1]].Nodes[ar[0]]);
                            newNode.X = 1200;
                            newNode.Y = (l.Count * 100)+100;
                            // mark the new node as a parent
                            newNode.IsParent = true;
                            AddVariable(newNode);
                            //add the parent nod to a list in order to add it to the subnet later on
                            l.Add(ar[0], newNode);
                            
                        }
                        catch (SmileException e)
                        {
                            Console.WriteLine("Exception {0}, Message: {1}\n The key is '{2}' and the variable is '{3}'"
                                , e.Data, e.Message, ar[1], ar[0]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception {0}, Message: {1}\n The key is '{2}' and the variable is '{3}'"
                                , e.Data, e.Message, ar[1], ar[0]);
                        }
                    } 

                    try
                    {
                        //add heir to the parent subnet.
                        h[ar[1]].Heirs.Add(new Relation( ar[0], submodel) );
                        //add parent to the heirs subnet.
                        h[submodel].Parents.Add(new Relation( ar[0], h[ar[1]].Name));
                        AddArc(ar[0], n.ID);                        
                    }
                    catch (SmileException e)
                    {
                        Console.WriteLine("Exception {0}, Message: {1}", e.Data, e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception {0}, Message: {1}\n The key is '{2}' and the variable is '{3}'"
                            , e.Data, e.Message, ar[1], ar[0]);
                    }
                }
            }
        }


        public void AddParents(Node n)
        {
            foreach (string pID in n.Parents)
            {
                string[] ar = pID.Split('$');
                try
                {
                    AddArc(pID, n.ID);
                }
                catch (SmileException e)
                {
                    Console.WriteLine("Exception {0}, Message: {1}", e.Data, e.Message);
                }
            }
        }

        public void AddUtility(Node n)
        {
            AddNode(NodeType.Table, n.ID);
            SetNodeName(n.ID, n.Name);
            Rectangle r = new Rectangle(n.X, n.Y, WIDTH, HEIGHT);
            SetNodePosition(n.ID, r.X, r.Y, r.Width, r.Height);
            SetNodeBgColor(n.ID, Color.Blue);
        }

        public void AddDecision(Node n)
        {
            AddNode(NodeType.List, n.ID);
            SetNodeName(n.ID, n.Name);
            Rectangle r = new Rectangle(n.X, n.Y, WIDTH, HEIGHT);
            SetNodePosition(n.ID, r.X, r.Y, r.Width, r.Height);
            SetNodeBgColor(n.ID, Color.Green);

            if (n.States.Count < 2)
            {
                Console.WriteLine(string.Format("ERROR, trying to create a Decision variable with less than 2 states.\n {1} has {0} states", n.States.Count, n.Name));
                return;
            }

            for (int each = 0; each < n.States.Count; each++)
            {
                if (each < 2) SetOutcomeId(n.ID, each, n.States[each]);
                else AddOutcome(n.ID, n.States[each]);
            }
        }

        public void Normalize(Node n)
        {
            //find number of columns in the table.
            int columns = n.Values.Count / n.States.Count;
            //we will colect the total for each column called sum.
            double sum = 0;
            //Then loop through columns
            for (int i = 0; i < columns; i++)
            {
                //then loop through states and add up total sum
                for (int z = 0; z < n.States.Count; z++)
                {
                    sum += n.Values[(n.States.Count * i) + z];
                }
                //if sum not null then enter normalized value into the field
                if (sum != 0)
                {
                    for (int z = 0; z < n.States.Count; z++)
                    {
                        if (n.Values[(n.States.Count * i) + z] != 0)
                        {
                            double value = n.Values[(n.States.Count * i) + z] / sum;
                            n.Values[(n.States.Count * i) + z] = value;
                        }
                    }
                }
                //remember to set sum to 0
                sum = 0;
            }
        }

        public int TableSize(Node n)
        {
            int size = n.States.Count;
            foreach (string parent in GetParentIds(n.ID))
            {
                size *= GetOutcomeCount(parent);
            }
            return size;
        }

        public int TableSize(string variable)
        {
            int size = GetOutcomeCount(variable);
            foreach (string parent in GetParentIds(variable))
            {
                size *= GetOutcomeCount(parent);
            }
            return size;
        }

        public void DeleteParentsArcs(Node n)
        {
            foreach (string parent in GetParentIds(n.ID))
            {
                DeleteArc(parent,n.ID);
            }
        }

        public void Calculate(Dictionary<string, ContextSubnet> k)
        {
            log.InfoFormat("Enter Bayes Calculations, Sise of K is '{0}'", k.Count);

            List<Subnet> nonValidSubnets = new List<Subnet>();
            Dictionary<string, Subnet> calculateSubnets = new Dictionary<string, Subnet>();
            //walk through each subnet and foreach nonvalid subnet 
            //mark all its chilren non valid and add them to a list
            //then do the same for the list untill no more nodes are on the list.
            foreach (ContextSubnet aSubnet in k.Values)
            {
                if (aSubnet.IsValid) continue;
                foreach (Subnet subnet in aSubnet.Knowledges.Values)
                {
                    if (subnet.IsValid) continue;
                    //these are nonvalid
                    nonValidSubnets.Add(subnet);
                    //these will be calculated
                    if (!calculateSubnets.ContainsKey(subnet.Name))
                    {
                        calculateSubnets.Add(subnet.Name, subnet);
                    }
                }
            }
            //if there are no nonvalid subnets then the knowledge base is up to date.
            if (calculateSubnets.Count == 0) return;

            log.InfoFormat("number of calcsubnets: {0}", calculateSubnets.Count);

            while (true)
            {
                //reset
                List<Subnet> newNonvalids = new List<Subnet>();

                foreach (Subnet subnet in nonValidSubnets)
                {
                    foreach (Relation childSubnet in subnet.Heirs)
                    {
                        Subnet child = k[childSubnet.submodel].Knowledges[childSubnet.name];
                        if (child.IsValid)
                        {
                            if (!calculateSubnets.ContainsKey(childSubnet.name))
                            {
                                //then set child as nonvalid
                                child.IsValid = false;
                                //k[parent.submodel].Knowledges[subnet.name].Valid = false;
                                //and add it to the list
                                newNonvalids.Add(child);
                                calculateSubnets.Add(child.Name, child);
                            }
                        }
                        
                    }
                }
                if (newNonvalids.Count == 0) break;
                nonValidSubnets = newNonvalids;
            }

            log.InfoFormat("number of calcsubnets: {0}", calculateSubnets.Count);

            while (true)
            {
                //reset
                Dictionary<string, Subnet> newCalculates = new Dictionary<string, Subnet>();

                foreach (Subnet subnet in calculateSubnets.Values)
                {
                    //first check if parent is nonvalid
                    if (!subnet.IsValid)
                    {
                        bool parentsValid = true;
                        foreach (Relation parent in subnet.Parents)
                        {
                            //log.InfoFormat("parent {0}, parent subnet {1}, valid '{2}'"
                               // , parent.name, parent.submodel, k[parent.submodel].Knowledges[parent.name].Valid);
                            //first check if parent is nonvalid
                            if (!k[parent.submodel].Knowledges[parent.name].IsValid)
                            {
                                parentsValid = false;
                                newCalculates.Add(subnet.Name, subnet);
                                break;
                            }

                        }

                        if (parentsValid)
                        {
                            log.InfoFormat("Calculating subnet {0}", subnet.Name);
                            subnet.IsValid = true;
                            //calculate net
                            subnet.Net.UpdateBeliefs();
                            //set the values in the nodes in heirs
                            foreach (Relation heir in subnet.Heirs)
                            {
                                string preID = subnet.Name + '_' + Constants.PREMISE;
                                //string heirID = heir.name + '_' + Constants.PREMISE;
                                double[] d = subnet.Net.GetNodeValue(preID);
                                k[heir.submodel].Knowledges[heir.name].Nodes[subnet.Name].Values = new List<double>(d);
                                k[heir.submodel].Knowledges[heir.name].Net.SetNodeDefinition(subnet.Name, d);
                                //for (int i = 0; i < d.Length; i++) log.InfoFormat("Calc: IDX '{0}', value '{1}', heir '{2}, subnet '{3}'"
                                   // , i, d[i], heir.name, heir.submodel);
                            }                            
                        }
                    }
                }
                if (newCalculates.Count == 0) break;
                calculateSubnets = newCalculates;
            }
        }

        internal void Calculate(ContextSubnet contextSubnet, Belief belief)
        {
            if (contextSubnet.IsValid) return;
            List<Subnet> nonValidSubnets = new List<Subnet>();
            Dictionary<string, Subnet> calculateSubnets = new Dictionary<string, Subnet>();
            //walk through each subnet and foreach nonvalid subnet 
            //mark all its chilren non valid and add them to a list
            //then do the same for the list untill no more nodes are on the list.
            
            foreach (Subnet subnet in contextSubnet.Knowledges.Values)
            {
                if (subnet.IsValid) continue;
                //these are nonvalid
                nonValidSubnets.Add(subnet);
                
                //these will be calculated
                if (!calculateSubnets.ContainsKey(subnet.Name))
                {
                    calculateSubnets.Add(subnet.Name, subnet);
                }
            }
            
            //if there are no nonvalid subnets then the knowledge base is up to date.
            if (calculateSubnets.Count == 0) return;

            
            log.InfoFormat("number of calcsubnets: {0}", calculateSubnets.Count);

            while (true)
            {
                //reset
                List<Subnet> newNonvalids = new List<Subnet>();

                foreach (Subnet subnet in nonValidSubnets)
                {
                    foreach (Relation childSubnet in subnet.Heirs)
                    {
                        Subnet child = contextSubnet.Knowledges[childSubnet.name];
                        if (child.IsValid)
                        {
                            if (!calculateSubnets.ContainsKey(childSubnet.name))
                            {
                                //then set child as nonvalid
                                child.IsValid = false;
                                //k[parent.submodel].Knowledges[subnet.name].Valid = false;
                                //and add it to the list
                                newNonvalids.Add(child);
                                calculateSubnets.Add(child.Name, child);

                                
                            }
                        }
                    }
                }
                if (newNonvalids.Count == 0) break;
                nonValidSubnets = newNonvalids;
            }

            foreach (Subnet s in calculateSubnets.Values) belief.AddNet(s.Net, s.Name);
            belief.Net.WriteFile("E:/SecondLife/Actor/bin/Debug/utility/tests/" + belief.Name + ".xdsl");
            log.InfoFormat("number of calcsubnets: {0}", calculateSubnets.Count);

            while (true)
            {
                //reset
                Dictionary<string, Subnet> newCalculates = new Dictionary<string, Subnet>();

                foreach (Subnet subnet in calculateSubnets.Values)
                {
                    //first check if parent is nonvalid
                    if (!subnet.IsValid)
                    {
                        bool parentsValid = true;
                        foreach (Relation parent in subnet.Parents)
                        {
                            //Only the variables in context are to be used
                            if (!contextSubnet.Knowledges.ContainsKey(parent.name)) continue;
                            //log.InfoFormat("parent {0}, parent subnet {1}, valid '{2}'"
                             //, parent.name, parent.submodel, contextSubnet.Knowledges[parent.name].IsValid);
                            //first check if parent is nonvalid
                            if (!contextSubnet.Knowledges[parent.name].IsValid)
                            {
                                parentsValid = false;
                                newCalculates.Add(subnet.Name, subnet);
                                break;
                            }

                        }

                        if (parentsValid)
                        {
                            log.InfoFormat("Calculating subnet {0}", subnet.Name);
                            subnet.IsValid = true;
                            //calculate net
                            subnet.Net.UpdateBeliefs();
                            //subnet.Net.WriteFile("E:/SecondLife/Actor/bin/Debug/utility/tests/" + subnet.Name + ".xdsl");
                            //set the values in the nodes in heirs
                            foreach (Relation heir in subnet.Heirs)
                            {
                                //Only the variables in context are to be used
                                if (!contextSubnet.Knowledges.ContainsKey(heir.name)) continue;

                                string preID = subnet.Name + '_' + Constants.PREMISE;
                                //string heirID = heir.name + '_' + Constants.PREMISE;
                                double[] d = subnet.Net.GetNodeValue(preID);
                                contextSubnet.Knowledges[heir.name].Nodes[subnet.Name].Values = new List<double>(d);
                                contextSubnet.Knowledges[heir.name].Net.SetNodeDefinition(subnet.Name, d);
                                //for (int i = 0; i < d.Length; i++) log.InfoFormat("Calc: IDX '{0}', value '{1}', heir '{2}, subnet '{3}'"
                                // , i, d[i], heir.name, heir.submodel);
                            }
                        }
                    }
                }
                if (newCalculates.Count == 0) break;
                calculateSubnets = newCalculates;
            }
        }
    }
}
