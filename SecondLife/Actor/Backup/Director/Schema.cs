using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using log4net;
using log4net.Config;


namespace DED.Director
{
    struct Trait {
        public Trait(string name, double value)
        {
            this.value = value;
            this.name = name;
        }
        double value;
        public double Value { get { return this.value; } }
        string name;
        public string Name { get { return this.name; } }
    }

    struct Emotion {
        public Emotion(string name, double value)
        {
            this.value = value;
            this.name = name;
        }
        double value;
        public double Value { get { return this.value; } }
        string name;
        public string Name { get { return this.name; } }

        //an emotion needs parents to determine the emotional levels.
    }

    class Goal
    {
        public Goal(string name, string owner, string role, string applicable, string actor, string aim, string submodel, string variable, string state)
        {            
            this.name = name;
            this.owner = owner;
            this.role = role;
            this.applicable = applicable;
            this.actor = actor;
            this.aim = aim;
            this.submodel = submodel;
            this.variable = variable;
            this.state = state;
            this.rank = 0;
            this.isSatisfied = false;
        }
                
        string name; //whole string
        public string Name { get { return this.name; } }
        string owner; //character or actor
        public string Owner { get { return this.owner; } }
        string role; //e.g. suspect
        public string Role { get { return this.role; } }
        string applicable; //to hwo should the goal be applied to - eg. any, self
        public string Applicable { get { return this.applicable; } }
        string actor; //the actor assigned the role.
        public string Actor { get { return this.actor; } set { this.actor = value; } }
        string aim; //for instance max the value 
        public string Aim { get { return this.aim; } }
        string submodel; //suspect
        public string Submodel { get { return this.submodel; } }
        string variable; //S_motive
        public string Variable { get { return this.variable; } }
        string state; //0 or self
        public string State { get { return this.state; } }
        int rank; //importance of the goal
        public int Rank { get { return this.rank; } set { this.rank = value; } }
        bool isSatisfied; //has the goal been satisfied
        public bool IsSatisfied { get { return this.isSatisfied; } set { this.isSatisfied = value; } }
    }

    class Precondition
    {
        public Precondition (string name) {
            this.name = name;
            this.isSatisfied = false;
        }

        string name;
        public string Name { get { return this.name; } }
        bool isSatisfied;
        public bool IsSatisfied { get { return this.isSatisfied; } set { this.isSatisfied = value; } }
    }

    class Action
    {
        public Action(Action a)
        {
            this.emotions = a.Emotions;
            this.traits = new List<Trait>();
            this.name = a.Name;
            this.essential = a.Essential;
            this.preconditions = new List<Precondition>();
            foreach (Precondition p in a.preconditions) 
            { this.preconditions.Add( new Precondition( p.Name ) ); }
            this.nrExecutions = a.NrExecutions;
        }

        public Action(string name, bool essential, List<Precondition> precon, int nrExecutions)
        {
            this.emotions = new List<Emotion>();
            this.traits = new List<Trait>();
            this.name = name;
            this.essential = essential;
            this.preconditions = precon;
            this.nrExecutions = nrExecutions;
        }

        List<Emotion> emotions;
        public List<Emotion> Emotions { get { return this.emotions; } }
        List<Precondition> preconditions;
        public List<Precondition> Preconditions { get { return this.preconditions; } }
        List<Trait> traits;
        public List<Trait> Traits { get { return this.traits; } }
        string name;
        public string Name { get { return this.name; } }
        bool essential;
        public bool Essential { get { return this.essential; } }
        public bool IsPreconditionsSatisfied
        {
            get
            {
                foreach (Precondition p in this.preconditions)
                { if (!p.IsSatisfied) return false; } return true;
            }
        }
        public void SatisfyPrecondition(string value) {
            for (int i = 0; i < this.preconditions.Count; ++i )
            {
                if (this.preconditions[i].Name == value)
                {
                    Precondition p = this.preconditions[i];
                    p.IsSatisfied = true;
                    return;
                }
            }            
        }
        int nrExecutions;
        public int NrExecutions { get { return this.nrExecutions; } }
        public void ActionExecuted() { this.nrExecutions -= 1; }
    }

    struct Role
    {
        public Role(string name, bool essential)
        {
            this.traits = new List<Trait>();
            this.actions = new List<Action>();
            this.essential = essential;
            this.name = name;
            this.maxNr = 0;
        }
        List<Trait> traits;
        public List<Trait> Traits { get { return this.traits; } }
        List<Action> actions;
        public List<Action> Actions { get { return this.actions; } }
        bool essential;
        public bool isEssential { get { return this.essential; } }
        string name;
        public string Name { get { return this.name; } }
        int maxNr;
        public int MaxNr { get { return this.maxNr; } set { this.maxNr = value; } }
    }     


    class Schema
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Schema));

        Dictionary<string, Role> roles = new Dictionary<string, Role>();
        List<Goal> goals = new List<Goal>();
        Dictionary<string, Goal> currentGoals = new Dictionary<string, Goal>();
        Dictionary<string, Role> possibleRoles = new Dictionary<string, Role>();
        public Dictionary<string, Role> Role { get { return this.roles; } }
        public Dictionary<string, Role> PossibleRoles { get { return this.possibleRoles; } }
        public List<Goal> Goals { get { return this.goals; } }
        string name;
        int instances;
        public string Name { get { return this.name; } }
        List<int> acts = new List<int>();
        public bool IsAct(int value) { return this.acts.Contains(value); }
        bool isDeployed;
        public bool IsDeployed { get { return this.isDeployed; } set { this.isDeployed = value; } }

        public Schema() { XmlConfigurator.Configure(); }
        public void Init(string filename) {
            this.name = filename;
            //read in schema
            string s = System.IO.File.ReadAllText(filename);
            foreach (string str in s.Split(';'))
            {
                string[] l = str.Trim().Split(':');
                switch (l[0].Trim())
                {
                    case "role":
                        SetRoles(l);
                        break;

                    case "goal":
                        SetGoals(l, str.Trim());
                        break;

                    case "action":
                        SetActions(l);
                        break;

                    case "act":
                        SetAct(l);
                        break;

                    case "instances":
                        SetInstances(l);
                        break;
                    
                    default:
                        break;
                }
            }
        }

        private void SetInstances(string[] l)
        {
            try
            {
                this.instances = Convert.ToInt32(l[1]);
            }
            catch (Exception e)
            {
                log.ErrorFormat("can not convert '{0}' to int setting instances to 1 \n Error reads :'{1}'\n{2}", l[1], e.Message, e.StackTrace);
                this.instances = 1;
            }
        }

        private void SetAct(string[] l)
        {
            string[] aActs = l[1].Trim().Split(',');

            foreach ( string value in aActs )
            {
                this.acts.Add( Convert.ToInt32( value ) );
            }
        }

        private void SetActions(string[] l)
        {
            List<Precondition> precon = new List<Precondition>();
            if (!(l.Length < 6) )
            {
                if (!(l[5] == ""))
                {
                    string[] aPreconditions = l[5].Trim().Split(',');

                    foreach (string name in aPreconditions)
                    {
                        precon.Add(new Precondition(name));
                    }
                }
            }
            Action action = new Action(l[2].Trim(), Boolean.Parse(l[4].Trim()), precon, Convert.ToInt32(l[3].Trim()));
            this.roles[l[1].Trim()].Actions.Add(action);
        }

        private void SetGoals( string[] l, string name )
        {
            Goal goal = new Goal( name, l[1], l[2], l[3], null, l[4], l[5], l[6], l[7] );
            goals.Add(goal);
        }

        private void SetRoles(string[] l)
        {
            Role r = new Role(l[1],true);
            if (l[2] == "+" || l[2] == "*") r.MaxNr = Constants.MAX;
            else r.MaxNr = Convert.ToInt32(l[2]);
            this.roles.Add(r.Name, r);
        }
    }
}
