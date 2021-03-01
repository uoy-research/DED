using System;
using System.Collections.Generic;
using System.Text;
using DED.Utils;
using DED.NPC;
using DED.Director;


namespace DED.Decision
{
    class UtilityGreaterVariable : Base_utility
    {
        ContextSubnet contextSubnet;
        string variable;

        public UtilityGreaterVariable(ContextSubnet contextSubnet, string variable) {
            this.contextSubnet = contextSubnet;
            this.variable = variable;
        }

        public override List<Strategy> GetStrategies(Goal goal)
        {
            //First determine which knowledges will increase the variable and by how much
            ActionFilter af = new ActionFilter();
            List<Strategy> strategies = new List<Strategy>();
            strategies = af.GreaterVariable(this.contextSubnet, this.variable, goal); 

            return strategies;
            //TODO then store them in a dict where they can be retrieved again.
        }
    }

    class UtilityGreaterDirectSingleVariable : Base_utility
    {
        ContextSubnet contextSubnet;
        string variable;

        public UtilityGreaterDirectSingleVariable(ContextSubnet contextSubnet, string variable)
        {
            this.contextSubnet = contextSubnet;
            this.variable = variable;
        }

        public override List<Strategy> GetStrategies(Goal goal)
        {
            //First determine which knowledges will increase the variable and by how much
            ActionFilter af = new ActionFilter();
            List<Strategy> strategies = new List<Strategy>();
            strategies = af.GreaterDirectSingleVariable(this.contextSubnet, this.variable, goal);

            return strategies;
            //TODO then store them in a dict where they can be retrieved again.
        }
    }

}
