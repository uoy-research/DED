using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.Utils.Reads;
using log4net;
using log4net.Config;
using System.Xml;
using DED.NPC.Actions;


namespace DED.Director
{
    public struct Trait {
        public Trait(string name, double value)
        {
            this.value = value;
            this.name = name;
        }
        /// <summary>
        /// The trait value
        /// </summary>
        double value;
        /// <summary>
        /// The trait value
        /// </summary>
        public double Value { get { return this.value; } }
        /// <summary>
        /// The trait name
        /// </summary>
        string name;
        /// <summary>
        /// The trait name
        /// </summary>
        public string Name { get { return this.name; } }
    }

    public struct Emotion {
        public Emotion(string name, double value)
        {
            this.value = value;
            this.name = name;
        }
        /// <summary>
        /// The emotion value
        /// </summary>
        double value;
        /// <summary>
        /// The emotion value
        /// </summary>
        public double Value { get { return this.value; } }
        /// <summary>
        /// The emotion name
        /// </summary>
        string name;
        /// <summary>
        /// The emotion name
        /// </summary>
        public string Name { get { return this.name; } }

        //TODO an emotion needs parents to determine the emotional levels.
    }

    [Serializable]
    public class Goal
    {
        public Goal(string name, int value, string variable, string state, string owner, string applies, Role role)
        {            
            this.name = name;
            this.rank = 0;
            this.isSatisfied = false;
            this.value = value;
            this.variable = variable;
            this.state = state;
            this.owner = owner;
            this.applies = applies;
            this.role = role;
        }

        public Goal(string name, int value, string variable, string state, string owner, string applies)
        {
            this.name = name;
            this.rank = 0;
            this.isSatisfied = false;
            this.value = value;
            this.variable = variable;
            this.state = state;
            this.owner = owner;
            this.applies = applies;
        }

        public Goal(string name, int value, string variable, string state)
        {
            this.name = name;
            this.rank = 0;
            this.isSatisfied = false;
            this.value = value;
            this.variable = variable;
            this.state = state;
            this.owner = Constants.CHARACTER;
            this.applies = Constants.SELF;
        } 

        public Goal(Goal g)
        {
            this.name = g.Name;
            this.rank = g.Rank;
            this.isSatisfied = g.IsSatisfied;
            this.value = g.Value;
            this.variable = g.Variable;
            this.state = g.State;
            this.owner = g.Owner;
            this.applies = g.Applies;
            this.role = g.Role;
        }
        
                
        /// <summary>
        /// The name of the goal
        /// </summary>
        string name;
        /// <summary>
        /// The name of the goal
        /// </summary>
        public string Name { get { return this.name; } set { this.name = value; } }

        string owner;
        public string Owner { get { return this.owner; } }

        Role role;
        public Role Role { get { return this.role; } }

        /// <summary>
        /// To hwo should the goal be applied to - eg. any, self
        /// </summary>
        string applies;
        /// <summary>
        /// To hwo should the goal be applied to - eg. any, self
        /// </summary>
        public string Applies { get { return this.applies; } set { this.applies = value; } }

        /// <summary>
        /// Which specific variable in the knowledgebase is influenced. 
        /// For example maxing S_motive
        /// </summary>
        string variable;
        /// <summary>
        /// Which specific variable in the knowledgebase is influenced. 
        /// For example maxing S_motive
        /// </summary>
        public string Variable { get { return this.variable; } set { this.variable = value; } }
        /// <summary>
        /// The state in the variable that should be affected.
        /// For example M_name:self means the state that bears the same name as the characters in question.
        /// S_murderer:0 is state 0 in the variable and * means all states.
        /// </summary>
        string state;
        /// <summary>
        /// The state in the variable that should be affected.
        /// For example M_name:self means the state that bears the same name as the characters in question.
        /// S_murderer:0 is state 0 in the variable and * means all states.
        /// </summary>
        public string State { get { return this.state; } set { this.state = value; } }
        
        /// <summary>
        /// Importance of the goal
        /// </summary>
        int rank;
        /// <summary>
        /// Importance of the goal
        /// </summary>
        public int Rank { get { return this.rank; } set { this.rank = value; } }
        /// <summary>
        /// Has the goal been satisfied
        /// </summary>
        bool isSatisfied;
        /// <summary>
        /// Has the goal been satisfied
        /// </summary>
        public bool IsSatisfied { get { return this.isSatisfied; } set { this.isSatisfied = value; } }
        /// <summary>
        /// The schemas that the goal belongs to.
        /// </summary>
        Dictionary<string, Schema> schemas = new Dictionary<string,Schema>();
        /// <summary>
        /// The schemas that the goal belongs to.
        /// </summary>
        public Dictionary<string, Schema> Schemas { get { return this.schemas; } set { this.schemas = value; } }
        int value;
        public int Value { get { return this.value; } set { this.value = value; } }
    }

    //class ActionGoal : Goal
    //{
    //    public ActionGoal(string name, string aim, int value, string variable, string state, string state)
    //        : base(name, aim, value, variable, state) {  }

    //}

    public class SchemaGoal : Goal
    {
        public SchemaGoal(string name, string owner, Role role, string applicable, string actor
            , string submodel, string variable, string state, string dramaGoal, int value)
            : base(name, value, variable, state, owner, applicable, role)
        {
            
            this.actor = actor;
            this.submodel = submodel;
            this.dramaGoal = dramaGoal;
        }

        public SchemaGoal(SchemaGoal g)
            : base(g)
        {            
            this.actor = g.Actor;
            this.submodel = g.Submodel;
            this.dramaGoal = g.DramaGoal;
        }
            
        /// <summary>
        /// The name of the actor 
        /// </summary>
        string actor;
        /// <summary>
        /// The name of the actor 
        /// </summary>
        public string Actor { get { return this.actor; } set { this.actor = value; } }
        /// <summary>
        /// Which submodel in the knowledge base does it belong to e.g. suspect, murder weapon
        /// </summary>
        string submodel;
        /// <summary>
        /// Which submodel in the knowledge base does it belong to e.g. suspect, murder weapon
        /// </summary>
        public string Submodel { get { return this.submodel; } }        

        string dramaGoal;
        public string DramaGoal { get { return this.dramaGoal; } }
    }

    public class Precondition
    {
        public Precondition(string name, string satisfier, bool precondition_satisfied)
        {
            this.name = name;
            this.satisfier = satisfier;
            this.isSatisfied = precondition_satisfied;
        }

        public Precondition(string name, bool precondition_satisfied)
        {
            this.name = name;
            this.isSatisfied = precondition_satisfied;
        }

        public Precondition(Precondition p)
        {
            this.name = p.Name;
            this.satisfier = p.Satisfier;
            this.isSatisfied = p.IsSatisfied;
        }

        /// <summary>
        /// The precondition Name
        /// </summary>
        string name;

        /// <summary>
        /// The precondition Name
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// The precondition Satisfier
        /// </summary>
        string satisfier;

        /// <summary>
        /// The precondition Satisfier
        /// </summary>
        public string Satisfier { get { return this.satisfier; } }

        /// <summary>
        /// Has the preconditon been satisfied
        /// </summary>
        bool isSatisfied;

        /// <summary>
        /// Has the preconditon been satisfied
        /// </summary>
        public bool IsSatisfied { get { return this.isSatisfied; } set { this.isSatisfied = value; } }
    }


    
    /// <summary>
    /// The role that an actor fills.
    /// </summary>
    public struct Role
    {
        public Role(string name, string schemaName, bool essential, string variable, string subnet)
        {
            this.traits = new Dictionary<string, Trait>();
            this.actions = new List<DEDAction>();
            this.essential = essential;
            this.name = name;
            this.maxNr = 0;
            this.minNr = 0;
            this.schemaName = schemaName;
            this.variable = variable;
            this.subnet = subnet;
        }
        
        /// <summary>
        /// The traits that this role represents
        /// </summary>
        Dictionary<string, Trait> traits;

        /// <summary>
        /// The traits that this role represents
        /// </summary>
        public Dictionary<string, Trait> Traits { get { return this.traits; } }

        /// <summary>
        /// Actions that go with the role
        /// </summary>
        List<DEDAction> actions;

        /// <summary>
        /// Actions that go with the role
        /// </summary>
        public List<DEDAction> Actions { get { return this.actions; } }

        /// <summary>
        /// Is the role essential
        /// </summary>
        bool essential;

        /// <summary>
        /// Is the role essential
        /// </summary>
        public bool isEssential { get { return this.essential; } set { this.isEssential = value; } }

        /// <summary>
        /// The name of the role
        /// </summary>
        string name;

        /// <summary>
        /// The name of the role
        /// </summary>
        public string Name { get { return this.name; } }

        string variable;
        public string Variable { get { return this.variable; } }
        string subnet;
        public string Subnet { get { return this.subnet; } }

        /// <summary>
        /// The name of the role
        /// </summary>
        string schemaName;

        /// <summary>
        /// The name of the role
        /// </summary>
        public string SchemaName { get { return this.schemaName; } }

        /// <summary>
        /// The maximum number of actors that can have this role given any one deployment of the schema.
        /// </summary>
        int maxNr;

        /// <summary>
        /// The maximum number of actors that can have this role given any one deployment of the schema.
        /// </summary>
        public int MaxNr { get { return this.maxNr; } set { this.maxNr = value; } }

        /// <summary>
        /// The minimum number of actors that need to have this role given any one deployment of the schema.
        /// </summary>
        int minNr;

        /// <summary>
        /// The minimum number of actors that need to have this role given any one deployment of the schema.
        /// </summary>
        public int MinNr { get { return this.minNr; } set { this.minNr = value; } }


    }

    /// <summary>
    /// An actor in a specific schema
    /// </summary>
    public struct SchemaActor
    {
        public SchemaActor(string name, string schemaName, Role role)
        {
            this.role = role;
            this.name = name;
            this.schemaName = schemaName;
        }

        /// <summary>
        /// The role the actor is playing
        /// </summary>
        Role role;

        /// <summary>
        /// The role the actor is playing
        /// </summary>
        public Role Role { get { return this.role; } }

        /// <summary>
        /// The actors name
        /// </summary>
        string name;

        /// <summary>
        /// The actors name
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// The schema name
        /// </summary>
        string schemaName;

        /// <summary>
        /// The schema name
        /// </summary>
        public string SchemaName { get { return this.schemaName; } }
    }

    public class Schema
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Schema));

        /// <summary>
        /// A dict of all the essential roles of the schema, idx is role.name
        /// </summary>
        Dictionary<string, Role> essentialRoles = new Dictionary<string, Role>();
        /// <summary>
        /// A dict of all the essential roles of the schema, idx is role.name
        /// </summary>
        public Dictionary<string, Role> EssentialRoles { get { return this.essentialRoles; } }

        /// <summary>
        /// All preconditions that need to be satisfied for this action to be applicable
        /// </summary>
        Dictionary<string, Precondition> preconditions = new Dictionary<string,Precondition>();

        /// <summary>
        /// All preconditions that need to be satisfied for this action to be applicable
        /// </summary>
        public Dictionary<string, Precondition> Preconditions { get { return this.preconditions; } }

        /// <summary>
        /// A dict of all possible roles that this schema offers, idx is role.name
        /// </summary>
        Dictionary<string, Role> possibleRoles = new Dictionary<string, Role>();
        /// <summary>
        /// A dict of all possible roles that this schema offers, idx is role.name
        /// </summary>
        public Dictionary<string, Role> PossibleRoles { get { return this.possibleRoles; } }

        /// <summary>
        /// A dict of all possible goals, idx is goal.name
        /// </summary>
        Dictionary<string, SchemaGoal> goals = new Dictionary<string, SchemaGoal>();
        /// <summary>
        /// A dict of all possible goals, idx is goal.name
        /// </summary>
        public Dictionary<string, SchemaGoal> Goals { get { return this.goals; } }

        /// <summary>
        /// The number of instances that a schema can be deployed at any given time.
        /// </summary>
        int instances;
        /// <summary>
        /// The number of instances that a schema can be deployed at any given time.
        /// </summary>
        public int Instances { get { return this.instances; } set { this.instances = value; } }

        /// <summary>
        /// The schema name
        /// </summary>
        string name;
        /// <summary>
        /// The schema name
        /// </summary>
        public string Name { get { return this.name; } set { this.name = value; } }
        
        /// <summary>
        /// A list that holds a list of the acts that thre schema is applicable for.
        /// </summary>
        List<int> acts = new List<int>();
        public List<int> Acts { get { return this.acts; } }

        List<Goal> SatisfiedGoals = new List<Goal>();

        /// <summary>
        /// Is the schema applicable to the act passed as int idx
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsAct(int value) {
            foreach (int i in this.acts) { if (i == value) return true; } return false; 
        }

        /// <summary>
        /// Marks whether this schema is currently deployed
        /// </summary>
        bool isDeployed;
        /// <summary>
        /// Marks whether this schema is currently deployed
        /// </summary>
        public bool IsDeployed { get { return this.isDeployed; } set { this.isDeployed = value; } }

        /// <summary>
        /// Marks whether this schema has become faulty and needs to be revoked
        /// </summary>
        bool isFaulty = false;
        /// <summary>
        /// Marks whether this schema has become faulty and needs to be revoked
        /// </summary>
        public bool IsFaulty { get { return this.isFaulty; } set { this.isFaulty = value; } }

        /// <summary>
        /// A dict of all schemas actors and which role they hold. Idx = actor.name
        /// </summary>
        Dictionary<string, SchemaActor> schemaActors = new Dictionary<string, SchemaActor>();
        /// <summary>
        /// A dict of all schemas actors and which role they hold. Idx = actor.name
        /// </summary>
        public Dictionary<string, SchemaActor> SchemaActors { get { return this.schemaActors; } }

        /// <summary>
        /// The value that the drama manager gives the schema
        /// </summary>
        double dramaValue;
        /// <summary>
        /// The value that the drama manager gives the schema
        /// </summary>
        public double DramaValue { get { return this.dramaValue; } set { this.dramaValue = value; } }

        int rank = 0;
        public int Rank { get { return this.rank; } set { this.rank = value; } }

        

        public Schema() { XmlConfigurator.Configure(); this.dramaValue = Constants.MAX;}

        public Schema(Schema schema) { 
            XmlConfigurator.Configure(); 
            this.dramaValue = Constants.MAX;
            this.acts = schema.Acts;
            this.dramaValue = schema.DramaValue;
            this.essentialRoles = schema.EssentialRoles;
            this.goals = schema.Goals;
            this.instances = schema.Instances;
            this.isDeployed = schema.IsDeployed;
            this.isFaulty = schema.IsFaulty;
            this.name = schema.Name;
            this.possibleRoles = schema.PossibleRoles;
            this.preconditions = schema.Preconditions;
            this.rank = schema.Rank;
            this.SatisfiedGoals = schema.SatisfiedGoals;
            this.schemaActors = schema.SchemaActors;
        }


        /// <summary>
        /// A function that initiates the schema by reading a textfile which describes the schema.
        /// </summary>
        /// <param name="filename"></param>
        public void Init(string filename) { ReadSchemas(filename);  }

        
        void ReadSchemas(string file)
        {
            ReadSchemas rs = new ReadSchemas(this);
            rs.ReadFile(file);           
        }   

     
        /// <summary>
        /// Have all goals of the schema been satisfied
        /// </summary>
        /// <returns></returns>
        public bool IsGoalsSatisfied() { if (this.goals.Count == 0) return true; return false; }

        /// <summary>
        /// Mark a single goal as satified
        /// </summary>
        /// <returns></returns>
        public void SatisfyGoal(Goal goal) {
            if (this.goals.ContainsKey(goal.Name))            
            {
                lock (this.SatisfiedGoals) { this.SatisfiedGoals.Add(this.goals[goal.Name]); }
                this.goals.Remove(goal.Name);
            }
        }

        /// <summary>
        /// Returns true if all preconditions are satisfied, else false.
        /// </summary>
        public bool IsPreconditionsSatisfied { get { foreach (Precondition p in this.preconditions.Values) if (!p.IsSatisfied) return false; return true; } }

        /// <summary>
        /// Marks a given precondition as satisfied.
        /// </summary>
        /// <param name="value"></param>
        public bool SatisfyPrecondition(string value, string role)
        {
            if (!this.preconditions.ContainsKey(value)) return false;
            if (role != this.preconditions[value].Satisfier && this.preconditions[value].Satisfier != null) return false;
            this.preconditions[value].IsSatisfied = true;
            return true;
        }

        internal bool UnSatisfyPrecondition(string value, string role)
        {
            if (!this.preconditions.ContainsKey(value)) return false;
            //if (role != this.preconditions[value].Satisfier) return false;
            this.preconditions[value].IsSatisfied = false;
            return true;
        }
    }
}
