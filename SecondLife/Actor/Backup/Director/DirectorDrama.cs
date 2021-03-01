using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.DPGE;
using DED.NPC;
using DED.NPC.Actions;
using log4net;
using log4net.Config;

namespace DED.Director
{
    struct Act
    {
        public Act(List<Goal> goals, int number)
        { 
            this.goals = goals;
            this.number = number;
        }

        List<Goal> goals;
        public List<Goal> Goals { get { return this.goals; } }

        int number;
        public int Number { get { return this.number; } }
    }

    class DirectorDrama
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectorDrama));
        Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
        List<Schema> applicableSchemas = new List<Schema>();
        List<Actor> actors = new List<Actor>();
        List<Thread> actorThreads = new List<Thread>();
        List<Act> acts = new List<Act>();
        int currentAct;
        int nrUnsatisfiedGoals;
        Plot plot;
        Dictionary<string, Subnet> k = new Dictionary<string,Subnet>();
        List<Goal> goals;

        public DirectorDrama() { XmlConfigurator.Configure(); }

        public void Init(string schemafile, string avatarfile, string plotfile, string dramafile)
        {
            log.Info("Creating plot");
            plot = new Plot(plotfile);
            this.plot.DrawNet();
            this.plot.WritePlot();

            log.Info("Reading " + avatarfile);
            string s = System.IO.File.ReadAllText(avatarfile);

            string[] aText = s.Split(';');
            
            foreach (string line in aText)
            {
                string[] aSplit = line.Split(',');
                if (aSplit.Length == 4) this.actors.Add(new Actor(aSplit[0].Trim(), aSplit[1].Trim(), aSplit[2].Trim(), aSplit[3].Trim(), plot.dictPlot, this.actors));
            }

            log.Info("Reading " + dramafile);
            string dstr = System.IO.File.ReadAllText(dramafile);

            string[] adrama = dstr.Trim().Split(';');

            foreach (string line in adrama)
            {
                
                string[] aSplit = line.Split(':');
                if ( aSplit.Length < 3 ) continue;
                string[] aGoals = aSplit[2].Trim().Split(',');
                List<Goal> goals = new List<Goal>();
                foreach (string name in aGoals)
                {
                    goals.Add(new Goal(name, null, null, null, null, null, null, null, null));
                }                            
                
                if (aSplit.Length == 3) acts.Add(new Act(goals, Convert.ToInt32(aSplit[1].Trim())));                
            }

            log.Info("Reading " + schemafile);
            s = System.IO.File.ReadAllText(schemafile);

            foreach ( string str in s.Trim( ).Split( ';' ) )
            {
                if (str.Length == 0) continue;
                Schema schema = new Schema();
                schema.Init(str.Trim());
                schemas.Add(str.Trim(), schema);
            }
            log.Info("Schemas are read");

            foreach (Actor a in actors)
            {
                Thread t = new Thread(a.Act);
                actorThreads.Add( t );
                t.Name = a.FirstName;
                t.Start();
            }
            Run();
        }

        void Run(){
            this.currentAct = acts[0].Number;
            this.nrUnsatisfiedGoals = acts[0].Goals.Count;
            this.goals = acts[0].Goals;

            while (true) {
                if ( FinalActGoalsComplete() ) break;
                RevokeSchema();
                MoveBetweenActs();
                if ( !SchemaNeeded() ) continue;
                PickSchema();
                DeploySchema();
                Thread.Sleep(50000);
            }

            foreach (Thread t in actorThreads)
            {
                log.Info("aborting thread " + t.Name);
                t.Abort();   
            }
        }

        private void MoveBetweenActs()
        {
            if (this.nrUnsatisfiedGoals == 0) ++this.currentAct;
        }
        
        void RevokeSchema() {
            //Check deployed schemas whether they are marked as faulty.
            //Reason for the faulty state perhaps?
            //Schema goals are complete
        }
            
        void PickSchema(){
            this.applicableSchemas.Clear();
            foreach (Schema schema in this.schemas.Values)
            {
                if (!schema.IsDeployed && schema.IsAct(this.currentAct)) this.applicableSchemas.Add(schema);
            }
            //Filter by precondition
            //Filter by unfulfilled goals
            //Filter by Characteristics
        }
            
        void DeploySchema(){
            if ( this.applicableSchemas.Count == 0 ) return;
            foreach (Actor a in this.actors)
            { a.DeploySchema(this.applicableSchemas[0], this.applicableSchemas[0].Role["actor"]); }
            this.applicableSchemas[0].IsDeployed = true;
        }
            
        bool FinalActGoalsComplete() {
            if ( this.acts[this.acts.Count - 1].Number == this.currentAct
                && this.nrUnsatisfiedGoals == 0 ) return true;          
            return false;
        }
            
        bool SchemaNeeded() {
            //Unfulfilled goals that do not have schemas assigned to them.
            return true;
        }

        //void Run()
        //{
        //    log.Info("Deploying schema");


        //    foreach (Actor actor in actors)
        //    {
        //        actor.CreateActor(p.dictPlot, actors, actor.FirstName, actor.LastName);
        //        actor.Login();
        //    }
        //    //Deploy schema
        //    //Schema dSchema = new Schema();
        //    //if (schemas.TryGetValue("Interrogation.txt", out dSchema))
        //    //{
        //    //    log.Info("Deploying Interrogation.txt");
        //    //    characters[0].InitializeSchema( dSchema, dSchema.PossibleRoles[0] );
        //    //    characters[1].InitializeSchema( dSchema, dSchema.PossibleRoles[0] );
        //    //    dSchema.Role.Add(dSchema.PossibleRoles[0]);
        //    //} else { log.Info("Deployment of schema failed"); }

        //    //characters[0].Act();

        //    //log.Info("Goodby cruel world, I'm  leaving you today!");
        //    //string read = Console.Read().ToString();
        //}
    }
}
