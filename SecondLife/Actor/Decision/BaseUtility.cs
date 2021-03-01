using System;
using System.Collections.Generic;
using System.Text;
using DED.NPC;
using DED.Utils;
using DED.Director;
using Smile;
using DED.DPGE;
using log4net;
using log4net.Config;



namespace DED.Decision
{    
    public struct Choice
    {
        public Choice(string name, string position, string subnet, int state, double value)
        {
            this.name = name;
            this.position = position;
            this.state = state;
            this.value = value;
            this.weight = 0;
            this.subnet = subnet;
        }

        string name;
        public string Name      { get { return this.name;       } }

        string subnet;
        public string Subnet { get { return this.subnet; } }


        string position;
        public string Position  { get { return this.position;   } }

        int state;
        public int State        { get { return this.state;      } }

        double value;
        public double Value     { get { return this.value;      } set { this.value = value;     } }

        double weight;
        public double Weight    { get { return this.weight;     } set { this.weight = value;    } }
    }

    public struct Optimal
    {
        public Optimal(string key, double value, List<Choice> choices)
        {
            this.key = key;
            this.value = value;
            this.choices = choices;
        }

        string key;
        public string Key               { get { return this.key;        } }

        double value;
        public double Value             { get { return this.value;      } }

        List<Choice> choices;
        public List<Choice> Choices     { get { return this.choices;    } }
    }

    public abstract class Base_utility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Base_utility));

        protected Optimal optimal = new Optimal();
        protected Dictionary<string, Optimal> results = new Dictionary<string,Optimal>();
        protected Dictionary<string, Optimal> Results { get { return this.results; } }

        protected Dictionary<string, Optimal> reactions = new Dictionary<string, Optimal>();
        protected Dictionary<string, Optimal> Reactions { get { return this.reactions; } }

        protected Dictionary<string, List<Choice>> innerResults = new Dictionary<string, List<Choice>>();
        Bayes net = new Bayes();

        public Base_utility()
        {
            XmlConfigurator.Configure();            
        }

        public abstract List<Strategy> GetStrategies(Goal goal);              
      
    }
}
