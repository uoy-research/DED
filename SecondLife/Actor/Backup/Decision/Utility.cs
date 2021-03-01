using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;
using log4net;
using log4net.Config;

namespace DED.Decision
{
    class Utility : Base_utility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Utility));
        public Utility() { XmlConfigurator.Configure(); }

        public void Evaluate( Knowledgebase kb, string opponent )
        {
            ActionFilter af = new ActionFilter();
            Dictionary<string, Relation> seeds = new Dictionary<string, Relation>();
            //seeds.Add("V_victims_sex",new Relation( "V_victims_sex","victim" ));
            foreach (String key in kb.Character.Keys)
            {
                foreach (Subnet subnet in kb.Character[key].Knowledges.Values)
                {
                    Dictionary<string, Relation> actions = af.FilterActionsBySeeds(kb.Character, new Relation(subnet.Name,key));
                    Dictionary<string, Dictionary<string, Relation>> reactions = new Dictionary<string, Dictionary<string, Relation>>();
                    foreach (Relation r in actions.Values)
                    {
                        reactions.Add(r.name, af.FilterActionsBySeeds(kb.Opponents[opponent], r));
                        
                    }
                    Reaction(kb, opponent, reactions);
                    
                    Action(kb, opponent, actions);
                    UpdateKnowledgeBase(kb,opponent);
                }
            }
        }

        public override List<Strategy> GetStrategies(Dictionary<string, ActorSubnet> kb, Subnet subnet, string state)
        { return new List<Strategy>(); }

        private void UpdateKnowledgeBase(Knowledgebase kb, string opponent)
        {
            //update opponent
            kb.Opponents[opponent][optimal.Choices[0].Subnet].Knowledges[optimal.Choices[0].Name].Valid = false;
        }

        void Reaction(Knowledgebase kb, string opponent, Dictionary<string, Dictionary<string, Relation>> reactions)
        {
            kb.Calculate();
            DrawReactionNodes();
            innerResults.Clear();
            Reactions.Clear();
            foreach ( string key in reactions.Keys )
            {
                Dictionary<string, Relation> actions = reactions[key];
                //contradiction
                Contradiction(kb, opponent, kb.Actor, actions);
                //risk
                Risk(kb, opponent, kb.Actor, actions);
                SetReactions(key);
            }
            log.Info("-------------Reactions------------------------");
            foreach (Optimal opt in Reactions.Values)
            {
                log.InfoFormat("Name:'{0}', Value:'{1}', Nr Choices:'{2}'", opt.Key, opt.Value, opt.Choices.Count);
            }
        }

        void Action(Knowledgebase kb, string opponent, Dictionary<string, Relation> actions) {
            DrawActionNodes();
            innerResults.Clear();
            //contradiction
            Contradiction(kb, opponent, kb.Actor, actions);
            //risk
            Risk(kb, opponent, kb.Actor, actions);
            SetActions();
            log.Info("-------------Actions------------------------");
            foreach (Choice c in optimal.Choices)
            {
                log.InfoFormat("Name:'{0}', State:'{1}', Position:'{2}', Value:'{3}'"
                    ,c.Name, kb.Character[c.Subnet].Knowledges[c.Name].Nodes[c.Name+'_'+Constants.PREMISE].States[c.State], c.Position,c.Value);
            }
            Choice say = optimal.Choices[0];
            log.InfoFormat("\n#########################################################");
            log.InfoFormat("Name:'{0}', State:'{1}', Position:'{2}', Value:'{3}'"
                , say.Name, kb.Character[say.Subnet].Knowledges[say.Name].Nodes[say.Name + '_' + Constants.PREMISE].States[say.State], say.Position, say.Value);
            log.InfoFormat("\n#########################################################\n");
        }
    }
}
