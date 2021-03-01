using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.Utils.Reads;
using DED.DPGE;
using DED.NPC;
using DED.NPC.Actions;
using log4net;
using log4net.Config;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.IO;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace DED.Director
{
    /// <summary>
    /// Act contains the act number and the goals defined for that act.
    /// </summary>
    public struct Act
    {
        public Act(Dictionary<string, DramaGoal> goals, int number)
        { 
            this.goals = goals;
            this.number = number;
        }

        Dictionary<string, DramaGoal> goals;
        public Dictionary<string, DramaGoal> Goals { get { return this.goals; } }

        int number;
        public int Number { get { return this.number; } }
    }

    [Serializable]
    public class DramaGoal : Goal
    {
        public DramaGoal(string name, string owners, int value, string die)
            : base(name, value, null, null, null, owners)
        {
            this.owners = owners;
            this.accumulatedValue = 0;
            this.die = die;
        }

        string owners;
        public string Owners { get { return this.owners; } }
        //role name that will die.
        string die;
        public string Die { get { return this.die; } }
        //bool satisfied;
        //public override bool Satisfied { get { return this.satisfied; } set { base.satisfied = value; } }
        int numberOfActors;
        /// <summary>
        /// The number of actors partisipating in the goal.
        /// </summary>
        public int NumberOfActors { get { return this.numberOfActors; } set { this.numberOfActors = value; } }
        int accumulatedValue = 0;

        /// <summary>
        /// value by number of actors to get a normalised accumulated value.
        /// </summary>
        public void UpdateAccumulatedValue(int agvalue){
            this.accumulatedValue += agvalue;
            if (this.accumulatedValue >= base.Value) base.IsSatisfied = true;
        }                
    }

    [Serializable]
    public class AssignedGoal : Goal
    {
        public AssignedGoal(DramaGoal goal, Schema schema, Goal schemaGoal, Actor actor)
            : base(schemaGoal.Name, 0, schemaGoal.Variable, schemaGoal.State, schemaGoal.Owner, schemaGoal.Applies, schemaGoal.Role)
        {
            this.dramaGoal = goal;
            this.schema = schema;
            this.actor = actor;            
        }
       

        DramaGoal dramaGoal;
        public DramaGoal DramaGoal { get { return this.dramaGoal; } }
        Schema schema;
        public Schema Schema { get { return this.schema; } }
        Actor actor;
        public Actor Actor { get { return this.actor; } }
        

        public void AddValue( int value) {
            this.dramaGoal.UpdateAccumulatedValue(value);
            base.IsSatisfied = true;
        }
    }

    public class Avatar
    {
        //string title; string first_name; string last_name; string img;

        public Avatar(string id, string title, string first_name, string last_name, string img)
        {
            this.ID = id; this.Title = title; this.FirstName = first_name; this.LastName = last_name; this.Img = img;
        }

        public string Title { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Img { get; set; }
    }    

    public class Settings
    {
        public Settings()
        {
            this.Act = -1; this.Victim = ""; this.Event = ""; this.IsPlot = false;
            this.IsKnowledgebase = false; this.IsCharacters = false; this.IsNewgame = false;
        }

        public int Act { get; set; }
        public string Victim { get; set; }
        public string Event { get; set; }
        public bool IsPlot { get; set; }
        public bool IsKnowledgebase { get; set; }
        public bool IsCharacters { get; set; }
        public bool IsNewgame { get; set; }
    }


    

    public class Deployed_goal
    {
        public Deployed_goal(string goal_name, int log_time)
        {
            this.GoalName = goal_name;
            this.LogicalTime = log_time;
        }

        public string GoalName { get; set; }
        public int LogicalTime { get; set; }
    }

    class DirectorDrama
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectorDrama));

        /// <summary>
        /// The complete set of schemas
        /// </summary>
        Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
        
        /// <summary>
        /// This holds all assigned goals indexed by goal,schema,actor and contains object AssignedGoal
        /// </summary>
        Dictionary<string, Dictionary< string, Dictionary<string, AssignedGoal>>> assignedGoals = new Dictionary<string,Dictionary<string,Dictionary<string,AssignedGoal>>>();
        
        /// <summary>
        /// Schemas that are suitable for deployment
        /// </summary>
        List<Schema> applicableSchemas = new List<Schema>();
        /// <summary>
        /// Schemas that are suitable for deployment
        /// </summary>
        List<Schema> optimalSchemas = new List<Schema>();

        /// <summary>
        /// Currecntly deployment schemas
        /// </summary>
        List<Schema> deployedSchemas = new List<Schema>();

        /// <summary>
        /// The complete set of actors
        /// </summary>
        List<Actor> actors = new List<Actor>();
        
        ReadOutput output;
        ReadPlot plotSettings;

        /// <summary>
        /// A dict of all actors threads, idx is actor.name
        /// </summary>
        List<Thread> actorThreads = new List<Thread>();

        /// <summary>
        /// A list of the acts
        /// </summary>
        List<Act> acts = new List<Act>();
        List<Deployed_goal> deployed_goals = new List<Deployed_goal>();

        /// <summary>
        /// The current act
        /// </summary>
        int currentAct;

        /// <summary>
        /// Nr of unsatisfied goals
        /// </summary>
        int nrUnsatisfiedGoals;

        /// <summary>
        /// The murder plot
        /// </summary>
        PlotNetwork plot;

        /// <summary>
        /// The knowledgebase 
        /// </summary>
        Dictionary<string, Subnet> k = new Dictionary<string,Subnet>();

        /// <summary>
        /// All goals that need to be satisfied for this act.
        /// </summary>
        Dictionary<string, DramaGoal> currentGoals = new Dictionary<string,DramaGoal>();

        Settings settings = new Settings();
        int logical_time = 1;

        /// <summary>
        /// Marks whether a schema has been revoked
        /// </summary>
        bool isRevoked;

        string traits_file;
        int UID;

        MySQL mysql = new MySQL();

        public bool STOP { get; set; }

        public DirectorDrama() { XmlConfigurator.Configure(); }


        public void Init(int UID, string schemafile, string plotfile, string dramafile,
            string primfile, string outputfile, string drinksfile, string plottsettingsfile
            , Dictionary<string, string> files, string traits_file)
        {
            this.schemas.Clear();
            this.assignedGoals.Clear();
            this.applicableSchemas.Clear();
            this.optimalSchemas.Clear();
            this.deployedSchemas.Clear();
            this.actors.Clear();
            this.actorThreads.Clear();
            this.acts.Clear();
            this.deployed_goals.Clear();
            this.currentAct = 0;
            this.k.Clear();
            this.currentGoals.Clear();
            this.settings = new Settings();
            this.logical_time = 1;
            this.isRevoked = false;            
            this.UID = UID;
            this.settings.Act = 0;
            this.settings.Victim = "";

            //Connect to MySQL DB 
            this.mysql.Connect();

            //set filenames
            this.traits_file = traits_file;    
            
            //clear settings so that the user does not jump start the game.
            SettingsToDB();

            ReadFiles rf = new ReadFiles(mysql);

            this.plot = rf.ReadPlotFile(plotfile);
            //writePlotToDB();
            this.plotSettings = rf.ReadPlot(plottsettingsfile);
            this.output = rf.ReadOutputFile(outputfile);

            List<Avatar> avatars = rf.LoadAvatarFromDB("select * from ded_NPC where NPC_type = 2");

            this.acts = rf.ReadDramaFile(dramafile);
            this.schemas = rf.ReadSchemaFile(schemafile);


            PlotGenerator dg = new PlotGenerator();
            dg.SetSingularVariables(this.plot, this.plotSettings);
            this.settings.IsPlot = true;
            this.settings.Victim = this.plotSettings.Variables["V_name"].State_label;
            this.settings.Event = "e1";
            FillMurderObject();

            SettingsToDB();

            //create actors
            foreach (Avatar a in avatars)
                if (!this.settings.Victim.Contains(a.ID))
                    this.actors.Add(new Actor(this.UID, a.ID, a.FirstName, a.LastName, this.plot.dictPlot
                                    , this.output, this.plotSettings, files, this.traits_file));

            log.InfoFormat("DIRECTOR Victim {0}", this.settings.Victim);
            //set the actors for each actor
            foreach (Actor actor in this.actors)
            {
                actor.Actors = this.actors;
                log.InfoFormat("DIRECTOR adds actor {0}", actor.Name);
            }

            //at the start we initiate the 0 act
            this.currentAct = acts[0].Number;
            //the nr of unsatisfied goals
            this.nrUnsatisfiedGoals = acts[0].Goals.Count;
            //and retrive what current goals are.
            this.currentGoals = new Dictionary<string, DramaGoal>(acts[0].Goals);
        }
  
        private void writePlotToDB()
        {
            foreach (Subnet s in this.plot.dictPlot.Values)
            {
                string sql = "";
                

                foreach (Node n in s.Nodes.Values){
                    int idx = n.ID.IndexOf("_");
                    string variable = n.ID.Substring(idx + 1);
                    
                    //first check if the variable excists
                    sql = "select count(variableID) as c, variableID from ded_dpge_variable where name = '" + variable + "'";
                    MySqlDataReader reader = this.mysql.Query(sql);
                                       
                    int variableID = 0;
                    reader.Read();
                    int count = Convert.ToInt32(reader["c"]);
                    if (count > 0)
                        variableID = Convert.ToInt32(reader["variableID"]);                    
                    reader.Close();

                    int place = 0;
                    foreach ( string parent in n.Parents ){

                        idx = parent.IndexOf("_");
                        variable = parent.Substring(idx + 1);
                        //first get the parent ID
                        sql = "select count(variableID) as c, variableID from ded_dpge_variable where name = '" + variable + "'";
                        reader = this.mysql.Query(sql);

                        int parentID = 0;
                        reader.Read();
                        count = Convert.ToInt32(reader["c"]);
                        if (count > 0)
                            parentID = Convert.ToInt32(reader["variableID"]);
                        reader.Close();

                        //check if the entry exists
                        sql = string.Format("select count(parent_child_ID) as c from ded_dpge_parent_child where childID = {0} and  parentID = {1}",
                            variableID, parentID);
                        reader = this.mysql.Query(sql);

                        count = 0;
                        reader.Read();
                        count = Convert.ToInt32(reader["c"]);
                        reader.Close();
                        //next parent if it exists
                        if (count > 0 || variableID == 0 || parentID == 0) { ++place; continue; }
                        

                        //insert parent child 
                            sql = string.Format("insert into ded_dpge_parent_child (parentID,childID,place) values ({0},{1},{2})",
                                parentID, variableID, place);
                            this.mysql.Insert(sql);

                        ++place;
                    }
                }                
            }
        }

        public void StartGame() {
            foreach (Actor a in actors)
            {
                //create the knowledge base
                a.CreateActorKB();
                Thread t = new Thread(a.Act);
                this.actorThreads.Add( t );
                t.Name = a.FirstName;                
                t.Start();
            }
            //set the player of
            //StartPlayer();
            
            Run();

            foreach (Thread t in actorThreads)
            {
                log.Info("aborting thread " + t.Name);
                t.Abort();
            }
        }                

        
        private void SettingsToDB()
        {
            //first check if an entry for this player exists
            string sql = "select count(UID) as c from ded_DED_Settings where UID = " + this.UID;
            MySqlDataReader reader = this.mysql.Query(sql);
            reader.Read();
            int isEntry = Convert.ToInt32(reader["c"]);
            reader.Close();

            //If an entry exists then update it
            if (isEntry > 0) { 
                string str = "UPDATE ded_DED_Settings SET act = {0}, victim = '{1}', event = '{2}', isPlot = {3},";
 	            str += "isKnowledgebase = {4}, isCharacters = {5}, isNewgame = {6} WHERE UID = 4";
                sql = string.Format(str, this.settings.Act, this.settings.Victim, this.settings.Event,
                    this.settings.IsPlot, this.settings.IsKnowledgebase, this.settings.IsCharacters, this.settings.IsNewgame);
            }
            //else insert new
            else
            {
                string str = "insert into ded_DED_Settings (UID, act, victim, event, isPlot, isKnowledgebase, ";
                str += "isCharacters, isNewgame) values ({0},{1},'{2}','{3}',{4},{5},{6},{7})";
                sql = string.Format(str, this.UID, this.settings.Act, this.settings.Victim, this.settings.Event,
                    this.settings.IsPlot, this.settings.IsKnowledgebase, this.settings.IsCharacters, this.settings.IsNewgame);
            }
            this.mysql.Insert(sql); 
        }
     

        private void WriteGoalsToDB()
        {
            foreach (Deployed_goal g in this.deployed_goals)
            {
                //first check if the goal excist
                string sql = string.Format("select count(UID) as c from ded_DeployedGoal where data = '{0}' and UID = {1}",g.GoalName,this.UID);
                MySqlDataReader reader = this.mysql.Query(sql);

                reader.Read();
                int count = Convert.ToInt32(reader["c"]);
                reader.Close();

                if (count > 0 || g.GoalName == "") continue;                

                //insert the goal if it does not excist.
                sql = string.Format("insert into ded_DeployedGoal (UID,data) values ({0},'{1}')", this.UID, g.GoalName);
                this.mysql.Insert(sql); 

            }
        }
             
        private void FillMurderObject()
        {
            string sql = "SELECT DISTINCT concat( s.dpge_identifier, '_', v.name ) as variable, d.variable_description_ID, st.smile_position ";
            sql += "FROM ded_variable_description d ";
            sql += "JOIN ded_dpge_variable_state st ON st.stateID = d.stateID ";
            sql += "JOIN ded_dpge_variable v ON v.variableID = d.variableID ";
            sql += "JOIN ded_dpge_variable_subnet vs ON vs.variableID = d.variableID ";
            sql += "JOIN ded_dpge_subnet s ON s.subnetID = vs.subnetID ";

            MySqlDataReader reader = this.mysql.Query(sql);
            Dictionary<string, Dictionary<int, string>> variables = new Dictionary<string, Dictionary<int, string>>();
            while (reader.Read())
            {
                string variable = Convert.ToString(reader["variable"]);
                string variable_description_ID = Convert.ToString(reader["variable_description_ID"]);
                int stateID = Convert.ToInt32(reader["smile_position"]);
                if (!variables.ContainsKey(variable))
                    variables.Add(variable, new Dictionary<int, string>());
                if (!variables[variable].ContainsKey(stateID))
                    variables[variable].Add(stateID, variable_description_ID);
  
            }
            reader.Close();

            //Remeber to clear out of plottsettings first
            sql = string.Format("delete from ded_plotsettings where UID = {0}",
                    this.UID);
            this.mysql.Insert(sql);

            foreach (string variable in variables.Keys)
            {
                if (!this.plotSettings.Variables.ContainsKey(variable)) continue;
                int state = this.plotSettings.Variables[variable].State;
                if (!variables[variable].ContainsKey(state)) continue;
                string variable_description_ID = variables[variable][state];
                sql = string.Format("insert into ded_plotsettings (UID,variable_description_ID ) values ({0},{1})",
                    this.UID,variable_description_ID);
                this.mysql.Insert(sql);
            }
        }


        bool IsAllGoalsSatisfied() { if (this.nrUnsatisfiedGoals < 1) return true; return false; }

        /// <summary>
        /// the while true loop that runs the director
        /// </summary>
        void Run(){

            while (!this.STOP && !FinalActGoalsComplete()) { //100% 
                RevokeSchema(); //100%
                MoveBetweenActs(); //100%
                if ( !SchemaNeeded() ) continue; //66% need to work on defining satisfaction of goals
                PickSchema(); //95%
                DeploySchema(); //100%
                Thread.Sleep(Constants.SLEEP);
            }            
        }        

        /// <summary>
        /// Moves the drama between acts when all goals of the act have been satisfied.
        /// </summary>
        private void MoveBetweenActs()
        {
            if (this.IsAllGoalsSatisfied())
            {
                ++this.currentAct;
                this.nrUnsatisfiedGoals = this.acts[this.currentAct].Goals.Count;
                log.InfoFormat(this.UID + " - " + "Next ACT is '{0}'", this.currentAct);
                //set current goals
                this.currentGoals = new Dictionary<string, DramaGoal>(this.acts[this.currentAct].Goals);
                foreach (Actor a in this.actors) a.CurrentAct = this.currentAct;
                this.settings.Act = this.currentAct;
                SettingsToDB();

                if (this.currentAct == 1)
                {
                    this.settings.IsKnowledgebase = true;
                    this.settings.IsCharacters = true;
                    this.settings.IsNewgame = true;
                    SettingsToDB();
                }
            }
            
        }

        
        
        /// <summary>
        /// Revokes a faulty or complete schema from each actor.
        /// </summary>
        void RevokeSchema() {
            this.isRevoked = false;
            //RemoveDeadActors();
            foreach ( Schema schema in this.schemas.Values ) {
                //TODO revoke the schema if it is either faulty or the goals are complete.
                if (!schema.IsDeployed || 
                    (!schema.IsFaulty 
                    && !IsGoalsSatisfied(schema))) continue;
                //Reason for the faulty state perhaps?
                
                //Notify that a schema is revoked
                this.isRevoked = true;

                //Remove schema from the appropriate set of actors
                foreach (Actor a in this.actors)
                {
                    if (!schema.SchemaActors.ContainsKey(a.ID)) continue;
                    //the current actor revokes the schema 
                    a.RevokeSchema(schema);
                }
                schema.SchemaActors.Clear();
                schema.IsDeployed = false;
            }
        }

        private bool IsGoalsSatisfied(Schema schema)
        {
            
            if (schema.Goals.Count == 0) return false;

            //foreach (DramaGoal dg in this.assignedGoals.Values)
            //{
            //    if (!dg.IsSatisfied) continue;
            //}



            foreach (Actor a in this.actors)
            {
                List<AssignedGoal> removedAGs = new List<AssignedGoal>();
                foreach (Dictionary<string, AssignedGoal> dict in a.AssignedGoals.Values)
                {
                    foreach (AssignedGoal ag in dict.Values)
                    {
                        if (!this.currentGoals.ContainsKey(ag.DramaGoal.Name)) continue;
                        if (ag.DramaGoal.IsSatisfied)
                        {
                            //log.InfoFormat("{0}, Assigned goal is '{1}' and satisfied is '{2}', nrUnsatisfiedGoals is '{3}'"
                                 //, a.Name, ag.DramaGoal.Name, this.currentGoals[ag.DramaGoal.Name].IsSatisfied
                                 //, this.nrUnsatisfiedGoals);
                            SatisfyScemaPreconditions(ag);
                            //if the actors goal is satisfied then remove it                        

                            //if (ag.DramaGoal.Die == "") 
                            //removedAGs.Add(ag);
                            if ( !this.assignedGoals[ag.DramaGoal.Name].ContainsKey( ag.Schema.Name ) ) continue;
                            this.assignedGoals[ag.DramaGoal.Name][ag.Schema.Name].Remove(ag.Actor.Name);


                            //if the schema has no more unfulfilled goals, remove it
                            if (this.assignedGoals[ag.DramaGoal.Name][ag.Schema.Name].Count == 0)
                                this.assignedGoals[ag.DramaGoal.Name].Remove(ag.Schema.Name);                            

                            //mark the acts goals satisfied if appropriate
                            this.currentGoals[ag.DramaGoal.Name].UpdateAccumulatedValue(ag.Value);
                            //log.InfoFormat("{0}, Assigned goal is '{1}' and satisfied is '{2}', nrUnsatisfiedGoals is '{3}'"
                                 //, a.Name, ag.DramaGoal.Name, this.currentGoals[ag.DramaGoal.Name].IsSatisfied
                                 //, this.nrUnsatisfiedGoals);
                        }
                    }
                }

                //remove it from the actor to
                //foreach (AssignedGoal ag in removedAGs)
                //{
                //    a.AssignedGoals.Remove(ag.DramaGoal.Name);
                //}                            
            }
                        
            RemoveDeadActors();

            //now we can check if the schema is assigned to any goal and if it is then retun false
            foreach (Dictionary<string, Dictionary<string, AssignedGoal>> dramagoals in this.assignedGoals.Values)
            {
                if (dramagoals.ContainsKey(schema.Name) ) return false;
            }

            //return true, we found no unfullfilled goal
            return true;
        }

        private void RemoveDeadActors(){
            Actor actorToBRemoved = null;
            int counts = 0;
            foreach (DramaGoal g in this.currentGoals.Values)
            {
                if (g.Die != "" && g.IsSatisfied) foreach (Actor a in this.actors) if (a.Roles.ContainsKey(g.Die) && a.AssignedGoals.ContainsKey(g.Name))
                            actorToBRemoved = a;
                if (!g.IsSatisfied)
                {
                    ++counts;
                }
            }
            this.nrUnsatisfiedGoals = counts;
            if (actorToBRemoved != null)
            {
                this.actors.Remove(actorToBRemoved);
                actorToBRemoved.setSchemasFaulty();
                this.settings.Victim = this.plotSettings.Variables["V_name"].State_label;
                this.settings.Event = "e1";
                FillMurderObject();

                SettingsToDB();
                log.InfoFormat(this.UID + " - " + "Revoked actor '{0}'", actorToBRemoved.Name);
            }            
        }

        private void SatisfyScemaPreconditions(AssignedGoal ag)
        {
            foreach (Schema s in this.schemas.Values)
            {
                s.SatisfyPrecondition(ag.DramaGoal.Name, ag.Role.Name);
                s.SatisfyPrecondition(ag.Variable, ag.Role.Name);
            }
        }
            
        /// <summary>
        /// Filters out a set of applicaple schemas.
        /// </summary>
        void PickSchema(){
            this.applicableSchemas.Clear();
            this.optimalSchemas.Clear();

            foreach (Schema schema in this.schemas.Values)
            {
                if (IsDeployInstancesLeft(schema) 
                    && schema.IsAct(this.currentAct)
                    && IsUnAssignedGoals(schema)
                    && schema.IsPreconditionsSatisfied) this.applicableSchemas.Add(schema);
            }
            //Filter by extra roles Filter by character traits            
            foreach (Schema schema in this.applicableSchemas)
            {
                TotalDistance(schema);    
                //Add to the set of optimal schemas
                //first check whether the set is not empty and that the drama value is less which will replace the set of optimal schemas
                if (this.optimalSchemas.Count != 0 && this.optimalSchemas[0].DramaValue > schema.DramaValue)
                {
                    this.optimalSchemas.Clear();
                    this.optimalSchemas.Add(schema);
                }
                else this.optimalSchemas.Add(schema);
            }
        }

        /// <summary>
        /// Are there any schemas that still have instances left to deploy
        /// </summary>
        /// <param name="schema">schema</param>
        /// <returns></returns>
        bool IsDeployInstancesLeft(Schema schema) { if (schema.Instances == 0) return false; return true; }

        /// <summary>
        /// Are there any drama goals that have no assigned schemas.
        /// </summary>
        /// <param name="schema">schema</param>
        /// <returns></returns>
        bool IsUnAssignedGoals(Schema schema) { foreach (Goal goal in this.currentGoals.Values) 
            if (goal.Schemas.Count == 0) return true; return false; }

        /// <summary>
        /// The summed distance between role and characters
        /// </summary>
        /// <param name="schema"></param>
        void TotalDistance(Schema schema)
        {
            foreach (Role role in schema.EssentialRoles.Values)
            {
                //TODO trait distance
                //foreach (Actor actor in this.actors)
                //{
                //    schema.DramaValue += Euclidean(role.Traits, actor.Traits);
                //}
            }
            //Normalise
            if (schema.DramaValue != 0) schema.DramaValue = schema.DramaValue / schema.EssentialRoles.Count;
        }


        /// <summary>
        /// Calculates the Euclidean distance between Role Traits RT and Character Traits CT
        /// </summary>
        /// <param name="RT">Role Traits</param>
        /// <param name="CT">Character Traits</param>
        /// <returns>Total</returns>
        double Euclidean(Dictionary<string, Trait> RT, Dictionary<string, Trait> CT)
        {
            double sum = 0;
            foreach (string k in RT.Keys )
            {
                sum += Math.Pow(Math.Abs(RT[k].Value - CT[k].Value), 2);
            }
            return Math.Sqrt(sum);
        }

       
            
        /// <summary>
        /// Deploys a random applicable schema
        /// </summary>
        void DeploySchema(){
            //If there are no applicable schemas then exit
            if (this.optimalSchemas.Count == 0) return;

            //First get a random idx
            Random RandomClass = new Random();
            int rand = RandomClass.Next(0, this.optimalSchemas.Count);

            //Get the schema
            Schema schema = this.optimalSchemas[rand];
            
            AssignRoles(schema);
            this.deployedSchemas.Add(schema);

            //TODO give the schema to the appropriate set of actors
            int rank = 0;
            foreach (Actor a in this.actors)
            {
                if (!schema.SchemaActors.ContainsKey(a.ID)) continue;
                //the current actor receives the schema and role to play
                a.DeploySchema(schema, schema.SchemaActors[a.ID].Role, rank);
                AssignGoals(schema, a);
                a.AddToApplicableActions();
                ++rank;
                a.PropogateGoalsToOtherActors();
            }     
            
            //serialise goals
            foreach (SchemaGoal schemaGoal in schema.Goals.Values)
            {
                Deployed_goal dg = new Deployed_goal(schemaGoal.Variable, this.logical_time);
                ++this.logical_time;
                this.deployed_goals.Add(dg);
            }
            WriteGoalsToDB();

            //mark schema as deployed
            this.optimalSchemas[rand].IsDeployed = true;
            //decrament instanaces
            --this.optimalSchemas[rand].Instances;
        }

        

        /// <summary>
        /// Assignes goals to actors and attaches goals to dramagoals
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="a"></param>
        void AssignGoals(Schema schema, Actor a)
        {            
            List<SchemaGoal> goals = new List<SchemaGoal>();
            foreach (SchemaGoal schemaGoal in schema.Goals.Values)
            {                
                //only if actor has this roal?
                if (!a.Roles.ContainsKey(schemaGoal.Role.Name)) continue;
                //if the schema goal applies to ANY then it means that 
                //all actors should have such a goal for each of the other actors of same role
                if (schemaGoal.Applies == Constants.ANY_ROLE)
                    foreach (Actor each in this.actors)
                    {
                        //only if actor has this roal?
                        if (!each.Roles.ContainsKey(schemaGoal.Role.Name)) continue;

                        SchemaGoal goal = new SchemaGoal(schemaGoal);
                        if (each.Name == a.Name)
                        {
                            goal.Applies = Constants.SELF;
                            goal.Name = goal.Name + "_" + Constants.SELF;
                        }
                        else
                        {
                            goal.Applies = each.ID;
                            goal.Name = goal.Name + "_" + each.ID;
                        }

                        if (this.currentGoals.ContainsKey(schemaGoal.DramaGoal))
                        {
                            this.currentGoals[schemaGoal.DramaGoal].NumberOfActors += 1;
                            DramaGoal dg = this.currentGoals[schemaGoal.DramaGoal];
                            AssignedGoal ag = new AssignedGoal(dg, schema, goal, a);
                            
                            StoreAssignedGoals(ag);
                            a.AddGoal(ag);
                            //ensure that the actor has the goal
                            while (!a.hasGoal(ag)) { a.AddGoal(ag); }
                            ag.Name = dg.Name;
                            //this.player.AddGoal(ag);
                        }
                        //else { a.AddGoal(new AssignedGoal(goal, schema, a)); this.player.AddGoal(new AssignedGoal(goal, schema, a)); }
                    }
                else                
                    if (this.currentGoals.ContainsKey(schemaGoal.DramaGoal))
                    {
                        this.currentGoals[schemaGoal.DramaGoal].NumberOfActors += 1;
                        DramaGoal dg = this.currentGoals[schemaGoal.DramaGoal];
                        AssignedGoal ag = new AssignedGoal(dg, schema, schemaGoal, a);
                        //ag.Name = dg.Name;
                        StoreAssignedGoals(ag);
                        a.AddGoal(ag);
                        //ensure that the actor has the goal
                        while (!a.hasGoal(ag)) { a.AddGoal(ag); }
                    }
            }            
        }

        void StoreAssignedGoals(AssignedGoal ag)
        {
            if (!this.assignedGoals.ContainsKey(ag.DramaGoal.Name)) this.assignedGoals.Add(ag.DramaGoal.Name
                         , new Dictionary<string, Dictionary<string, AssignedGoal>>());
            if (!this.assignedGoals[ag.DramaGoal.Name].ContainsKey(ag.Schema.Name))
                this.assignedGoals[ag.DramaGoal.Name].Add(ag.Schema.Name, new Dictionary<string, AssignedGoal>());
            if (!this.assignedGoals[ag.DramaGoal.Name][ag.Schema.Name].ContainsKey(ag.Actor.Name))
                this.assignedGoals[ag.DramaGoal.Name][ag.Schema.Name].Add(ag.Actor.Name, ag);
        }
        

        //Todo complete assigning roles
        private void AssignRoles(Schema schema)
        {
            Dictionary<string, Actor> roleActors = new Dictionary<string, Actor>();
            //first set all actors in a dict
            foreach (Actor a in this.actors) roleActors.Add(a.ID, a);

            //todo First assign essential roles.
            //todo find the best fitting essential roles
            AssignEssentialRoles(schema, roleActors);

            //todo assign nonessential roles.
            //todo find the best fitting nonessential roles
            AssignNonEssentialRoles();            
        }

        int idx(int length, int count) { return length - count; }

        /// <summary>
        /// Assign essential roles. Find the best fitting essential roles
        /// </summary>
        /// <param name="schema"></param>
        private void AssignEssentialRoles(Schema schema, Dictionary<string, Actor> roleActors)
        {            
            //int length = this.actors.Count;
            
            //first assign all roles specifically taged to a name
            foreach (Role role in schema.PossibleRoles.Values)
            {
                if (role.Variable == "" || role.Subnet == "") continue;
                string actor = this.plotSettings.Variables[role.Variable].State_label;
                schema.SchemaActors.Add(roleActors[actor].ID, new SchemaActor(roleActors[actor].ID, schema.Name, role));
                roleActors.Remove(actor);
            }

            //Assign all essential roles less than max where 
            // + means at least one, so we first assign one and mark it non essential
            // * or all means ALL
            //number means that number must be assigned
                        
            foreach ( Role role in schema.PossibleRoles.Values ){
                if (!role.isEssential || !(role.Variable == "" || role.Subnet == "")) continue;

                // ALL, add all remaining actors and return
                if (role.MinNr == Constants.MAX)
                {
                    foreach (Actor a in roleActors.Values) 
                    {   
                        if (!schema.SchemaActors.ContainsKey(a.ID))
                            schema.SchemaActors.Add(a.ID, new SchemaActor(a.ID, schema.Name, role));
                    }
                    return;
                }

                //assign as many players as min number indicates
                int countActors = 0;
                foreach (Actor a in roleActors.Values)
                {
                    if (countActors >= role.MinNr) break;

                    if (!schema.SchemaActors.ContainsKey(a.ID)) 
                        schema.SchemaActors.Add(a.ID, new SchemaActor(a.ID, schema.Name, role));
                    ++countActors;
                }                
            }       
        }

        private void AssignNonEssentialRoles()
        {
            //TODO AssignNonEssentialRoles();
        }
        
            
        /// <summary>
        /// If this is the final act
        /// and the number of unsatisfied goals = 0 
        /// return true else false.
        /// </summary>
        /// <returns></returns>
        bool FinalActGoalsComplete() {
            if (this.acts.Count == 0) return false;
            if ( this.acts[this.acts.Count - 1].Number == this.currentAct
                && this.IsAllGoalsSatisfied()) return true;          
            return false;
        }
            

        /// <summary>
        /// Desides if there is a need to deploy a schema. IF no schema is deployed, IF there are goals that do not have schemas assigned to them,
        /// IF a schema has been revoked
        /// </summary>
        /// <returns></returns>
        bool SchemaNeeded() {
            //IF a schema has been revoked
            if ( this.isRevoked ) return true;
            //IF no schema is deployed
            if ( this.deployedSchemas.Count == 0 ) return true;
            //IF there are goals that do not have schemas assigned to them
            if (this.IsUnassignedGoals()) return true;

            return false;
        }

        bool IsUnassignedGoals() {  foreach (Goal goal in this.currentGoals.Values) if (goal.Schemas.Count == 0) return true; return false;   }
   
    }
}
