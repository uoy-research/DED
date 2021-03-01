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

    class Dependancy
    {
        public Dependancy( int n, string variable)
        { this.Number = n; this.Dependant = variable; }

        public int Number { get; set; }
        public string Dependant { get; set; }
    }
    
    
    class PlotNetwork
    {
        Dictionary<string, Subnet> h = new Dictionary<string, Subnet>();
        Dictionary<string, Dependancy> dependants = new Dictionary<string, Dependancy>();
        private static readonly ILog log = LogManager.GetLogger(typeof(PlotNetwork));

        public PlotNetwork(string file)
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
                    case "loop":
                        Loop(l);
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
                subnet.Net.WriteFile("plot\\"+subnet.Name+".xdsl");
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
                    if (!subnet.IsValid)
                    {
                        bool parentsValid = true;
                        foreach (Relation parent in subnet.Parents)
                        {
                            //first check if parent is nonvalid
                            if (!h[parent.submodel].IsValid)
                            {
                                parentsValid = false;
                                calc = true;
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

        private void Loop(string[] l)
        {
            Dependancy d = new Dependancy(Convert.ToInt32(l[2]),"");
            this.dependants.Add(l[1],d);
        }
        
        public void Subnet(string[] s) {
            string subname = s[1];
            //add a Plot subnet for this subnet to the main hash table
            if (!this.dependants.ContainsKey(subname)) { this.h.Add(subname, new Subnet(subname)); return; }

            //else do multiple instances of subnets
            for (int i = 0; i < this.dependants[subname].Number; ++i)
            {
                string subname_new = subname + "_" + i;
                this.h.Add(subname_new, new Subnet(subname_new));
            }
        }

        public void Variable(string[] s) {
            string subname = s[1];

            string[] par = s[2].Split(',');
            Node n = new Node();
            n.ID = par[0];
            n.Name = par[1];
            n.X = Convert.ToInt32(par[2]);
            n.Y = Convert.ToInt32(par[3]);            

            if (!this.dependants.ContainsKey(subname)) { this.h[subname].Nodes.Add(n.ID, n); return; }

            //else do multiple instances of subnets
            for (int i = 0; i < this.dependants[subname].Number; ++i)
            {
                Node n2 = new Node(n);
                string subname_new = subname + "_" + i;
                this.h[subname_new].Nodes.Add(n2.ID, n2);
            }            
        }

        public void States(string[] s) {
            string subname = s[2];

            string[] par = s[3].Split(',');
            List<string> al = new List<string>();
            foreach (string v in par) { al.Add(v); }

            //Store the state in the node in the 2D dict.            

            if (!this.dependants.ContainsKey(subname)) { this.h[subname].Nodes[s[1]].States = al; return; }

            //else do multiple instances of subnets
            for (int i = 0; i < this.dependants[subname].Number; ++i)
            {
                List<string> al2 = new List<string>();
                foreach (string v in par) { al2.Add(v); }
                string subname_new = subname + "_" + i;
                this.h[subname_new].Nodes[s[1]].States = al2;
            }   
        }

        public void Parents(string[] s)
        {
            string subname = s[2];

            string[] par = s[3].Split(',');
            List<string> al = new List<string>();
            foreach ( string v in par ) { al.Add(v); }
                
            //Store the state in the node in the 2D dict.           

            if (!this.dependants.ContainsKey(subname)) { this.h[subname].Nodes[s[1]].Parents = al; return; }

            //else do multiple instances of subnets
            for (int i = 0; i < this.dependants[subname].Number; ++i)
            {
                List<string> al2 = new List<string>();
                foreach (string v in par) { al2.Add(v); }
                string subname_new = subname + "_" + i;
                this.h[subname_new].Nodes[s[1]].Parents = al2;
            } 
        }

        public void Values(string[] s)
        {
            string subname = s[2];
            
            if (!this.dependants.ContainsKey(subname)) { 
                if (s.Length > 4)
                {
                    string[] par = s[4].Split(',');
                    List<double> al = new List<double>();
                    if (Convert.ToDouble("0.4")>1) foreach (string v in par) { al.Add(Convert.ToDouble(v.Replace('.', ','))); }
                    else foreach (string v in par) { al.Add(Convert.ToDouble(v)); }
                    //Store the values in the node in the 2D dict.
                    this.h[subname].Nodes[s[1]].Values = al;
                }
                //store the value flag in the node
                this.h[subname].Nodes[s[1]].ValueFlag = s[3];

            return;
            }
            //else do multiple instances of subnets
            for (int i = 0; i < this.dependants[subname].Number; ++i)
            {
                
                string subname_new = subname + "_" + i;
                if (s.Length > 4)
                {
                    string[] par = s[4].Split(',');
                    List<double> al = new List<double>();
                    //foreach (string v in par) { al.Add(Convert.ToDouble(v.Replace('.',','))); }
                    foreach (string v in par) { al.Add(Convert.ToDouble(v)); }
                    //Store the values in the node in the 2D dict.
                    this.h[subname_new].Nodes[s[1]].Values = al;
                }
                //store the value flag in the node
                this.h[subname_new].Nodes[s[1]].ValueFlag = s[3];
            }
        }
        
        public Dictionary<string, Subnet> dictPlot
        {
            get { return this.h; }
            set { this.h = value; }
        }
    }        
}
