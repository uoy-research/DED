using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;

namespace DED.Utils
{
    class Subnet
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
        public bool Valid { get { return this.valid; } set { this.valid = value; } }
    }

    struct Relation
    {
        public string name, submodel;
        public Relation(string name, string submodel)
        {
            this.name = name;
            this.submodel = submodel;
        }
    }

    struct Strategy
    {
        string submodel;
        string variable;
        int state;
        double value;

        public Strategy(string submodel, string variable, double value, int state)
        {
            this.variable = variable;
            this.submodel = submodel;
            this.value = value;
            this.state = state;
        }

        public double Value { get { return this.value; } }
        public string Variable { get { return this.variable; } }
        public string Submodel { get { return this.submodel; } }
        public int State { get { return this.state; } }
    }

    class Node
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
