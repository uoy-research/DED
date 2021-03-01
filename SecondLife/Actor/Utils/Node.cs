using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;

namespace DED.Utils
{
    public class Subnet
    {
        public Subnet() { }
        
        public Subnet(string name)
        {
            this.name = name;
            this.nodes = new Dictionary<string, Node>();
            this.net = new Bayes(this.name);
            this.heirs = new List<Relation>();
            this.parents = new List<Relation>();
            this.valid = false;
        }

        

        // Nodes 
        protected Dictionary<string, Node> nodes;
        public Dictionary<string, Node> Nodes { get { return this.nodes; } }
        // Name of the subnet
        protected string name;
        public string Name { get { return this.name; } }
        // Bayesian net of the subnet.
        protected Bayes net;
        public Bayes Net { get { return this.net; } }
        // The nodes in other subnets that have parents in this subnet.
        protected List<Relation> heirs;
        public List<Relation> Heirs { get { return this.heirs; } }
        // The nodes in other subnets that have children in this subnet.
        protected List<Relation> parents;
        public List<Relation> Parents { get { return this.parents; } }
        //has this subnet been calculated
        protected bool valid;
        public bool IsValid { get { return this.valid; } set { this.valid = value; } }
    }

    public class Belief
    {
        public Belief() { }

        public Belief(string v, string g)
        {
            this.name = v + "_" + g;
            this.variable = v;
            this.goal = g;
            this.nodes = new Dictionary<string, Node>();
            this.net = new Bayes(this.name);            
            this.valid = false;
        }
        protected string name;
        public string Name { get { return this.name; } set { this.name = value; } }
        protected Dictionary<string, Node> nodes;
        public Dictionary<string, Node> Nodes { get { return this.nodes; } }
        protected string variable;
        public string Variable { get { return this.variable; } set { this.variable = value; } }
        protected string goal;
        public string Goal { get { return this.goal; } }        
        
        // Bayesian net of the subnet.
        protected Bayes net;
        public Bayes Net { get { return this.net; } }
  
        //has this subnet been calculated
        protected bool valid;
        public bool IsValid { get { return this.valid; } set { this.valid = value; } }

        protected bool empty;
        public bool IsEmpty { get { return this.empty; } set { this.empty = value; } }


        internal void AddNet(Bayes net, string name)
        {
            string infID = name + '_' + Constants.INFERENCE;
            //if (this.nodes.ContainsKey(name)) return;
            //add the INF node
            Node INF = new Node();
            INF.Name = name;
            INF.ID = name;
            INF.Values = new List<double>( net.GetNodeDefinition(infID) );
            INF.States = new List<string>( net.GetOutcomeIds(infID) );

            if (!this.nodes.ContainsKey(name))
            {
                this.net.AddVariable(INF);
                this.nodes.Add(INF.ID, INF);
            }
            
            //Next add the parents
            string[] parents = net.GetParentIds(infID);
            foreach (string parent in parents)
            {
                if (this.nodes.ContainsKey(parent)) continue;
                Node P = new Node();
                P.Name = parent;
                P.ID = parent;
                P.Values = new List<double>(net.GetNodeDefinition(parent));
                P.States = new List<string>(net.GetOutcomeIds(parent));

                this.net.AddVariable(P);
                this.nodes.Add(P.ID, P);
                this.net.SetNodeDefinition(P.ID, net.GetNodeDefinition(parent));
            }

            INF.Parents = new List<string>(parents);
            this.net.AddParents(INF);

            this.net.SetNodeDefinition(INF.ID, INF.Values.ToArray());      

        }
    }

    public class Variable
    {
        string name;
        string subnet;
        int state;
        string state_label;
        bool singular;
        bool setParent;
        ArrayList parents = new ArrayList();

        
        public Variable(Variable v)
        {
            this.name = v.Name;
            this.subnet = v.Subnet;
            this.state = v.State;
            this.singular = v.Singular;
            this.state_label = v.State_label;
            this.setParent = v.SetParent;
        }

        public Variable(string name, string subnet, int state)
        {
            this.name = name;
            this.subnet = subnet;
            this.state = state;
        }

        public Variable(string name, string subnet, int state, bool singular, bool setParent)
        {
            this.name = name;
            this.subnet = subnet;
            this.state = state;
            this.singular = singular;
            this.setParent = setParent;
        }

        public Variable(string name, string subnet, int state, bool singular, bool setParent, ArrayList parents)
        {
            this.name = name;
            this.subnet = subnet;
            this.state = state;
            this.singular = singular;
            this.setParent = setParent;
            foreach (string p in parents) { this.parents.Add(p); }
        }

        public bool Singular { get { return this.singular; } }
        public bool SetParent { get { return this.setParent; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public string Subnet { get { return this.subnet; } }
        public int State { get { return this.state; } set { this.state = value; } }
        public string State_label { get { return this.state_label; } set { this.state_label = value; } }
        public string getHashKey() { return this.name + this.subnet; }
        public ArrayList Parents { get { return this.parents; } }
    }

    public struct Relation
    {
        public string name, submodel;
        public Relation(string name, string submodel)
        {
            this.name = name;
            this.submodel = submodel;
        }
    }
    
    [Serializable]
    public class Strategy
    {
        string submodel;
        string variable;
        int state;
        double value;
        int total;
        bool isDenial;

        public Strategy(){}
        public Strategy(string submodel, string variable, double value, int state, bool isDenial)
        {
            this.variable = variable;
            this.submodel = submodel;
            this.value = value;
            this.state = state;
            this.isDenial = isDenial;
        }

        public Strategy(Variable v)
        {
            this.variable = v.Name;
            this.submodel = v.Subnet;
            this.state = Convert.ToInt32(v.State);
        }

        public Strategy(string submodel, string variable, double value, int state, int total, bool isDenial)
        {
            this.variable = variable;
            this.submodel = submodel;
            this.value = value;
            this.state = state;
            this.total = total;
            this.isDenial = isDenial;
        }

        public bool IsDenial { get { return this.isDenial; } set { this.isDenial = value; } }
        public double Value { get { return this.value; } set { this.value = value; } }
        public string Variable { get { return this.variable; } set { this.variable = value; } }
        public string Submodel { get { return this.submodel; } set { this.submodel = value; } }
        public int State { get { return this.state; } set { this.state = value; } }
        public int Total { get { return this.total; } set { this.total = value; } }
    }

    public class Node
    {
        List<string> parents = new List<string>();
        List<double> values = new List<double>();
        List<string> states = new List<string>();
        string name, id, valueFlag;
        int x, y;
        bool isParent;

        public Node() { }

        public Node(Node n) {
            this.parents = new List<string>( n.Parents );
            this.values  = new List<double>( n.Values  );
            this.states  = new List<string>( n.States  );
            this.name = n.Name;
            this.id = n.ID;
            this.valueFlag = n.ValueFlag;
            this.x = n.X;
            this.y = n.Y;
            this.isParent = n.IsParent;
        }

        public bool IsParent        { get { return this.isParent;   } set { this.isParent = value;  } }

        public List<string> Parents { get { return this.parents;    } set { this.parents = value;   } }

        public List<double> Values  { get { return this.values;     } set { this.values = value;    } }

        public List<string> States
        {
            get { return this.states;   }
            set { this.states = value;  }
        }

        public string Name
        {
            get { return this.name;     }
            set { this.name = value;    }
        }

        public string ValueFlag
        {
            get { return this.valueFlag;    }
            set { this.valueFlag = value;   }
        }

        public string ID
        {
            get { return this.id;   }
            set { this.id = value;  }
        }

        public int X
        {
            get { return this.x;    }
            set { this.x = value;   }
        }

        public int Y
        {
            get { return this.y;    }
            set { this.y = value;   }
        }

        override public string ToString()
        {
            return string.Format("ID = '{0}', Name = '{1}'",id,name);
        }
    }
}
