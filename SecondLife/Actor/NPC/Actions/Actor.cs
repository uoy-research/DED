using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DED.Utils;
using DED.Utils.Reads;
using DED.DPGE;
using DED.Director;
using DED.Decision;
using DED.NPC.Actions;
using log4net;
using log4net.Config;
using OpenMetaverse;
using System.Threading;
using DED.NPC.Utils;
using System.Xml.Serialization;
using System.IO;

using MySql.Data.MySqlClient;
using MySql.Data.Types;


namespace DED.NPC
{

    public struct Rank
    {
        public Rank( int value, string schema )
        {
            this.value = value;
            this.schema = schema;
        }

        int value;
        public int Value { get { return this.value; } }
        string schema;
        public string Schema { get { return this.schema; } }
    }

    public class Motive
    {
        public Motive(int id, string variable)
        {
            this.ID = id;
            this.Variable = variable;
        }

        public int ID { get; set; }
        public string Variable { get; set; }
    }

    public class UserAction
    {
        public UserAction(int id, string variable, int state, string addressedTo
            , string talkAbout, string sender
            ,string subnet, string goalName, bool isQuestion)
        {
            this.ID = id;
            this.Variable = variable;
            this.State = state;
            this.AddressedTo = addressedTo;
            this.TalkAbout = talkAbout;
            this.IsSpeech = true;
            this.Subnet = subnet;
            this.GoalName = goalName;
            this.IsQuestion = isQuestion;
            this.Sender = sender;
        }

        public int ID               { get; set; }
        public string Variable      { get; set; }
        public int State            { get; set; }
        public string AddressedTo   { get; set; }
        public string TalkAbout     { get; set; }
        public bool IsSpeech        { get; set; }
        public string Sender        { get; set; }
        public string Subnet        { get; set; }
        public string GoalName      { get; set; }
        public bool IsQuestion      { get; set; }       

    }
    

    public class ActorAction
    {
        public ActorAction(){}
        public ActorAction(int id, string variable, string state, string talkabout, string target, string responceVariable, String time)
        {
            this.ID = id;
            this.Variable = variable;
            this.State = state;
            this.TalkAbout = talkabout;
            this.Target = target;
            this.ResponceVariable = responceVariable;
            this.Time = time;
        }

        public int ID                   { get; set; }
        public string Variable          { get; set; }
        public string State             { get; set; }
        public string TalkAbout         { get; set; }
        public string Target            { get; set; }
        public string ResponceVariable  { get; set; }
        public string Time              { get; set; }
    }

    public class Actor : SLInterface
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Actor));


        Actor helper = null;
        public string Name { get { return base.FirstName + ' ' + base.LastName; } set { base.FirstName = value.Split(' ')[0]; base.LastName = value.Split(' ')[1]; } }

        int logical_time;

        public string ID { get; set; }
        
        OptimalStrategy optimalStrategy = new OptimalStrategy();

        Opponents opponents;
        /// The actors in the drama
        /// </summary>
        public override List<Actor> Actors { get { return base.Actors; } set { base.Actors = new List<Actor>(value); this.opponents = new Opponents(base.Actors); } }
        
        /// <summary>
        /// Actors that are curretnly on the mane scene
        /// </summary>
        List<Actor> actorsOnScene = new List<Actor>();

        /// <summary>
        /// The knowledgebase
        /// </summary>
        Knowledgebase kb;
        /// <summary>
        /// The knowledgebase
        /// </summary>
        public Knowledgebase KnowledgeBase { get { return this.kb; } }
        /// <summary>
        /// The roles that the actor is playing, indexed by roles
        /// </summary>
        Dictionary<string, Role> roles = new Dictionary<string, Role>();
        /// <summary>
        /// The roles that the actor is playing, indexed by roles
        /// </summary>
        public Dictionary<string, Role> Roles { get { return this.roles; } }
        /// <summary>
        /// The schemas that the actor is participating in
        /// </summary>
        Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
        public void setSchemasFaulty() { foreach (Schema s in this.schemas.Values) s.IsFaulty = true; }

        /// <summary>
        /// Goals that the actor has been assigned. Index goal,schema and object is AssignedGoal
        /// </summary>
        Dictionary<string,Dictionary<string,AssignedGoal>> assignedGoals = new Dictionary<string,Dictionary<string,AssignedGoal>>();
        public Dictionary<string, Dictionary<string, AssignedGoal>> AssignedGoals { get { return this.assignedGoals; } }

        /// <summary>
        /// Goals that the actor has been assigned. Index goal,schema and object is AssignedGoal
        /// </summary>
        Dictionary<string, AssignedGoal> currentGoals = new Dictionary<string, AssignedGoal>();
        public Dictionary<string, AssignedGoal> CurrentGoals { get { return this.currentGoals; }  }

        /// <summary>
        /// The actions that the actor has
        /// </summary>
        Dictionary<int, DEDAction> actions = new Dictionary<int, DEDAction>();

        public Dictionary<int, DEDAction> Actions { get { return this.actions; } }
        /// <summary>
        /// The utilities that the actor has
        /// </summary>
        Dictionary<string, Base_utility> utilities = new Dictionary<string, Base_utility>();
        /// <summary>
        /// The utilities that the actor has
        /// </summary>
        public Dictionary<string, Base_utility> Utilities { get { return this.utilities; } }
        
        /// <summary>
        /// The drama plot
        /// </summary>
        Dictionary<string, Subnet> plot;
        /// <summary>
        /// The drama plot
        /// </summary>
        public Dictionary<string, Subnet> Plot{ get { return this.plot; } }

        Dictionary<string, Rank> rank = new Dictionary<string,Rank>();
        public Dictionary<string, Rank> Rank { get { return this.rank; } }

        int currentAct;
        public int CurrentAct { set { this.currentAct = value; } }

        Dictionary<int, DEDAction> applicableActions = new Dictionary<int, DEDAction>();

        List<double> integrityValues = new List<double>();
        public List<double> IntegrityValues { get { return this.integrityValues; } }

        /// <summary>
        /// The traits of the character
        /// </summary>
        ReadTraits traits = new ReadTraits();
        /// <summary>
        /// The traits of the character
        /// </summary>
        public ReadTraits Traits { get { return this.traits; } }

        ReadPlot plotSettings;
        public ReadPlot PlotSettings { get { return this.plotSettings; } }
        public void SetPlotSettings(ReadPlot p)
        {
            this.plotSettings = new ReadPlot();
            foreach (Variable v in p.Variables.Values)
                this.PlotSettings.Variables.Add(v.Name, new Variable(v));
        }

        ReadOutput output;
        public ReadOutput Output { get { return this.output; } }

        Vector3 lookTowards;
        public Vector3 LookTowards { get { return this.lookTowards; } }

        List<ActorAction> actorActions = new List<ActorAction>();
        //public List<PlayerAction> PlayerActions { get { return this.playerActions; } set { this.playerActions = value; } }

        Conversations conversations = new Conversations();

        int logicalUserActionTime;

        Dictionary<string, string> files = new Dictionary<string, string>();
        string traits_file;
        int UID;

        MySQL mysql = new MySQL();

        public Actor() { }

        public Actor(int UID, string id, string first_name, string last_name, Dictionary<string, Subnet> plot
            , ReadOutput output, ReadPlot plottSettings
            , Dictionary<string, string> files, string traits_file)
            : base(first_name, last_name)
        {
            this.logicalUserActionTime = -1;
            this.ID = id;
            this.logical_time = 0;
            this.SetPlotSettings(plottSettings);
            this.plot = new Dictionary<string,Subnet>(plot);
            //Connect to MySQL DB 
            this.mysql.Connect();
            
            XmlConfigurator.Configure();
            this.output = output;
            base.ActorNPC = this;
            this.UID = UID;
            this.files = new Dictionary<string, string>(files);

            this.files["actor_actions"] = string.Format(this.files["actor_actions"], this.UID, this.ID);
            this.files["user_actions"] = string.Format(this.files["user_actions"], this.UID);
            this.files["actor_motive"] = string.Format(this.files["actor_motive"], this.UID, this.ID);

            this.traits_file = traits_file;
        }

        public void WriteActionToDB(ActorAction a)
        {            
            int idx = a.Variable.IndexOf("_");
            string variable = a.Variable.Substring(idx + 1);

            //first check if the variable excists
            string sql = "select count(variableID) as c, variableID from ded_dpge_variable where name = '" + variable + "'";
            MySqlDataReader reader = this.mysql.Query(sql);

            int variableID = 0;
            reader.Read();
            int count = Convert.ToInt32(reader["c"]);
            if (count > 0)
                variableID = Convert.ToInt32(reader["variableID"]);
            reader.Close();
            if (variableID == 0) return;

            //find stateID
            sql = "select count(stateID) as c, stateID from ded_dpge_variable_state where variableID = '" + variableID + "'";
            reader = this.mysql.Query(sql);

            int stateID = 0;
            reader.Read();
            count = Convert.ToInt32(reader["c"]);
            if (count > 0)
                stateID = Convert.ToInt32(reader["stateID"]);
            reader.Close();
            if (stateID == 0) return;

            //insert into actor action table.
            sql = string.Format("insert into ded_actor_action (actor_action_ID, UID,NPCID,talk_about_NPCID,variableID,stateID,target,response_variable,time,log_time) values ({0},{1},{2},{3},{4},{5},'{6}','{7}',{8},{9})",
                this.logical_time, this.UID, this.ID, a.TalkAbout, variableID, stateID, a.Target, a.ResponceVariable, a.Time, a.ID);

            this.mysql.Insert(sql);
            ++this.logical_time;
            
        }

        public void WriteMotiveToDB(string motive)
        {
            string sql = string.Format("SELECT count(motive_ID) as c, motive_ID FROM ded_Motive WHERE motive = '{0}'", motive);
            MySqlDataReader reader = this.mysql.Query(sql);
            reader.Read();

            int count = Convert.ToInt32(reader["c"]);
            if (count < 1) return;
            int motive_ID = Convert.ToInt32(reader["motive_ID"]);
            reader.Close();

            sql = string.Format("INSERT INTO ded_NPC_Motive (UID,NPCID,motive_ID,known) VALUES ({0},{1},{2},{3})",this.UID,this.ID,motive_ID,0);
            this.mysql.Insert(sql);            
        }

        List<UserAction> ReadUserActionsFromDB()
        {
            List<UserAction> list = new List<UserAction>();

            string sql = string.Format("SELECT * FROM ded_user_speech WHERE user_speech_ID > {0} AND UID = {1} AND NPCID = {2}", 
                this.logicalUserActionTime, this.UID, this.ID);
            MySqlDataReader reader = this.mysql.Query(sql);

            while (reader.Read())
            {
                int logtime = Convert.ToInt32( reader["user_speech_ID"] );
                list.Add(new UserAction(logtime, Convert.ToString( reader["variable"] ), Convert.ToInt32( reader["state"] ), this.ID,
                    Convert.ToString(reader["talk_about_NPCID"]), Convert.ToString(this.UID), 
                    Convert.ToString( reader["subnet"] ), Convert.ToString( reader["goal_name"] ), Convert.ToBoolean( reader["isquestion"] )));
                if (logtime > this.logicalUserActionTime) this.logicalUserActionTime = logtime;
            }
            reader.Close();
            
            return list;
        }
 
        
        /// <summary>
        /// Creates the knowledgebase
        /// </summary>
        public void CreateActorKB()
        {
            KnowledgebaseGenerator k = new KnowledgebaseGenerator(base.FirstName + " " + base.LastName);
            this.kb = k.CreateKnowledgeBase(this.plot, base.Actors);
            
            setKnowledgeBase();
            
            this.traits.ReadFile(this.traits_file);
            this.integrityValues.Add(this.traits.GetValue(this.ID, Constants.INTEGRITY));
            this.integrityValues.Add(1 - this.traits.GetValue(this.ID, Constants.INTEGRITY));

            WriteKnowledgebase();
        }


        /// <summary>
        /// Create a new goal to propagate to each actor that gives them the goal need to satisfy to give correct info. 
        /// </summary>
        public void PropogateGoalsToOtherActors()
        {
            if (!this.AssignedGoals.ContainsKey("motive")) return;
            if (!this.AssignedGoals["motive"].ContainsKey("actor_motive_SELF")) return;
            AssignedGoal ag = this.AssignedGoals["motive"]["actor_motive_SELF"];
            AssignedGoal new_ag = new AssignedGoal(ag.DramaGoal,ag.Schema,ag,ag.Actor);
            new_ag.Applies = this.ID;
            new_ag.Name = "actor_" + new_ag.Name+ "_"+ this.ID;
            new_ag.Variable = this.plotSettings.Variables["S_motive"].Name;
            
            //propagate the goal to each actor except itself
            foreach (Actor a in this.Actors)
            {
                if (a.ID == this.ID) continue;
                a.AddGoal(new_ag);
            }
        }

        private void WriteKnowledgebase()
        {
            foreach (Variable v in plotSettings.Variables.Values)
            {
                List<ContextSubnet> subnets = kb.GetSubnets(v.Subnet);
                foreach (ContextSubnet subnet in subnets)
                {
                    string file = string.Format("characters/{0}/{1}.xdsl", this.Name, subnet.Name);
                    subnet.Knowledges[v.Name].Net.WriteFile(file);
                }
            }
        }

        private void setKnowledgeBase()
        {
            PlotGenerator dg = new PlotGenerator();
            dg.SetPlot(this.kb, this.plotSettings);
            //Bayes net = this.kb.GetSubnet("suspect").Knowledges["S_occupation"].Net;
            //net.WriteFile("debug/Plot set_S_occupation.xdsl");

            //write motive
            string m = this.plotSettings.Variables["S_motive"].Name;
            WriteMotiveToDB(m);
        }
                
        /// <summary>
        /// If action is needed then choose one
        /// and execut it 
        /// </summary>
        public void Act()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(Constants.SLEEP);
                    //log.InfoFormat("{0}, applicable actions {1}", this.first_name, this.applicableActions.Count);
                    if (!IsActionNeeded()) continue;
                    
                    DEDAction action = PickAction();

                    if (action == null) continue;
                    log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0}, Current action '{1}'", base.FirstName, action.Name);
                    Execute(action);
                    action = null;
                    log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0}, applicable actions {1}", base.FirstName, this.applicableActions.Count);
                }
            }
            catch (Exception e)
            {
                log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0} Error '{1}'", this.FirstName, e.Message);
            }            
        }

        /// <summary>
        /// checks if:
        /// there is an active call for responce 
        /// there is a request for an action
        /// there is an internal result
        /// there is no active action and it has unfulfilled goals.
        /// </summary>
        private bool IsActionNeeded()
        {
            if (IsNewUserAction()) return true;
            //if there are no applicableActions actions
            lock (this.applicableActions)
            {
                if (this.applicableActions.Count != 0) return true;
            }
 
            if (this.conversations.IsActiveSpeeches()) return true;

            lock (this.conversations)
            {
                if (this.conversations.IsTimeToEngage())
                {
                    lock (this.applicableActions)
                    {
                        AddSpeechToApplicableActions();
                    }
                    return false;
                }
            }
            
            // It has unfulfilled goals.
            if (this.isUnsatisfiedGoals()) return true;
            return false;
        }

        private bool IsNewUserAction()
        {
            if (this.kb == null || this.currentAct < 1 || this.assignedGoals.Count < 2) return false;
            bool isNew = false;
            
            List<UserAction> uas = ReadUserActionsFromDB();
            foreach (UserAction a in uas)
            {                
                Variable v = new Variable(a.Variable, a.Subnet, a.State);
                DEDAction action = new DEDAction(a.ID.ToString(), a.AddressedTo, a.TalkAbout, a.Sender, a.Variable, v, a.IsQuestion, a.GoalName);
                this.conversations.addSpeech(action, a.Sender, new Variable(a.Variable, a.Subnet, -1), SentenceType.Question);
                isNew = true;                
            }
            return isNew;
        }

        private void AddSpeechToApplicableActions()
        {
            foreach (DEDAction a in this.actions.Values)
                if (a.Name == Constants.SAY 
                    && a.NrExecutions > 0
                    && AllPreconditionsSatisfied(a)) {
                        ProcessSpeechAction(a);
                        DEDAction new_a = this.optimalStrategy.Say(a, this);
                        this.conversations.addSpeech(new_a, this.Name, new Variable(new_a.Variable, new_a.SubnetName, new_a.State), SentenceType.Claim);
                        Conversation c = this.conversations.getSpeech();
                        if (c != null && !this.applicableActions.ContainsKey(c.Action.ID))
                        {
                            this.applicableActions.Add(c.Action.ID, c.Action);
                            log.InfoFormat("{0}, adds action variable '{1}' to applicable actions", this.Name, c.Action.Variable);
                            a.ActionExecuted();
                        }
                    return; 
                }                
        }

        private void ProcessSpeechAction(DEDAction action)
        {
            DEDAction new_a = this.optimalStrategy.Say(action, this);
            this.conversations.addSpeech(new_a, this.Name, new Variable(new_a.Variable, new_a.SubnetName, new_a.State), SentenceType.Claim);
            Conversation c = this.conversations.getSpeech();
            if (c != null && !this.applicableActions.ContainsKey(c.Action.ID))
            {
                this.applicableActions.Add(c.Action.ID, c.Action);
                log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0}, adds action variable '{1}' to applicable actions", this.Name, c.Action.Variable);
                action.ActionExecuted();
            }
        }

        /// <summary>
        /// Marks an action as executed and marks preconditions satisfied on relevant actions.
        /// </summary>
        /// <param name="action"></param>
        private void UpdateExecutedActions(DEDAction action, string role)
        {            
            lock (this.actions)
            {
                lock (this.applicableActions)
                {
                    foreach (DEDAction a in this.actions.Values)
                    {
                        //log.InfoFormat("Actor {1}, updating action {4} for action name {2} action nr executions {0} is precon satisfied {3}"
                        //    , a.NrExecutions, base.FirstName, a.Name, a.IsPreconditionsSatisfied, action.Name);

                        if (action.ID == a.ID) { a.ActionExecuted(); }

                        if (a.SatisfyPrecondition(action.Name, role) || a.SatisfyPrecondition(action.Variable, role))
                            if (AllPreconditionsSatisfied(a) && !this.applicableActions.ContainsKey(a.ID))
                            {
                                a.AddressedTo = action.AddressedTo;
                                //if (!setAction(a))
                                if (a.Name != Constants.SAY)
                                    this.applicableActions.Add(a.ID, a);
                                if (a.Name == Constants.SAY && a.Rank == 1.0)
                                    ProcessSpeechAction(a);
                                //log.InfoFormat("Actor {1}, updating action {4} for action name {2} action applicable actions {0} is precon satisfied {3}"
                                //, this.applicableActions.Count, base.FirstName, a.Name, a.IsPreconditionsSatisfied, action.Name);
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="action"></param>
        private void Execute(DEDAction action)
        {
            if (action == null) return;
            log.InfoFormat("({3}), Executing: action '{0}', state '{1}', Addressed to '{2}'"
                , action.Variable, action.State, action.AddressedTo, this.Name);
            
            //todo make into a separate thread and lock action
            lock (this.applicableActions)
            {
                this.applicableActions.Remove(action.ID);
            }
            if (action.IsSpeech) {
                string time = string.Format("{0}",action.EndTime.Milliseconds);
                ActorAction aa = new ActorAction(action.ID, action.Variable
                    , Convert.ToString(action.State), action.TalkedAbout, action.Target, action.Responce_to_variable, time);
                WriteActionToDB(aa);
            }
            
            log.InfoFormat("({3}), Executed action '{0}', state '{1}', Addressed to '{2}'"
                , action.Variable, action.State, action.AddressedTo, this.Name);
            
            if (action != null) UpdateExecutedActions(action, "self");
            HandleContradiction(action);            
            SatisfyGoals(action);
        }
        

        private void HandleContradiction(DEDAction action)
        {
            if (Contradiction(action) && action.IsSpeech)
            {                
                if (this.helper == null)
                {
                    Random rand = new Random();
                    while (true)
                    {
                        int idx = rand.Next(base.Actors.Count - 1);
                        this.helper = base.Actors[idx];
                        if (this.helper.Name != this.Name) break;
                    }
                }
                action.AddressedTo = this.helper.Name;
                this.helper.RequestResponce(action, this.ID);
            }
        }

        /// <summary>
        /// Do the character goals of the action contradict one of the actors goals
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool Contradiction(DEDAction action)
        {
            lock (this.assignedGoals)
            {
                foreach (Dictionary<string,AssignedGoal> dict in this.assignedGoals.Values)
                {
                    foreach (Director.AssignedGoal actorGoal in dict.Values)
                    {
                        if (actorGoal.Owner != Constants.ACTOR) continue;
                        foreach (Director.Goal actionGoal in action.Goals.Values)
                        {
                            if ( !this.plotSettings.Variables.ContainsKey(actionGoal.Variable) ) return false;
                            if ( this.plotSettings.Variables[actionGoal.Variable].Name != action.Variable ) return false;
                            if ( this.plotSettings.Variables[actionGoal.Variable].State == action.State ) return false;
                            if (actionGoal.Owner != Constants.CHARACTER) continue;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool StatesContradict(Goal a, Goal b) { return (a.Applies == b.Applies && a.Variable == b.Variable && a.State != b.State); }

       
        /// <summary>
        /// Filters applicable actions and chooses one
        /// </summary>
        /// <param name="action"></param>
        private DEDAction PickAction()
        {
            DEDAction action = null;

            // 
            if (this.conversations.IsActiveSpeeches())
                this.optimalStrategy.EvaluateResponces(this.conversations.getSpeech(), this);

            List<DEDAction> optimalActions = new List<DEDAction>();
            FilterOptimalActions(optimalActions);

            //randomly picks one action among all optimal actions
            if (optimalActions.Count > 0)
            {
                Random RandomClass = new Random();
                int rand = RandomClass.Next(0, optimalActions.Count);
                action = optimalActions[rand];
            }
            //if there is no applicable action then action is set to null
            else return null;

            //if optimal action is not in character
            if (!action.Traits.ContainsKey(Constants.CHATTY.ToLower())) return action;
            Trait trait = action.Traits[Constants.CHATTY.ToLower()];
            Random random = new Random();
            double d = random.NextDouble();
            double dd = this.Traits.GetValue(this.ID, Constants.CHATTY);
            if (d > this.Traits.GetValue(this.ID, Constants.CHATTY)
                || d > trait.Value)
            {
                lock (this.applicableActions)
                {
                    this.applicableActions.Remove(action.ID);
                }
                return null;
            }
            return action;
        }

        /// <summary>
        /// Adds all actions with the highest rank to the optimal actions list
        /// </summary>
        /// <param name="optimalActions"></param>
        private void FilterOptimalActions(List<DEDAction> optimalActions)
        {
            lock (this.applicableActions)
            {
                foreach (DEDAction a in this.applicableActions.Values)
                {
                    if (optimalActions.Count != 0 && (a.Rank > optimalActions[0].Rank))
                    {
                        optimalActions.Clear();
                        optimalActions.Add(a);
                        continue;
                    }

                    optimalActions.Add(a);
                }
            }
        }

        public void Stimuly(DEDAction action, string sender_name) {
            UpdateKnowledge(action, sender_name);
            if (this.currentAct < 1 || !IsRelevant(action)) return;
            this.conversations.addSpeech(action, sender_name, new Variable(action.Variable, action.SubnetName, -1), SentenceType.Question);

        }

       

        /// <summary>
        /// Received stimuly(action) from other actors on scene.
        /// </summary>
        /// <param name="a">action</param>
        public void Stimuly(List<DEDAction> actions, string sender_name)
        {
            log.InfoFormat(this.UID + " - " + this.ID + " - " + "({2}) Stimuly: Received actions '{0}', sender '{1}'"
                , actions.Count, sender_name, this.Name);

            // Reads the notification and checks wheather this action is relevant to the actor            
            bool isRelevant = IsRelevant(actions);

            //harvest knowledge
            //UpdateKnowledge(action);

            // If it is not relevant then function exited
            if (!isRelevant) return;

            // If it is relevant

            // If it is speech: Computes a set of optimal responses with respect
            // to its knowledge base, character, situation, emotions and its current goals.
            // This is done by querying the knowledgebase and calculationg utilities
            // Marks the applicable responces with a need to respond flag and they are added to applicable actions
            if (actions[0].IsSpeech && actions[0].AddressedTo == this.Name) EvaluateResponces(actions, sender_name);

            // If it is not a question then all actions that have this action as a precondition have the
            // precondition marked as satisfied and if the action has no unsatisfied precon 
            // it is added to the list of applicable actions.                        
            else foreach (DEDAction action in actions) EvaluatActions(action, sender_name);
        }

        

        /// <summary>
        /// Evaluates all actions for actions that are applicable but are not on the list of applicable actions
        /// </summary>
        /// <param name="action"></param>
        private void EvaluatActions(DEDAction sender_action, string sender_name)
        {   
            // the action passed has just been executed so the first thing
            // is to satisfy preconditions and add actions that then become 
            // applicable to the list of applicable actions
            lock (this.applicableActions)
            {
                foreach (DEDAction actor_action in this.actions.Values)
                {
                    if (this.applicableActions.ContainsKey(actor_action.ID)) continue;
                    string role = sender_action.Role;
                    if (sender_name == this.Name) role = "self";
                    if (actor_action.SatisfyPrecondition(sender_action.Name, role))
                    {
                        log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0} action name '{1}'", base.FirstName, sender_action.Name);

                        if (AllPreconditionsSatisfied(actor_action)) this.applicableActions.Add(actor_action.ID, actor_action);
                    }
                }
            }
        }

        private void UpdateKnowledge(DEDAction action, string sender)
        {
            //If an actor dies then remove from actor array
            if ( action.Name.ToUpper() == Constants.DIE && sender != this.Name ) {
                Actor removeActor = null;
                foreach (Actor a in this.Actors)
                {
                    if (a.Name == sender)
                    {
                        removeActor = a;
                        break;
                    }
                }
                this.Actors.Remove(removeActor);
                this.DeadActors.Add(removeActor);
            }

            if (action != null) UpdateExecutedActions(action, action.Role);
        }

        /// <summary>
        /// Is it in one of its schemas and is it a precon to  one of its actions
        /// is it addressed ToString the actor
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool IsRelevant(DEDAction action)
        {
            if (action.Variable == "" || action.SubnetName == "") return false;
            if (action.IsGreeting) return true;
            //is it addressed ToString the actor
            if (this.Name.Contains(action.AddressedTo) && action.AddressedTo.Length > 3) return true;

            //if (action.AddressedTo == Constants.ALL)
            //{   
            //    //in one of its schemas
            //    foreach ( Schema schema in action.Schemas.Values ) if (this.schemas.ContainsKey(schema.Name)) return true;
            //    if (this.HasPrecondition(action)) return true;
            //}
            
            return false;
        }

        /// <summary>
        /// Is it in one of its schemas and is it a precon to  one of its actions
        /// is it addressed ToString the actor
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool IsRelevant(List<DEDAction> actions) {
            foreach (DEDAction a in actions) if (IsRelevant(a)) return true; return false; }

        public void RequestResponce(DEDAction sender_action, string sender)
        {
            this.optimalStrategy.RequestResponce(sender_action, sender, this);           
        }

        private void EvaluateResponces(List<DEDAction> sender_actions, string sender_name)
        {
           // log.InfoFormat("({2}) Stimuly: Received actions '{0}', sender '{1}'"
             //   , sender_actions.Count, sender_name, this.Name);
        }

        public void SetForExecution(DEDAction action)
        {
            lock (this.applicableActions)
            {
                if (!this.applicableActions.ContainsKey(action.ID))
                    this.applicableActions.Add(action.ID, action);
            }
        }

        public void SetForExecution(Conversation conv, DEDAction action)
        {
            lock (this.applicableActions)
            {
                if (!this.applicableActions.ContainsKey(action.ID))
                {
                    this.applicableActions.Add(action.ID, action);
                    this.conversations.addResponce(conv, new Variable(action.Variable, action.SubnetName, action.State));
                }
            }
        }

        
        public int LogicalTime() { ++this.logical_time; return this.logical_time; }

        public Dictionary<string, Schema> getActionSchemas(DEDAction sender_action)
        {
            Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
            foreach (Schema schema in sender_action.Schemas.Values) if (this.schemas.ContainsKey(schema.Name))
                    schemas.Add(schema.Name, this.schemas[schema.Name]);
            return schemas;
        }


        /// <summary>
        /// adds a schema to the actor by adding a role, the schemas actions and goals
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="role"></param>
        public void DeploySchema(Schema schema, Role role, int rank)
        {
            log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0} received schema '{1}'", base.FirstName, schema.Name);

            //add the schema
            if (!this.schemas.ContainsKey(schema.Name)) this.schemas.Add(schema.Name, new Schema(schema));
            //Add the role
            if (!this.roles.ContainsKey(role.Name)) this.roles.Add(role.Name, role);
            //log.InfoFormat("{0} my role is: '{1}'", base.FirstName, role.Name);
            //Add the rank
            if (!this.rank.ContainsKey(schema.Name)) this.rank.Add(schema.Name, new Rank(rank, schema.Name));
            //add the actions
            foreach (DEDAction a in role.Actions)
            {//TODO add errorhandling
                DEDAction newAction = new DEDAction(LogicalTime(), a);
                //if the action does not exist then add it
                lock (this.actions)
                {
                    if (!this.actions.ContainsKey(newAction.ID)) this.actions.Add(newAction.ID, newAction);

                    //add schema annotation
                    if (!this.actions[newAction.ID].Schemas.ContainsKey(schema.Name)) this.actions[newAction.ID].Schemas.Add(schema.Name, schema);
                }                
            }              
        }

        public void AddToApplicableActions()
        {
            lock (this.applicableActions)
            {
                foreach (DEDAction a in this.actions.Values)
                    if (a.Preconditions.Count == 0 && a.NrExecutions > 0)
                    {
                        if (a.Name != Constants.SAY && !this.applicableActions.ContainsKey(a.ID))
                            this.applicableActions.Add(a.ID, a);
                    }
            }
        }
        

        /// <summary>
        /// Revokes a schema from the actor
        /// removes all relevant goals, actions, roles, and the schema itself.
        /// </summary>
        /// <param name="schema"></param>
        internal void RevokeSchema(Schema schema)
        {
            log.InfoFormat(this.UID + " - " + this.ID + " - " + "{0} revoking schema '{1}'", base.FirstName, schema.Name);

            //Remove the schema
            this.schemas.Remove(schema.Name);
            //Remove the Rank
            this.rank.Remove(schema.Name); 
            
            //first retrive the role of the actor
            Role role = schema.SchemaActors[this.ID].Role;

            //There can be one or more schema per action
            //Removes the role that the actor held for that schema
            this.roles.Remove(role.Name);

            foreach (DEDAction a in role.Actions)
            {
                int id = 0;
                foreach (DEDAction action in this.actions.Values)
                {
                    if (a.Name == action.Name && action.Schemas.ContainsKey(schema.Name)) id = action.ID;
                }

                lock (this.actions)
                {
                    //Delete the schema annotation from the action
                    if (this.actions[id].Schemas.ContainsKey(schema.Name))
                        this.actions[id].Schemas.Remove(schema.Name);

                    //if there are no schema annotations then remove the action
                    if (this.actions[id].Schemas.Count == 0) this.actions.Remove(id);
                }
            }      
        }

        /// <summary>
        /// Goals are added to dictionary assigned goals and indexed by 
        /// the drama goal that they contribute to satisfy
        /// </summary>
        /// <param name="ag"></param>
        internal void AddGoal(AssignedGoal ag)
        {
            //the actor has one of each drama goal and a range of schema goals that contribute to the dramagoal, 
            //the schema goals are indexed by the drama goal
            if (!this.assignedGoals.ContainsKey(ag.DramaGoal.Name))
                this.assignedGoals.Add(ag.DramaGoal.Name, new Dictionary<string,AssignedGoal>());
            if (!this.assignedGoals[ag.DramaGoal.Name].ContainsKey(ag.Name))
                this.assignedGoals[ag.DramaGoal.Name].Add(ag.Name, ag);
            else this.assignedGoals[ag.DramaGoal.Name][ag.Name] = ag;
        }

        public bool hasGoal(AssignedGoal goal)
        {
            if (this.assignedGoals.ContainsKey(goal.DramaGoal.Name))
                if (this.assignedGoals[goal.DramaGoal.Name].ContainsKey(goal.Name))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if there are unsatisfied goals
        /// </summary>
        /// <returns></returns>
        bool isUnsatisfiedGoals() { if (this.assignedGoals.Count == 0) return false; return true; }

        /// <summary>
        /// Marks the goals satisfied that the last successfully executed action has declared.
        /// </summary>
        void SatisfyGoals(DEDAction action)
        {
            
            if (this.assignedGoals.Count == 0) return;
            //log.InfoFormat("{0} satisfying goals for action '{1}'", this.Name, action.Variable);
            foreach (Goal actionG in action.Goals.Values)
            {
                foreach (Schema schema in action.Schemas.Values)
                {                    
                    foreach (SchemaGoal sg in schema.Goals.Values)
                    {
                        if (sg.Name == actionG.Name)
                        {
                            if (this.assignedGoals.ContainsKey(sg.DramaGoal))
                            {
                               // log.InfoFormat("{0}, pre update dramagoal goal '{3}' and schema goal '{4}' for action '{1}', drama goal value = '{2}'"
                                 //   , this.Name, action.Variable, this.assignedGoals[sg.DramaGoal][sg.Name].Value, sg.DramaGoal, sg.Name);
                                this.assignedGoals[sg.DramaGoal][sg.Name].AddValue(actionG.Value);
                               // log.InfoFormat("{0}, post update dramagoal goal '{3}' and schema goal '{4}' for action '{1}', drama goal value = '{2}'"
                                  //  , this.Name, action.Variable, this.assignedGoals[sg.DramaGoal][sg.Name].Value, sg.DramaGoal, sg.Name);
                            }
                        }
                    }                    
                }
            }            
        }

        /// <summary>
        /// Returns true if the action passed is a precondition to any 
        /// of the actors actions that are in the same action group as the action passed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal bool HasPrecondition(DEDAction action)
        {
            foreach (DEDAction a in this.actions.Values)
                if (a.BaseAction == action.BaseAction)
                    if ( a.Preconditions.ContainsKey(action.Name) ) 
                            return true;
            return false;
        }

        /// <summary>
        /// Returns true if the action passed is a precondition to any 
        /// of the actors actions that are in the same action group as 
        /// the action passed and the precondition has been satisfied
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal bool HasPreconditionSatisfied(DEDAction action)
        {
            foreach (DEDAction a in this.actions.Values)
                if (a.BaseAction == action.BaseAction)
                    if (a.Preconditions.ContainsKey(action.Name))
                        if (a.Preconditions[action.Name].IsSatisfied)
                            return true;
            return false;
        }

        internal bool AllPreconditionsSatisfied(DEDAction action)
        {
            foreach (Precondition precon in action.Preconditions.Values)
                if (!precon.IsSatisfied) return false; return true;
        }
    }
}


