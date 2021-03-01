using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace DED.NPC.Actions
{
    class ActLogin : Action
    {

        
        public ActLogin(Actor actor)
                : base(actor, "login", 0) { }

        public override void act()
        {
            if (base.Client.Network.Login(base.actor.FirstName, base.actor.LastName, base.actor.Password, "", base.actor.StartLocation, ""))
            { log.InfoFormat("{0} {1}I logged into Second Life!", base.actor.FirstName, base.actor.LastName); }
            else { log.InfoFormat("I couldn't log in, here is why: " + base.Client.Network.LoginMessage); }      
        }
        
    }
}
