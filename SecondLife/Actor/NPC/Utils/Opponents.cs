using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DED.NPC.Utils
{
    class Opponent{
        public Opponent() { }
        public Opponent(Actor a) { }
    }

    class Opponents
    {
        Dictionary<string, Opponent> actors = new Dictionary<string, Opponent>();
        Dictionary<string, Opponent> players = new Dictionary<string, Opponent>();
        public Opponents(List<Actor> actors) {
            foreach (Actor a in actors) this.actors.Add(a.Name,new Opponent(a));            
        }

        public bool IsPlayer(String name){
            if ( actors.ContainsKey(name)) return false;
            if ( !players.ContainsKey(name) ) players.Add(name, new Opponent());
            return true;
        }
    }
}
