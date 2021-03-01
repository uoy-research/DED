using System;
using System.Collections.Generic;
using System.Text;

namespace DED.NPC
{
    class Perception
    {
        //actorname, actionname, action
        Dictionary<string, Dictionary<string, Action>> actions = new Dictionary<string, Dictionary<string, Action>>();

        public Perception()
        {        }

        public void AddAction( Action a ){
            //if (!actions.ContainsKey(a.Client.Self.FirstName)) actions.Add(a.Client.Self.FirstName, new Dictionary<string, Action>());

            //actions[a.Client.Self.FirstName].Add(a.ActionName, a);
        }
    }
}
