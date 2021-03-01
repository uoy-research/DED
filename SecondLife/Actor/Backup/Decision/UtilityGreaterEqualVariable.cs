using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;


namespace DED.Decision
{
    class UtilityGreaterEqualVariable : Base_utility
    {
        public UtilityGreaterEqualVariable(){}
        public override List<Strategy> GetStrategies(Dictionary<string, ActorSubnet> kb, Subnet subnet, string state)
        {
            //First determine which knowledges will increase the variable and by how much
            ActionFilter af = new ActionFilter();
            List<Strategy> strategies = new List<Strategy>();
            if (state != "*" && state != "self") { return af.GreaterOrEqualVariable(kb,subnet, Convert.ToInt32(state)); }

            for (int i = 0; i < subnet.Net.GetOutcomeCount(subnet.Name + '_' + Constants.PREMISE); i++)
            {
                strategies.AddRange(af.GreaterOrEqualVariable(kb, subnet, Convert.ToInt32(i)));
            }

            return strategies;
            //then store them in a dict where they can be retrieved again.
        }
    }
}
