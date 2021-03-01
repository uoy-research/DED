using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;


namespace DED.NPC.Actions
{
    class ActInitPosition : Action
    {        
        public ActInitPosition(Actor actor)
            : base(actor, "initPosition", 0) { }

        public override void act()
        {
            base.ToPosition("61.143.25");
            base.ToPosition("91.138.26");
            base.ToPosition(base.actor.InitPosition);
        }
    }
}
