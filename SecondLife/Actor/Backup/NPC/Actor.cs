using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.DPGE;
using DED.Director;
using DED.Decision;
using DED.NPC.Actions;
using log4net;
using log4net.Config;
using libsecondlife;
using System.Threading;


namespace DED.NPC
{
    class Actor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Actor));
        string first_name;
        string last_name;
        string password = "";
        public string Name { get { return this.first_name + ' ' + this.last_name; } }
        public string FirstName { get { return this.first_name; } }
        public string LastName { get { return this.last_name; } }
        public string Password { get { return this.password; } }
        List<Actor> actors = new List<Actor>();
        public List<Actor> Actors { get { return this.actors;  } }
        List<Actor> actorsOnScene = new List<Actor>();
        Knowledgebase kb;
        public Knowledgebase KnowledgeBase { get { return this.kb; } }
        Dictionary<string, Role> roles = new Dictionary<string, Role>();
        Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
        Dictionary<string, Goal> goals = new Dictionary<string, Goal>();
        Dictionary<string, Base_utility> utilities = new Dictionary<string, Base_utility>();
        public Dictionary<string, Base_utility> Utilities { get { return this.utilities; } }
        Perception perception;
        public Perception Perception { get { return this.perception; } set { this.perception = value; } }
        string initPosition;
        public string InitPosition { get { return this.initPosition; } }
        Dictionary<string, Subnet> plot;
        public Dictionary<string, Subnet> Plot{ get { return this.plot; } }
        List<Director.Action> actions = new List<Director.Action>();
        List<Director.Action> applicableActions = new List<Director.Action>();
        Dictionary<string, Action> slActions = new Dictionary<string, Action>();

        public SecondLife client = null;
        public SecondLife Client { get { return this.client; } }
        string startLocation = NetworkManager.StartLocation("University of York", 94, 88, 27);
        public string StartLocation { get { return this.startLocation; } }

        public Actor(string first_name, string last_name, string password, string initPosition, Dictionary<string, Subnet> plot, List<Actor> actors)
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.password = password;
            this.initPosition = initPosition;
            if (this.client != null) this.client = null;
            this.client = new SecondLife();
            this.actors = actors;
            this.plot = plot;
            kb = new Knowledgebase(this.first_name);
            this.client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            XmlConfigurator.Configure();
            initSLActions();
        }
        

        //public void Login()
        //{
        //    if (this.client.Network.Login(this.first_name, this.last_name, this.password, "My First Bot", this.startLocation, "Your name"))
        //    {
        //        log.InfoFormat("{0} {1}I logged into Second Life!", first_name, last_name);
        //        //InitPosition();
        //        //CreateActor();
        //    }
        //    else
        //    {
        //        log.InfoFormat("I couldn't log in, here is why: " + this.client.Network.LoginMessage);
        //    }            
        //}              

        public void CreateActor()
        {          
            kb.CreateKnowledgeBase(this.plot, this.actors);
            utilities.Add(">=", new UtilityGreaterEqualVariable());
            utilities.Add("<", new UtilityLessVariable());
            this.perception = new Perception();
        }
        
        public void Act()
        {
            try
            {
                while (true)
                {
                    this.applicableActions.Clear();
                    if (this.actions.Count < 1) continue;
                    Director.Action action = null;
                    //log.InfoFormat("{0}, applicable actions {1}", this.first_name, this.applicableActions.Count);
                    PickAction(out action);
                    
                    if (this.applicableActions.Count == 0)
                    {
                        //log.InfoFormat("{0}, No applicable actions", this.first_name);
                        Thread.Sleep(50000);
                        if (this.KnowledgeBase != null) log.Warn(this.first_name+" knwledgebase sise " + this.KnowledgeBase.Character.Count);
                        else log.Warn(this.first_name+" knwledgebase NULL ");
                        continue;
                    }
                    if (action != null ) log.InfoFormat("{0}, contmplates action {1} applicable actions {2}",this.first_name, action.Name, this.applicableActions.Count);
                    
                    Execute(action);
                    UpdateExecutedActions(action);
                    Thread.Sleep(10000); 
                }
            }
            finally
            {
                log.InfoFormat("{0} Logging out", this.FirstName);
                this.client.Network.Logout();
            }            
        }

        private void UpdateExecutedActions(Director.Action action)
        {
            if (action == null) return;
            foreach ( Director.Action a in this.actions)
            {
                log.InfoFormat("Actor {1}, updating action {4} for action name {2} action nr executions {0} is precon satisfied {3}"
                    , a.NrExecutions, this.first_name, a.Name, a.IsPreconditionsSatisfied, action.Name);
                if (action.Name == a.Name)
                {
                    a.ActionExecuted();
                }

                a.SatisfyPrecondition(action.Name);                
            }
            
        }

        private void Execute(Director.Action action)
        {
            if (action == null) return;
            this.slActions[action.Name].act();
        }

        private void PickAction(out Director.Action action)
        {
            foreach (Director.Action a in this.actions)
            {
                if (a.NrExecutions > 0 && a.IsPreconditionsSatisfied) this.applicableActions.Add(a);
            }
            if (this.applicableActions.Count > 0)
            {
                Random RandomClass = new Random();
                int rand = RandomClass.Next(0, this.applicableActions.Count);
                action = this.applicableActions[rand];
            }
            else action = null;
        }        

        private void NotifyOthers(Action a)
        {
            foreach (Actor actor in actorsOnScene)
            { actor.AddStimuly(a); }
        }

        public void AddStimuly(Action a)
        {
            this.perception.AddAction(a);
        }

        private void Network_OnConnected(object sender)
        {
            client.Appearance.SetPreviousAppearance(false);
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            log.InfoFormat("{0} {1} connected to the simulator", this.FirstName, this.LastName);
            client.Self.Chat("Hello World!", 0, ChatType.Normal);
        }

        private void Self_OnInstantMessage(InstantMessage im, Simulator sim)
        {
            string[] friends = null;
            IMHandler imhandler = new IMHandler(client, im, sim, friends);

            switch (im.Dialog)
            {
                case InstantMessageDialog.MessageFromAgent:
                    imhandler.RespondToMessageFromAgent();
                    break;
                case InstantMessageDialog.FriendshipOffered:
                    imhandler.AcceptFriendshipOffer();
                    break;
                case InstantMessageDialog.GroupInvitation:
                    imhandler.AcceptGroupInvitation();
                    break;
                case InstantMessageDialog.InventoryOffered:
                    imhandler.AcceptInventoryOffered();
                    break;
                // someone sent a teleport lure
                case InstantMessageDialog.RequestTeleport:
                    imhandler.AcceptTeleportRequest();
                    break;
                default:
                    break;
            }
        }

        public void DeploySchema(Schema schema, Role role)
        {
            log.InfoFormat("{0} received schema '{1}'", this.first_name, schema.Name);
            roles.Add(role.Name, role);
            foreach (Director.Action a in role.Actions)
            {
                actions.Add(new Director.Action(a));
            }
            schemas.Add(schema.Name, schema);
            foreach (Goal g in schema.Goals)
            {
                this.goals.Add(g.Name, g);
            }
        }

        private void initSLActions()
        {
            this.slActions.Add("login",new ActLogin(this));
            this.slActions.Add("initPosition", new ActInitPosition(this));
            this.slActions.Add("createknowledgbase", new ActCreateKnowledgebase(this));
        }
    }
}


//Check for new actions, determine importance and categorize.
//start new threads if needed to process the information.

//serach for viable actions
//foreach (Goal g in goals.Values)
//{   
//    List<Strategy> strategies = Utilities[g.Aim].GetStrategies(kb.Character, kb.Character[g.Submodel].Knowledges[g.Variable], g.State);
//    foreach (Strategy s in strategies)
//    {
//        log.InfoFormat("goal {3}, State {4}, Strategy {0}, State {2}, Value {1}", s.Variable, s.Value, s.State, g.Variable, g.State);
//    }
//    //a = new Action(first_name, strategies[0].Variable + ' ' + strategies[0].State, strategies[0].Value);
//}

//take action. we may want to perform a schema of actions
//ExecuteAction(a);
//NotifyOthers(a);