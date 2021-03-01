using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using DED.Director;
using DED.Utils;
using DED.Utils.Reads;



namespace DED.NPC.Actions
{
    public class DEDAction
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(DEDAction));

        public DEDAction(int id, DEDAction a)
        {
            XmlConfigurator.Configure();
            //this.emotions = a.Emotions;
            //this.traits = new List<Trait>();

            this.id = id; 
            this.name = a.Name;
            this.essential = a.Essential;
            this.preconditions = new Dictionary<string, Precondition>();
            if (a.Preconditions != null)
                foreach (Precondition p in a.Preconditions.Values) 
                { this.preconditions.Add( p.Name, new Precondition( p ) ); }
            this.nrExecutions = a.NrExecutions;
            this.goals = new Dictionary<string, Goal>(a.Goals);
            this.schemas = new Dictionary<string, Schema>(a.Schemas);
            this.role = a.Role;
            this.baseAction = a.BaseAction;
            this.lastAction = a.LastAction;
            this.rank = a.Rank;
            this.isSpeech = a.IsSpeech;
            this.gestureName = a.GestureName;
            this.msg = a.Msg;
            this.variable = a.Variable;
            this.subnet_name = a.SubnetName;
            this.state = a.State;
            this.AddressedTo = a.AddressedTo;
            this.talkedAbout = a.talkedAbout;
            this.Traits = new Dictionary<string, Trait>(a.Traits);
            //this.victimsName = a.VictimsName;
            //this.responce_to_variable = a.Responce_to_variable;
            this.target = a.Target;
        }

        public DEDAction(int id, string name)
        {
            this.name = name;
            this.id = id;
        }

        public DEDAction(int id, string name, bool essential, Dictionary<string, Precondition> precon
            , int nrExecutions, Dictionary<string, Goal> actionGoals, Dictionary<string, Schema> schemas
            , string role, string baseAction, string lastAction, int rank
            , bool isSpeech, string variable, string subnet, Dictionary<string, Trait> traits
            , string victimsName)
        {
            this.id = id;
            this.emotions = new List<Emotion>();
            this.traits = new Dictionary<string, Trait>(traits);
            this.name = name;
            this.essential = essential;
            this.preconditions = precon;
            this.nrExecutions = nrExecutions;
            this.goals = new Dictionary<string, Goal>(actionGoals);
            this.schemas = new Dictionary<string, Schema>(schemas);
            this.role = role;
            this.baseAction = baseAction;
            this.lastAction = lastAction;
            this.rank = rank;
            this.isSpeech = isSpeech;
            this.addressedTo = Constants.ALL;
            //this.VictimsName = victimsName;
            this.variable = variable;
            this.subnet_name = subnet;
        }

        public DEDAction(int id, string name, string knowledge, string subnet_name, int state
            , Goal goal, string victimsName)
        {
            this.id = id;
            this.variable = knowledge;
            this.State = state;
            this.AddressedTo = Constants.SELF;
            //this.VictimsName = victimsName;
            this.IsSpeech = true;
            this.name = name;
            this.subnet_name = subnet_name;
            this.goals.Add(goal.Name, goal);
        }

        public DEDAction(string msg, string addessedTo, string speaker, string name, Variable variable )
        {
            this.msg = msg;
            this.variable = variable.Name;
            this.State = variable.State;
            this.addressedTo = addessedTo;
            //this.VictimsName = victimsName;
            this.IsSpeech = true;
            this.name = name;
            this.subnet_name = variable.Subnet;
        }

        public DEDAction(string msg, string addessedTo, string talkedAbout, string speaker, string name
            , Variable variable, bool isQuestion, string goal_name)
        {
            this.msg = msg;
            this.variable = variable.Name;
            this.State = variable.State;
            this.addressedTo = addessedTo;
            this.talkedAbout = talkedAbout;
            this.IsSpeech = true;
            this.name = name;
            this.subnet_name = variable.Subnet;
            this.isQuestion = isQuestion;
            this.goals.Add(goal_name, null);
        }

        public DEDAction(int id, Strategy s, Dictionary<string, Schema> schemas, double rank
            , Dictionary<string, AssignedGoal> goals, ReadOutput output, string adressedto
            , Dictionary<string, Trait> traits, string victimsName, string talkedAbout)
        {
            XmlConfigurator.Configure();
            this.id = id;
            this.name = Constants.SAY;
            this.subnet_name = s.Submodel;
            this.variable = s.Variable;
            this.state = s.State;
            this.isDenial = s.IsDenial;
            this.essential = false;            
            this.nrExecutions = 1;
            this.schemas = new Dictionary<string, Schema>(schemas);
            //this.role = role;
            this.rank = rank;
            this.addressedTo = adressedto;
            this.TalkedAbout = talkedAbout;
            this.isSpeech = true;
            this.Traits = new Dictionary<string, Trait>(traits);
            setGoals(goals);
        }        
        


        /// <summary>
        /// Set any received goals as the actions goals if they have a variable, state and aim.
        /// </summary>
        /// <param name="goals"></param>
        private void setGoals(Dictionary<string, AssignedGoal> goals)
        {
            foreach (AssignedGoal goal in goals.Values)
            {
                if (goal.Variable == "" || goal.State == "" ) continue;
                this.goals.Add(goal.Name, goal);
                if (!this.goals[goal.Name].Schemas.ContainsKey(goal.Schema.Name))
                {
                    this.goals[goal.Name].Schemas.Add(goal.Schema.Name, goal.Schema);
                    this.goals[goal.Name].Value = goal.Value;
                }
                if (!this.schemas.ContainsKey(goal.Schema.Name)) this.schemas.Add(goal.Schema.Name, goal.Schema);
            }
        }
               
        
        int id;
        public int ID { get { return this.id; } }

        DateTime initTime = DateTime.Now;
        public DateTime InitTime { get { return this.initTime; } set { this.initTime = value; } }

        public System.TimeSpan EndTime { get { return DateTime.Now.Subtract(this.initTime); } }

        string gestureName;
        public string GestureName { get { return this.gestureName; } }

        string target;
        public string Target { get { return this.target; } set { this.target = value; } }
        

        string variable;
        public string Variable { get { return this.variable; } }

        bool isDenial;
        public bool IsDenial { get { return (this.State == 1); } }

        string subnet_name;
        public string SubnetName { get { return this.subnet_name; } }     

        int state;
        public int State { get { return this.state; } set { this.state = value; } }
        string msg;
        public string Msg { get { return this.msg; } set { this.msg = value; } }

        double rank = 0;
        public double Rank { get { return this.rank; } set { this.rank = value; } }

        bool isSpeech = false;
        public bool IsSpeech { get { return this.isSpeech; } set { this.isSpeech = value; } }

        bool isQuestion = false;
        public bool IsQuestion { get { return this.isQuestion; } set { this.isQuestion = value; } }

        bool isGreeting = false;
        public bool IsGreeting { get { return this.isGreeting; } set { this.isGreeting = value; } }

        string talkedAbout;
        public string TalkedAbout { get { return this.talkedAbout; } set { this.talkedAbout = value; } }
        string addressedTo;
        public string AddressedTo { get { return this.addressedTo; } set { this.addressedTo = value; } }
        //string speaker;
        //public string Speaker { get { return this.speaker; } set { this.speaker = value; } }

        //string victimsName;
        //public string VictimsName { get { return this.victimsName; } set { this.victimsName = value; } }

        string responce_to_variable;
        public string Responce_to_variable { get { return this.responce_to_variable; } set { this.responce_to_variable = value; } }

        /// <summary>
        /// Emotions that this action is descriptive for
        /// </summary>
        List<Emotion> emotions;

        /// <summary>
        /// Emotions that this action is descriptive for
        /// </summary>
        public List<Emotion> Emotions { get { return this.emotions; } }

        /// <summary>
        /// All preconditions that need to be satisfied for this action to be applicable
        /// </summary>
        Dictionary<string, Precondition> preconditions;

        /// <summary>
        /// All preconditions that need to be satisfied for this action to be applicable
        /// </summary>
        public Dictionary<string, Precondition> Preconditions { get { return this.preconditions; } }

        /// <summary>
        /// The traits that the action emphasises.
        /// </summary>
        Dictionary<string, Trait> traits = new Dictionary<string,Trait>();

        /// <summary>
        /// The traits that the action emphasises.
        /// </summary>
        public Dictionary<string, Trait> Traits { get { return this.traits; } set { this.traits = value; } }

        /// <summary>
        /// The action name
        /// </summary>
        string name;

        /// <summary>
        /// The action name
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Is the action essential
        /// </summary>
        bool essential;

        /// <summary>
        /// Is the action essential
        /// </summary>
        public bool Essential { get { return this.essential; } }
        
        /// <summary>
        /// Returns true if all preconditions are satisfied, else false.
        /// </summary>
        public bool IsPreconditionsSatisfied
        {
            get
            {
                foreach (Precondition p in this.preconditions.Values)
                { if (!p.IsSatisfied) return false; } return true;
            }
        }
        
        /// <summary>
        /// Marks a given precondition as satisfied.
        /// </summary>
        /// <param name="value"></param>
        public bool SatisfyPrecondition(string value, string role)
        {
            if (!this.preconditions.ContainsKey(value) ) return false;
            if (role != this.preconditions[value].Satisfier) return false;
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

        /// <summary>
        /// How many times it can be executed.
        /// </summary>
        int nrExecutions;
        /// <summary>
        /// How many times it can be executed.
        /// </summary>
        public int NrExecutions { get { return this.nrExecutions; } }

        /// <summary>
        /// Marks action as executed by decramenting nrExecutions
        /// </summary>
        public void ActionExecuted() { this.nrExecutions -= 1; }

        /// <summary>
        /// The schemas that the action belongs to.
        /// </summary>
        Dictionary<string, Schema> schemas = new Dictionary<string,Schema>();

        /// <summary>
        /// The schemas that the action belongs to.
        /// </summary>
        public Dictionary<string, Schema> Schemas { get { return this.schemas; } set { this.schemas = value; } }

        /// <summary>
        /// A dict of all goals, idx is goal.name
        /// </summary>
        Dictionary<string, Goal> goals = new Dictionary<string, Goal>();
        /// <summary>
        /// A dict of all goals, idx is goal.name
        /// </summary>
        public Dictionary<string, Goal> Goals { get { return this.goals; } }

        string baseAction;
        /// <summary>
        /// The class of actions that this action belongs to
        /// </summary>
        public string BaseAction { get { return this.baseAction; } }

        string role;
        /// <summary>
        /// The role that this action belogs to
        /// todo make role generic
        /// </summary>
        public string Role { get { return this.role; } }

        string lastAction;
        public string LastAction { get { return this.lastAction; } }

        
    }

}
