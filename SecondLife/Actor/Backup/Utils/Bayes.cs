using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Smile;
using DED.DPGE;
using log4net;
using log4net.Config;

namespace DED.Utils
{
    class Bayes : Network
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
                if (each < 2) SetOutcomeId(id, each, (string)n.States[each]);
                else AddOutcome(id, (string)n.States[each]);
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

        public void DeleteParentsArcs(Node n)
        {
            foreach (string parent in GetParentIds(n.ID))
            {
                DeleteArc(parent,n.ID);
            }
        }
    }
}
