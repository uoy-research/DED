using System;
using System.Collections.Generic;
using System.Text;
using DED.Decision;


namespace DED.NPC.Actions
{
    class ActCreateKnowledgebase : Action
    {
        public ActCreateKnowledgebase(Actor actor)
            : base(actor, "createknowledgbase", 0) { }

        public override void act()
        {
            base.log.Info(base.actor.FirstName+" Creating knowledgebase ");
            base.actor.KnowledgeBase.CreateKnowledgeBase(base.actor.Plot, base.actor.Actors);
            base.actor.Utilities.Add(">=", new UtilityGreaterEqualVariable());
            base.actor.Utilities.Add("<", new UtilityLessVariable());
            base.actor.Perception = new Perception();
            base.log.Info(base.actor.FirstName + " knowledgebase created");
        }
    }
}
