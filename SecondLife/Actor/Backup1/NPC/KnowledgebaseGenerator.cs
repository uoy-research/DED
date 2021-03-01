using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.DPGE;
using log4net;
using log4net.Config;
using System.Threading;
using DED.Director;


namespace DED.NPC
{
    public class ContextSubnet : Subnet
    {
        /// <summary>
        /// This is to create larger subnets from the plot
        /// </summary>
        /// <param name="subnet"></param>
        public ContextSubnet(Subnet subnet)
        {
            this.name = subnet.Name;
            this.nodes = new Dictionary<string, Node>(subnet.Nodes);
            this.net = new Bayes(this.name);
            this.heirs = new List<Relation>(subnet.Heirs);
            this.parents = new List<Relation>(subnet.Parents);
            this.valid = false;
            this.knowledges = new Dictionary<string, Subnet>();
        }        

        Dictionary<string, Subnet> knowledges;
        /// <summary>
        /// Knowledge subnets of that plotSubnet. 
        /// </summary>         
        public Dictionary<string, Subnet> Knowledges
        {
            get { return this.knowledges; }
        }               
    }

    public class Knowledgebase
    {
        //subnet
        Dictionary<string, ContextSubnet> kCharacter = new Dictionary<string, ContextSubnet>();
        //opponent, subnet
        Dictionary<string, Dictionary<string, ContextSubnet>> kOpponents = new Dictionary<string, Dictionary<string, ContextSubnet>>();
        //opponent, opponent, subnet
        Dictionary<string, Dictionary<string, Dictionary<string, ContextSubnet>>> kOpponentsOpponents = new Dictionary<string, Dictionary<string, Dictionary<string, ContextSubnet>>>();

        public ContextSubnet GetSubnet(string subnet) { if (this.kCharacter.ContainsKey(subnet)) return kCharacter[subnet]; System.Console.WriteLine("EEEEE-SUBNET '" + subnet + "' Does not exist"); return null; }
        public ContextSubnet GetSubnet(string subnet, string character) { return kOpponents[character][subnet]; }
        public ContextSubnet GetSubnet(string subnet, string character, string opponent) { return kOpponentsOpponents[character][opponent][subnet]; }

        public Dictionary<string, ContextSubnet> GetCharacterKB() { return kCharacter; }
        public Dictionary<string, ContextSubnet> GetCharacterKB(string character) { return kOpponents[character]; }
        public Dictionary<string, ContextSubnet> GetCharacterKB(string character, string opponent) { return kOpponentsOpponents[character][opponent]; }

        public Dictionary<string, Dictionary<string, ContextSubnet>> GetOpponents { get { return this.kOpponents; } }

        public List<ContextSubnet> GetSubnets(string subnet) {

            List<ContextSubnet> subnets = new List<ContextSubnet>();

            foreach (string network in this.kCharacter.Keys)
            {
                if (network.Contains(subnet)) subnets.Add(this.kCharacter[network]);
            }

            return subnets;            
        }


        //public string GetVictim() {
        //    Subnet ck = this.kCharacter["victim"].Knowledges["V_victim"];
        //    //double[] val = ck.Net.GetNodeValue

        //}

        internal void SetSubnet(string subnet, ContextSubnet contextSubnet)
        {
            if (!kCharacter.ContainsKey(subnet)) kCharacter.Add(subnet, contextSubnet);
        }

        internal void SetSubnet(string subnet, string character, ContextSubnet contextSubnet)
        {
            if (!kOpponents.ContainsKey(character))
            {
                kOpponents.Add(character, new Dictionary<string, ContextSubnet>());                
            }
            kOpponents[character].Add(subnet, contextSubnet);
        }

        internal void SetSubnet(string subnet, string character, string opponent, ContextSubnet contextSubnet)
        {
            if (!kOpponentsOpponents.ContainsKey(character))
            {
                kOpponentsOpponents.Add(character, new Dictionary<string, Dictionary<string, ContextSubnet>>());
            }
            if (!kOpponentsOpponents[character].ContainsKey(opponent))
            {                
                kOpponentsOpponents[character].Add(opponent, new Dictionary<string, ContextSubnet>());
            }
            kOpponentsOpponents[character][opponent].Add(subnet, contextSubnet);
        }
    }


    public class KnowledgebaseGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(KnowledgebaseGenerator));
        
        string actor;
        List<Actor> lOpponents = new List<Actor>();
        Knowledgebase KB = new Knowledgebase();
        //subnet
        //Dictionary<string, ContextSubnet> kCharacter = new Dictionary<string, ContextSubnet>();
        //opponent, subnet
        //Dictionary<string, Dictionary<string, ContextSubnet>> kOpponents = new Dictionary<string, Dictionary<string, ContextSubnet>>();
        //opponent, opponent, subnet
        //Dictionary<string, Dictionary<string, Dictionary<string, ContextSubnet>>> kOpponentsOpponents = new Dictionary<string, Dictionary<string, Dictionary<string, ContextSubnet>>>();

        public string Actor { get { return this.actor; } }
        //public Dictionary<string, ContextSubnet> Character { get { return this.kCharacter; } }
        //public Dictionary<string, Dictionary<string, ContextSubnet>> Opponents { get { return this.kOpponents; } }
        //public Dictionary<string, Dictionary<string, Dictionary<string, ContextSubnet>>> OpponentsOpponents { get { return this.kOpponentsOpponents; } }

        public KnowledgebaseGenerator(string actor)
        {
            XmlConfigurator.Configure();
            log.Info("Enter Knowledge base");
            this.actor = actor;
        }

        public Knowledgebase CreateKnowledgeBase(Dictionary<string, Subnet> k, List<Actor> lOpponents)
        {
            this.lOpponents = lOpponents;
            
            //loop through the subnets
            foreach ( Subnet plotSubnet in k.Values )
            {
                this.KB.SetSubnet(plotSubnet.Name, new ContextSubnet(plotSubnet));                

                //loop through all the opponents and 
                //foreach (Actor opponent in lOpponents) {
                    
                //    if (opponent.Name == actor) continue;                    
                //    this.KB.SetSubnet(plotSubnet.Name, opponent.Name , new ContextSubnet(plotSubnet));

                //    //loop through the opponents opponents
                //    foreach (Actor opponentOpponent in lOpponents)
                //    {
                         
                //        if (opponent.Name == opponentOpponent.Name) continue;
                //        this.KB.SetSubnet(plotSubnet.Name, opponent.Name, opponentOpponent.Name, new ContextSubnet(plotSubnet));
                //    }
                //}          
            }            

            //loop through the charcter subnets
            
            DrawNet( this.KB.GetCharacterKB(), k );
            //Bayes net = this.KB.GetSubnet("suspect").Knowledges["S_occupation"].Net;
            //net.WriteFile("debug/KB A_S_occupation.xdsl");

            System.Console.WriteLine( string.Format("{0}, {1} starts to update the knowledge base values", actor, DateTime.Now.TimeOfDay) );
            Calculate(this.KB.GetCharacterKB());
            
            //net = this.KB.GetSubnet("suspect").Knowledges["S_occupation"].Net;
            //net.WriteFile("debug/KB B_S_occupation.xdsl");
            //WriteKnowledgbase(this.KB.GetCharacterKB(), "caracter_" + this.actor + '_');
            //loop through all the opponents and 
            //foreach ( Actor opponent in lOpponents )
            //{                
            //    if (opponent.Name == actor) continue;
            //    DrawNet(this.KB.GetCharacterKB(opponent.Name), k);
            //    Calculate(this.KB.GetCharacterKB(opponent.Name));
            //    //WriteKnowledgbase(kOpponents[opponent], opponent + '_');
            //    //loop through the opponents opponents
            //    foreach ( Actor opponentopponent in lOpponents )
            //    {                    
            //        if ( opponent.Name == opponentopponent.Name ) continue;
            //        DrawNet(this.KB.GetCharacterKB(opponent.Name,opponentopponent.Name), k);
            //        Calculate(this.KB.GetCharacterKB(opponent.Name,opponentopponent.Name));
            //        //WriteKnowledgbase(kOpponentsOpponents[opponent][opponentopponent], opponent + '_' + opponentopponent + '_');
            //    }
            //}
            System.Console.WriteLine(string.Format("{0}, {1} has updated the knowledge base values", actor, DateTime.Now.TimeOfDay));
            return this.KB;
        }

        public void Calculate()
        {
            log.InfoFormat("Enter Calculate");
            Calculate(this.KB.GetCharacterKB());
            //foreach (Actor opponent in this.lOpponents)
            //{
            //    if (opponent.Name == actor) continue;
            //    Calculate(this.KB.GetCharacterKB(opponent.Name));
            //    foreach (Actor opponentopponent in this.lOpponents)
            //    {
            //        if (opponent.Name == opponentopponent.Name) continue;
            //        Calculate(this.KB.GetCharacterKB(opponent.Name, opponentopponent.Name));
            //    }
            //}
            log.InfoFormat("Exit Calculate");
        }

        private void Calculate(Dictionary<string, ContextSubnet> k)
        {
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
                        calculateSubnets.Add(subnet.Name,subnet);
                    }
                }
            }
            //if there are no nonvalid subnets then the knowledge base is up to date.
            if (nonValidSubnets.Count == 0) return;
            
            while ( true )
            {
                //reset
                List<Subnet> newNonvalids = new List<Subnet>();

                foreach (Subnet subnet in nonValidSubnets)
                {
                    foreach (Relation childSubnet in subnet.Heirs)
                    {
                        Subnet child = k[childSubnet.submodel].Knowledges[childSubnet.name];
                        if ( child.IsValid )
                        {             
                            if (!calculateSubnets.ContainsKey(childSubnet.name))
                            {
                                //then set child as nonvalid
                                subnet.IsValid = false;
                                //and add it to the list
                                newNonvalids.Add(child);
                                calculateSubnets.Add(child.Name, child);
                            }
                        }
                    }                    
                }
                if ( newNonvalids.Count == 0 ) break;
                nonValidSubnets = newNonvalids;
            }

            while ( true )
            {
                //reset
                Dictionary<string, Subnet> newCalculates = new Dictionary<string, Subnet>();

                foreach (Subnet subnet in calculateSubnets.Values)
                {
                    //first check if parent is nonvalid
                    if ( !subnet.IsValid )
                    {
                        bool parentsValid = true;
                        foreach ( Relation parent in subnet.Parents )
                        {
                            //first check if parent is nonvalid
                            if (!k[parent.submodel].Knowledges[parent.name].IsValid)
                            {
                                parentsValid = false;
                                newCalculates.Add(subnet.Name,subnet);
                                break;
                            }
                        }
                        if (parentsValid)
                        {
                            subnet.IsValid = true;
                            //calculate net
                            subnet.Net.UpdateBeliefs();
                            //set the values in the nodes in heirs
                            foreach (Relation heir in subnet.Heirs)
                            {
                                string preID = subnet.Name + '_' + Constants.PREMISE;
                                string heirID = heir.name + '_' + Constants.PREMISE;
                                double[] d = subnet.Net.GetNodeValue(preID);
                                k[heir.submodel].Knowledges[heir.name].Nodes[subnet.Name].Values = new List<double>(d);
                                k[heir.submodel].Knowledges[heir.name].Net.SetNodeDefinition(subnet.Name, d);
                            }
                            log.DebugFormat("Calculating subnet {0}", subnet.Name);
                        }
                    }
                }
                if (newCalculates.Count == 0) break;
                calculateSubnets = newCalculates;
            }
        }

        void DrawNet(Dictionary<string, ContextSubnet> actorKB, Dictionary<string, Subnet> k)
        {

            foreach (ContextSubnet aSubnet in actorKB.Values)
            {
                foreach (Node n in aSubnet.Nodes.Values)
                {
                    //first check that this node belongs to this subnet
                    if (n.IsParent) continue;

                    //each node from the plot gets a private Bayesian net that is stored alongside relevant info in 
                    //a Knowledge struct.
                    Subnet knowledge = new Subnet(n.ID);
                    

                    // intitalize the premise node
                    Node pre = new Node(n);
                    pre.ID = n.ID + '_' + Constants.PREMISE;
                    pre.Name = n.ID + ' ' + Constants.PREMISE;

                    //initialize the inference node.
                    Node inf = new Node(n);
                    inf.ID = n.ID + '_' + Constants.INFERENCE;
                    inf.Name = n.ID + ' ' + Constants.INFERENCE;
                    //inf.Parents.Add(pre.ID);
                    //inf.States = n.States;


                    //add inf and premise to the Knowledge.net
                    knowledge.Net.AddVariable(pre.ID, pre);
                    knowledge.Net.AddVariable(inf.ID, inf);

                    //add inf and prem to the Knowledge.dict
                    knowledge.Nodes.Add(pre.ID, pre);
                    knowledge.Nodes.Add(inf.ID, inf);

                    //set premise node as parent to inference node
                    //knowledge.Net.AddArc(pre.ID, inf.ID);

                    //set values to inference node
                    //Values v = new Values();
                    //v.InitInference(inf, knowledge.Net);
                    //add the knowledge to the actors knowledgebase.
                    aSubnet.Knowledges.Add(knowledge.Name, knowledge);

                }
            }
            foreach (ContextSubnet aSubnet in actorKB.Values)
            {
                foreach (Subnet subnet in aSubnet.Knowledges.Values)
                {
                    Node pre = subnet.Nodes[subnet.Name+'_'+Constants.PREMISE];
                    string ID_INF = pre.ID.Replace(Constants.PREMISE, Constants.INFERENCE);
                    //set parent nodes in the subnet
                    foreach (string parent in pre.Parents)
                    {
                        string[] ar = parent.Split('$');
                        Node nP;
                        if (ar.Length == 1)
                        {
                            nP = new Node(aSubnet.Nodes[ar[0]]);
                            aSubnet.Knowledges[nP.ID].Heirs.Add(new Relation(subnet.Name, aSubnet.Name));
                            aSubnet.Knowledges[subnet.Name].Parents.Add(new Relation(nP.ID, aSubnet.Name));
                        }
                        else
                        {
                            nP = new Node(k[ar[1]].Nodes[ar[0]]);
                            aSubnet.Knowledges[subnet.Name].Parents.Add(new Relation(nP.ID, ar[1]));
                            actorKB[ar[1]].Knowledges[ar[0]].Heirs.Add(new Relation(subnet.Name, aSubnet.Name));
                        }
                        aSubnet.Knowledges[subnet.Name].Net.AddVariable(nP.ID, nP);
                        aSubnet.Knowledges[subnet.Name].Nodes.Add(nP.ID, nP);

                        //add arc from parent to premise node
                        aSubnet.Knowledges[subnet.Name].Net.AddArc(nP.ID, pre.ID);
                        //add arc from parent to inf node
                        aSubnet.Knowledges[subnet.Name].Net.AddArc(nP.ID, ID_INF);
                        //set relations

                    }
                    //set premise node definition
                    if (pre.Values.Count > 0)
                    {
                        aSubnet.Knowledges[subnet.Name].Net.SetNodeDefinition(pre.ID, (double[])pre.Values.ToArray());
                        //set inf node definition
                        aSubnet.Knowledges[subnet.Name].Net.SetNodeDefinition(ID_INF, (double[])pre.Values.ToArray());
                    }
                    //if (subnet.Name == "S_occupation") aSubnet.Knowledges[subnet.Name].Net.WriteFile(string.Format("debug/KB_{0}.xdsl", subnet.Name));
                }
                
            }                
        }

        public void WriteKnowledgbase(Dictionary<string, ContextSubnet> k, string filename)
        {
            foreach (ContextSubnet aSubnet in k.Values)
            {
                foreach (Subnet subnet in aSubnet.Knowledges.Values)
                {
                    string f = string.Format("characters/{0}/{1}{2}.xdsl", this.actor, filename, subnet.Name);
                    subnet.Net.WriteFile(f);
                }
            }
        }

        
    }
}
