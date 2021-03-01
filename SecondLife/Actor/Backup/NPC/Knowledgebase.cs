using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.DPGE;
using log4net;
using log4net.Config;
using System.Threading;


namespace DED.NPC
{
    class ActorSubnet : Subnet
    {
        public ActorSubnet(Subnet subnet)
        {
            this.name = subnet.Name;
            this.nodes = new Dictionary<string, Node>(subnet.Nodes);
            this.net = new Bayes(this.name);
            this.heirs = new List<Relation>(subnet.Heirs);
            this.parents = new List<Relation>(subnet.Parents);
            this.valid = false;
            this.knowledges = new Dictionary<string, Subnet>();
            //if ( this.valid ) net.UpdateBeliefs();
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

    class Knowledgebase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Knowledgebase));
        
        string actor;
        List<Actor> lOpponents = new List<Actor>();
        //subnet
        Dictionary<string, ActorSubnet> kCharacter = new Dictionary<string, ActorSubnet>();
        //opponent, subnet
        Dictionary<string, Dictionary<string, ActorSubnet>> kOpponents = new Dictionary<string, Dictionary<string, ActorSubnet>>();
        //opponent, opponent, subnet
        Dictionary<string, Dictionary<string, Dictionary<string, ActorSubnet>>> kOpponentsOpponents = new Dictionary<string, Dictionary<string, Dictionary<string, ActorSubnet>>>();

        public string Actor { get { return this.actor; } }
        public Dictionary<string, ActorSubnet> Character { get { return this.kCharacter; } }
        public Dictionary<string, Dictionary<string, ActorSubnet>> Opponents { get { return this.kOpponents; } }
        public Dictionary<string, Dictionary<string, Dictionary<string, ActorSubnet>>> OpponentsOpponents { get { return this.kOpponentsOpponents; } }

        public Knowledgebase(string actor)
        {
            XmlConfigurator.Configure();
            log.Info("Enter Knowledge base");
            this.actor = actor;
        }

        public void CreateKnowledgeBase(Dictionary<string, Subnet> k, List<Actor> lOpponents)
        {
            int t = 0;
            this.lOpponents = lOpponents;
            //loop through the subnets
            foreach ( Subnet plotSubnet in k.Values )
            {
                Thread.Sleep(t); 
                kCharacter.Add(plotSubnet.Name, new ActorSubnet(plotSubnet));                

                //loop through all the opponents and 
                foreach (Actor opponent in lOpponents) {
                    Thread.Sleep(t); 
                    if (opponent.Name == actor) continue;

                    //initiate the dicts
                    if ( !kOpponents.ContainsKey(opponent.Name) ){ 
                        kOpponents.Add(opponent.Name, new Dictionary<string, ActorSubnet>());
                        kOpponentsOpponents.Add(opponent.Name, new Dictionary<string, Dictionary<string, ActorSubnet>>());
                    }

                    //set the opponent subnet
                    kOpponents[opponent.Name].Add(plotSubnet.Name, new ActorSubnet(plotSubnet));


                    //loop through the opponents opponents
                    foreach (Actor opponentOpponent in lOpponents)
                    {
                        Thread.Sleep(t); 
                        if (opponent.Name == opponentOpponent.Name) continue;

                        //initiate the dict
                        if (!kOpponentsOpponents[opponent.Name].ContainsKey(opponentOpponent.Name))
                        {
                            kOpponentsOpponents[opponent.Name].Add(opponentOpponent.Name, new Dictionary<string, ActorSubnet>());
                        }
                        //set the opponent opponents subnet                        
                        kOpponentsOpponents[opponent.Name][opponentOpponent.Name].Add(plotSubnet.Name, new ActorSubnet(plotSubnet));
                    }
                }          
            }            

            //loop through the charcter subnets
            
            DrawNet( kCharacter, k );
            Calculate(kCharacter);
            //WriteKnowledgbase(kCharacter, "caracter_"+ this.actor + '_');
            //loop through all the opponents and 
            foreach ( Actor opponent in lOpponents )
            {
                Thread.Sleep(t); 
                if (opponent.Name == actor) continue;
                DrawNet(kOpponents[opponent.Name], k);
                Calculate(kOpponents[opponent.Name]);
                //WriteKnowledgbase(kOpponents[opponent], opponent + '_');
                //loop through the opponents opponents
                foreach ( Actor opponentopponent in lOpponents )
                {
                    Thread.Sleep(t); 
                    if ( opponent.Name == opponentopponent.Name ) continue;
                    DrawNet(kOpponentsOpponents[opponent.Name][opponentopponent.Name], k);
                    Calculate(kOpponentsOpponents[opponent.Name][opponentopponent.Name]);
                    //WriteKnowledgbase(kOpponentsOpponents[opponent][opponentopponent], opponent + '_' + opponentopponent + '_');
                }
            }        
        }

        public void Calculate()
        {
            log.InfoFormat("Enter Calculate");
            Calculate(kCharacter);
            foreach (Actor opponent in this.lOpponents)
            {
                if (opponent.Name == actor) continue;
                Calculate(kOpponents[opponent.Name]);
                foreach (Actor opponentopponent in this.lOpponents)
                {
                    if (opponent.Name == opponentopponent.Name) continue;
                    Calculate(kOpponentsOpponents[opponent.Name][opponentopponent.Name]);
                }
            }
            log.InfoFormat("Exit Calculate");
        }

        private void Calculate(Dictionary<string, ActorSubnet> k)
        {
            List<Subnet> nonValidSubnets = new List<Subnet>();
            Dictionary<string, Subnet> calculateSubnets = new Dictionary<string, Subnet>();
            //walk through each subnet and foreach nonvalid subnet 
            //mark all its chilren non valid and add them to a list
            //then do the same for the list untill no more nodes are on the list.
            foreach (ActorSubnet aSubnet in k.Values)
            {
                if (aSubnet.Valid) continue;
                foreach (Subnet subnet in aSubnet.Knowledges.Values)
                {
                    if (subnet.Valid) continue;
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
                        if ( child.Valid )
                        {             
                            if (!calculateSubnets.ContainsKey(childSubnet.name))
                            {
                                //then set child as nonvalid
                                subnet.Valid = false;
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
                    if ( !subnet.Valid )
                    {
                        bool parentsValid = true;
                        foreach ( Relation parent in subnet.Parents )
                        {
                            //first check if parent is nonvalid
                            if (!k[parent.submodel].Knowledges[parent.name].Valid)
                            {
                                parentsValid = false;
                                newCalculates.Add(subnet.Name,subnet);
                                break;
                            }
                        }
                        if (parentsValid)
                        {
                            subnet.Valid = true;
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

        void DrawNet(Dictionary<string, ActorSubnet> actorKB, Dictionary<string, Subnet> k)
        {
            
            foreach (ActorSubnet aSubnet in actorKB.Values)
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
                    Node inf = new Node();
                    inf.ID = n.ID + '_' + Constants.INFERENCE;
                    inf.Name = n.ID + ' ' + Constants.INFERENCE;
                    inf.Parents.Add(pre.ID);
                    inf.States = n.States;


                    //add inf and premise to the Knowledge.net
                    knowledge.Net.AddVariable(pre.ID, pre);
                    knowledge.Net.AddVariable(inf.ID, inf);

                    //add inf and prem to the Knowledge.dict
                    knowledge.Nodes.Add(pre.ID, pre);
                    knowledge.Nodes.Add(inf.ID, inf);

                    //set premise node as parent to inference node
                    knowledge.Net.AddArc(pre.ID, inf.ID);

                    //set values to inference node
                    Values v = new Values();
                    v.InitInference(inf, knowledge.Net);
                    //add the knowledge to the actors knowledgebase.
                    aSubnet.Knowledges.Add(knowledge.Name, knowledge);

                }
            }
            foreach (ActorSubnet aSubnet in actorKB.Values)
            {
                foreach (Subnet subnet in aSubnet.Knowledges.Values)
                {
                    Node pre = subnet.Nodes[subnet.Name+'_'+Constants.PREMISE];
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
                        //set relations

                    }
                    //set premise node definition
                    aSubnet.Knowledges[subnet.Name].Net.SetNodeDefinition(pre.ID, (double[])pre.Values.ToArray());
                }
            }                
        }

        public void WriteKnowledgbase(Dictionary<string, ActorSubnet> k, string filename)
        {
            foreach (ActorSubnet aSubnet in k.Values)
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
