using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using DED.NPC;
using DED.NPC.Actions;
using log4net;
using log4net.Config;

namespace DED.Utils
{
    public class SLInterface  
    {       

        /// <summary>
        /// Second Life First name
        /// </summary>
        string first_name;
        /// <summary>
        /// Second Life last name
        /// </summary>
        string last_name;

        /// <summary>
        /// Second Life first name + last name
        /// </summary>

        public string FirstName { get { return this.first_name; } set { this.first_name = value; } }
        /// <summary>
        /// Second Life last name
        /// </summary>
        public string LastName { get { return this.last_name; } set { this.last_name = value; } }
        /// <summary>
        /// The actors in the drama
        /// </summary>
       

        private static readonly ILog log = LogManager.GetLogger(typeof(SLInterface));

        Actor actor;
        public Actor ActorNPC { set { this.actor = value; } }
        /// <summary>
        /// Second Life First name
        /// </summary>
        List<Actor> actors = new List<Actor>();
        /// <summary>
        /// The actors in the drama
        /// </summary>
        public virtual List<Actor> Actors { get { return this.actors; } set { this.actors = new List<Actor>(value); } }

        List<Actor> deadActors = new List<Actor>();
        /// <summary>
        /// The actors in the drama
        /// </summary>
        public List<Actor> DeadActors { get { return this.deadActors; } set { this.deadActors = new List<Actor>(value); } }

        public SLInterface() { }

        public SLInterface(string first_name, string last_name)
        {
            XmlConfigurator.Configure();
            this.first_name = first_name;
            this.last_name = last_name;                  
        }                
    }
}
