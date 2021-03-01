using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Smile;
using DED.Utils;
using log4net;
using log4net.Config;

namespace DED.DPGE
{
    
    
    class Plot
    {
        Dictionary<string, Subnet> h = new Dictionary<string, Subnet>();
        private static readonly ILog log = LogManager.GetLogger(typeof(Plot));

        public Plot(string file)
        {
            XmlConfigurator.Configure();
            string s = System.IO.File.ReadAllText(file);

            string[] text = s.Split(';');
            //Console.WriteLine(text.Length);
            foreach (string line in text)
            {
                //Console.WriteLine(line);
                string[] l = line.Trim().Split(':');
                //Console.WriteLine(string.Format("line = {0}, l = {1}", line.Trim(), l[0].Trim()));
                switch (l[0].Trim())
                {
                    case "sub":
                        Subnet(l);
                        break;
                    case "var":
                        //Console.WriteLine("enter var");
                        Variable(l);
                        break;
                    case "states":
                        States(l);
                        break;
                    case "parents":
                        Parents(l);
                        break;
                    case "values":
                        Values(l);
                        break;
                    default:                        
                        break;
                }
            }
        }

        public void DrawNet()
        {
            Values values = new Values();
            foreach (Subnet subnet in h.Values)
            {
                foreach (Node n in subnet.Nodes.Values)
                {
                    subnet.Net.AddVariable(n);
                }
            }

            foreach (Subnet subnet in h.Values)
            {
                Dictionary<string,Node> l = new Dictionary<string,Node>();
                foreach (Node n in subnet.Nodes.Values)
                {
                    subnet.Net.AddParents(n, subnet.Name, h, l);
                    //Then set values for the node
                    values.SetValues(n, subnet.Net);

                    if (n.Values.Count == subnet.Net.TableSize(n))
                    {
                        subnet.Net.SetNodeDefinition(n.ID, (double[])n.Values.ToArray());
                    }
                    else
                    {
                        Console.WriteLine("Node: '{2}', Value count: '{0}', tablesize: '{1}'", n.Values.Count, subnet.Net.TableSize(n), n.ID);
                    }
                }
                foreach ( Node n in l.Values ){
                    subnet.Nodes.Add(n.ID, n);                     
                }                
            }  
            CalculatePlot();
        }

        public void CalculatePlot()
        {
            bool calc = true;
            
            while (calc)
            {
                calc = false;
                foreach (Subnet subnet in h.Values)
                {                    
                    //first check if parent is nonvalid
                    if (!subnet.Valid)
                    {
                        bool parentsValid = true;
                        foreach (Relation parent in subnet.Parents)
                        {
                            //first check if parent is nonvalid
                            if (!h[parent.submodel].Valid)
                            {
                                parentsValid = false;
                                calc = true;
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
                                double[] d = subnet.Net.GetNodeValue(heir.name);
                                h[heir.submodel].Nodes[heir.name].Values = new List<double>(d);
                                h[heir.submodel].Net.SetNodeDefinition(heir.name,d);
                            }
                            log.DebugFormat("Calculating subnet {0}", subnet.Name);
                        }                        
                    }
                }
            }               
        }

        public void WritePlot()
        {
            foreach (Subnet subnet in h.Values)
            {
                subnet.Net.WriteFile("plot/" + subnet.Name + ".xdsl");
            }
        }
        
        public void Subnet(string[] s) {
            string subname = s[1];
            //add a Plot subnet for this subnet to the main hash table
            h.Add(subname, new Subnet(subname));
        }

        public void Variable(string[] s) {
            string[] par = s[2].Split(',');
            Node n = new Node();
            n.ID = par[0];
            n.Name = par[1];
            n.X = Convert.ToInt32(par[2]);
            n.Y = Convert.ToInt32(par[3]);
            //Console.WriteLine( n.ToString());
            h[s[1]].Nodes.Add( n.ID, n );
        }

        public void States(string[] s) {
            string[] par = s[3].Split(',');
            List<string> al = new List<string>();
            foreach (string v in par) { al.Add(v); }
            //Store the state in the node in the 2D dict.
            h[s[2]].Nodes[s[1]].States = al;
        }

        public void Parents(string[] s)
        {
            string[] par = s[3].Split(',');
            List<string> al = new List<string>();
            foreach ( string v in par ) { al.Add(v); }
                //Store the state in the node in the 2D dict.
            h[s[2]].Nodes[s[1]].Parents = al;
        }

        public void Values(string[] s)
        {
            if (s.Length > 4)
            {
                string[] par = s[4].Split(',');
                List<double> al = new List<double>();
                foreach (string v in par) { al.Add(Convert.ToDouble(v.Replace('.',','))); }
                //Store the values in the node in the 2D dict.
                h[s[2]].Nodes[s[1]].Values = al;
            }
            //store the value flag in the node
            h[s[2]].Nodes[s[1]].ValueFlag = s[3];
        }
        
        public Dictionary<string, Subnet> dictPlot
        {
            get { return this.h; }
            set { this.h = value; }
        }
    }        
}
