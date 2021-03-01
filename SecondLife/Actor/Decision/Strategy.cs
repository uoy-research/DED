using System;
using System.Collections.Generic;
using System.Text;
using DED.NPC;
using DED.Utils;
using DED.Utils.Reads;
using DED.DPGE;
using DED.Director;
using DED.Decision;
using DED.NPC.Actions;
using log4net;
using log4net.Config;
using System.Threading;

namespace DED.Decision
{
    class OptimalStrategy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OptimalStrategy));
        Constants constants = new Constants();

        public OptimalStrategy() { XmlConfigurator.Configure(); }


        private List<Strategy> GetSpeechResponce(DEDAction sender_action, string sender_name, Base_utility util, Actor actor)
        {
            //log.InfoFormat("({4}) GetSpeechResponce: Received action '{0}', state '{1}', sender '{2}', Addressed to '{3}'"
            //    , sender_action.Variable, sender_action.State, sender_name, sender_action.AddressedTo, actor.Name);
            List<Strategy> responces = new List<Strategy>();
            //Find all aims and collect all variables that satisfy them.

            string subnetname = this.constants.SUBNET(sender_action.Variable);

            Dictionary<string, Strategy> goalStrategies = new Dictionary<string, Strategy>();
            //get goals

            foreach (AssignedGoal goal in actor.CurrentGoals.Values)
            {
                if (goal.Variable == "") continue;
                foreach (Strategy s in util.GetStrategies(goal)) responces.Add(s);
                //goalStrategies.Add(s.Variable+s.State,s);
            }

            return responces;
        }

        
        public void EvaluateResponces(Conversation conv, Actor actor)
        {
            
            //log.InfoFormat("({4}) EvaluateResponces: Received action '{0}', state '{1}', sender '{2}', Addressed to '{3}'"
            //    , conv.StimulyVariable.Name, conv.StimulyVariable.State, conv.Actor, conv.Action.AddressedTo, actor.Name);
            
            // If the sender action was a question or an accusation 
            if (conv.Action.IsQuestion) { RespondToQuestion(conv.Action, conv.Actor, actor); return; }
            DirectSpeech(conv, actor);
            return;
        }

        private void DirectSpeech(Conversation conv, Actor actor)
        {
            Dictionary<string, AssignedGoal> goals = setDirectActionGoals(conv.Action, actor);
            UtilityDirectSpeech utility = new UtilityDirectSpeech(actor.KnowledgeBase, conv.StimulyVariable);
            List<Strategy> speech_actions = utility.GetStrategies(null);
            DEDAction action = getRandomAction(conv.Action, speech_actions, actor, conv.Actor, goals);

            if (action != null)
            {
                actor.SetForExecution(conv, action);
            }
            return;
        }

        private Dictionary<string, AssignedGoal> setDirectActionGoals(DEDAction action, Actor actor)
        {
            Dictionary<string, AssignedGoal> goals = new Dictionary<string, AssignedGoal>();
            foreach (Goal g in action.Goals.Values)
            {
                log.InfoFormat("({0}) 1 action goal '{1}'", actor.Name, g.Name);
                foreach (Dictionary<string, AssignedGoal> dict in actor.AssignedGoals.Values)
                {
                    if (dict.ContainsKey(g.Name) && !goals.ContainsKey(g.Name))
                    {
                        log.InfoFormat("({0}) 2 action goal '{1}'", actor.Name, g.Name);
                        //if (dict[g.Name].Role.Name != g.Role.Name) continue;
                        goals.Add(g.Name, dict[g.Name]);
                        goals[g.Name].Value = g.Value;
                    }
                    //foreach (Goal gg in dict.Values) log.InfoFormat("({0}) 3 action goal '{1}'", actor.Name, gg.Name);
                }
            }
            return goals;
        }

        public DEDAction Say(DEDAction base_action, Actor actor)
        {
            Variable variable = new Variable(base_action.Variable, base_action.SubnetName, base_action.State);
            Dictionary<string, AssignedGoal> goals = setDirectActionGoals(base_action, actor);
            UtilityDirectSpeech utility = new UtilityDirectSpeech(actor.KnowledgeBase, variable);
            List<Strategy> speech_actions = utility.GetStrategies(null);

            DEDAction action = getRandomAction(base_action, speech_actions, actor, base_action.AddressedTo, goals);
            //if (action != null) actor.SetForExecution(action);
            return action;
        }                

                
        public void RequestResponce(DEDAction sender_action, string sender, Actor actor)
        {
            log.InfoFormat("({4}) RequestResponce: Received action '{0}', state '{1}', sender '{2}', Addressed to '{3}'"
                , sender_action.Variable, sender_action.State, sender, sender_action.AddressedTo, actor.Name);
            sender_action.TalkedAbout = sender;
            FindSpeechGoalsForSender(sender_action, sender, actor);
            
            string knowledge = this.constants.SUBNET(sender_action.Variable);
            List<Strategy> responces = new List<Strategy>();

            Dictionary<string, Strategy> goalStrategies = new Dictionary<string, Strategy>();

            foreach (Strategy s in GetSpeechResponce(sender_action, sender
                , new UtilityGreaterDirectSingleVariable(actor.KnowledgeBase.GetSubnet(knowledge), sender_action.Variable), actor))
                goalStrategies.Add(s.Variable + s.State, s);


            //Query knowledge base for direct actions that will satisfy the traits.
            IntegrityUtility integrity = new IntegrityUtility(actor.KnowledgeBase, actor.KnowledgeBase.GetSubnet(knowledge), sender_action.Variable);
            integrity.IntegrityValues = actor.IntegrityValues;

            //Merge the values and return the set of optimal strategies
            //FindOptimalStatement(responces, integrity.GetDirectStrategies(), goalStrategies);
            FindOptimalStatement(responces, goalStrategies);

            //map the strategy to action
            DEDAction action = getRandomAction(sender_action, responces, actor, sender, actor.CurrentGoals);

            if (action == null) return;
            action.AddressedTo = sender;

            AddContributingArguments(action, sender, knowledge, actor, sender, Constants.OTHER);

            log.InfoFormat("{0}, Responds to request, msg '{1}', variable '{2}', state '{3}', subnet '{4}' ", actor.Name, action.Msg, action.Variable, action.State, action.SubnetName);
            action.Target = Constants.OTHER;
            actor.SetForExecution(action);
        }

        private void RespondToQuestion(DEDAction sender_action, string sender_name, Actor actor)
        {
            log.InfoFormat("({4}) Respconce to question: Received action '{0}', state '{1}', sender '{2}', Addressed to '{3}'"
                , sender_action.Variable, sender_action.State, sender_name, sender_action.AddressedTo, actor.Name);
            //Find the goals that the character has
            if (sender_action.TalkedAbout == null || sender_action.TalkedAbout == "") FindSpeechGoalsForSelf(actor, sender_action);
            else FindSpeechGoalsForSender(sender_action, sender_action.TalkedAbout, actor);

            List<Strategy> responces = new List<Strategy>();
            //The knowledge that sender action is questioning
            string knowledge = this.constants.SUBNET(sender_action.Variable);

            Dictionary<string, Strategy> goalStrategies = new Dictionary<string, Strategy>();

            //Query knowledge base for direct actions that will satisfy the goals.            
            UtilityGreaterDirectSingleVariable util = new UtilityGreaterDirectSingleVariable(actor.KnowledgeBase.GetSubnet(sender_action.SubnetName), sender_action.Variable);
            
            //get goals
            foreach (AssignedGoal goal in actor.CurrentGoals.Values)
            {
                if (goal.Variable == "") continue;
                foreach (Strategy s in util.GetStrategies(goal)) if (!goalStrategies.ContainsKey(s.Variable + s.State)) goalStrategies.Add(s.Variable + s.State, s);
            }

            log.InfoFormat("({0}) Respconce to question: Goal Strategies '{1}', current goals {2}", actor.Name, goalStrategies.Count, actor.CurrentGoals.Count);


            //Query knowledge base for direct actions that will satisfy the traits.
            IntegrityUtility integrity = new IntegrityUtility(actor.KnowledgeBase, actor.KnowledgeBase.GetSubnet(knowledge), sender_action.Variable);
            integrity.IntegrityValues = actor.IntegrityValues;

            //Merge the values and return the set of optimal strategies
            //FindOptimalStatement(responces, integrity.GetDirectStrategies(), goalStrategies);
            FindOptimalStatement(responces, goalStrategies);

            log.InfoFormat("({0}) Respconce to question: Responces {1} and Goal Strategies '{2}'", actor.Name, responces.Count, goalStrategies.Count);

            //map the strategy to action
            //DEDAction action = getRandomAction(sender_action, responces, actor, sender_name, actor.CurrentGoals);
            DEDAction action = getRandomAction(sender_action, responces, actor, sender_name, actor.CurrentGoals);
            if (action == null) return;

            //add necessary flaggs
            action.Rank = Constants.RESPONCE_FLAGG;

            //Todo set the goal that the action satisfies
            if (!action.IsDenial)
            {
                //action.Goals["character_motive"].State = "0";
                AddContributingArguments(action, sender_name, knowledge, actor, Constants.USER, Constants.SELF);
            }
            action.Responce_to_variable = action.Variable;
            action.Target = Constants.SELF;
            actor.SetForExecution(action);
            return;
        }

        private DEDAction getRandomAction(DEDAction action, List<Strategy> speech_actions, Actor actor, string addressedTo, Dictionary<string, AssignedGoal> goals)
        {
            log.InfoFormat("Enter get random action, ({3}) getRandomAction: Received action '{0}', state '{1}', Addressed to '{2}'"
                , action.Variable, action.State, addressedTo, actor.Name);
            if (speech_actions.Count < 1) return null;
            Random rand = new Random();
            int idx = rand.Next(speech_actions.Count - 1);
            Strategy strategy = speech_actions[idx];

            PrintActions(speech_actions, actor.Name);

            foreach (AssignedGoal g in goals.Values)
            {
                log.InfoFormat("Goals in random action, ({0}), assigned goal '{1}', state '{2}', value '{3}'"
                , actor.Name, g.Variable, g.State, g.Value );
                g.Value = 10;
            }

            string victimName = actor.PlotSettings.Variables["V_name"].State_label;

            DEDAction a = new DEDAction(actor.LogicalTime(), strategy, actor.getActionSchemas(action)
                    , Convert.ToDouble(strategy.Value), goals, actor.Output, addressedTo, action.Traits
                    , victimName, action.TalkedAbout);

            log.InfoFormat("Decision in get random action: ({3}), action '{0}', state '{1}', Addressed to '{2}'"
                , a.Variable, a.State, a.AddressedTo, actor.Name);
            if (a.Msg == null) a.Msg = " say ";
            return a;
        }

        private bool isOtherID(string applies, string actorID)
        {
            double Num;
            bool isNum = double.TryParse(applies, out Num);
            if (isNum) return (applies != actorID);
            return false;
        }
 
        private void FindSpeechGoalsForSender(DEDAction sender_action, string sender, Actor actor)
        {
            actor.CurrentGoals.Clear();
            //log.InfoFormat("Goal, actor '{0}', sender '{1}', action '{2}', action state '{3}' ", actor.Name, sender, sender_action.Variable, sender_action.State);


            foreach (Dictionary<string, AssignedGoal> dict in actor.AssignedGoals.Values)
            {
                foreach (AssignedGoal goal in dict.Values)
                {
                    if (goal.Variable == "") continue;
                    //log.InfoFormat("Find Goal For Sender actor '{0}', sender '{1}', goal '{2}', goal state '{3}', applicable '{4}', goal name '{5}', goal.Owner '{6}' "
                           // , actor.Name, sender, goal.Variable, goal.State, goal.Applies, goal.Name, goal.Owner);
                    string applies = goal.Applies;        
                    string a = sender_action.Variable.Substring(0, 1);
                    string b = goal.Variable.Substring(0, 1);
                    int c = string.Compare(a, b);
                    bool d = goal.Applies.Contains(sender);
                    bool e = goal.Applies == Constants.OTHER;
                    bool f = isOtherID(goal.Applies, actor.ID);
                                     
                    if ((goal.Applies.Contains(sender) || goal.Applies == Constants.OTHER )
                            && (string.Compare(a,b) == 0))
                    {
                        log.InfoFormat("ADD Goal For Sender, actor '{0}', sender '{1}', goal '{2}', goal state '{3}', applicable '{4}', goal name '{5}' "
                                , actor.Name, sender, goal.Variable, goal.State, goal.Applies, goal.Name);
                        if (!actor.CurrentGoals.ContainsKey(goal.Name)) actor.CurrentGoals.Add(goal.Name, goal);
                    }

                    //else if (sender_action.State == 1 && !actor.CurrentGoals.ContainsKey(goal.Name)) actor.CurrentGoals.Add(goal.Name, goal);
                }
            }
        }


        private void FindSpeechGoalsForSelf(Actor actor, DEDAction sender_action)
        {
            actor.CurrentGoals.Clear();
            log.InfoFormat("ENTER FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  action '{1}', action state '{2}'", actor.Name, sender_action.Variable, sender_action.State);


            foreach (Dictionary<string, AssignedGoal> dict in actor.AssignedGoals.Values)
            {
                foreach (AssignedGoal goal in dict.Values)
                {
                    //log.InfoFormat("FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  action '{1}', action state '{2}', Owner {3}", actor.Name, goal.Variable, goal.State, goal.Owner);

                    if ((goal.Applies == Constants.SELF || goal.Applies == Constants.OTHER)  && goal.Owner.ToUpper() == Constants.CHARACTER && sender_action.Goals.ContainsKey(goal.Variable))
                    {
                        log.InfoFormat("SELF GOALS: actor '{0}',  goal '{1}', goal state '{2}' ", actor.Name, goal.Variable, goal.State);

                        actor.CurrentGoals.Add(goal.Name, goal);
                    }
                }
            }
        }

        private void FindSpeechGoalsForSelfAndSenderGoal(DEDAction sender_action, Actor actor)
        {
            actor.CurrentGoals.Clear();
            //log.InfoFormat("FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  action '{1}', action state '{2}'", actor.Name, sender_action.Variable, sender_action.State);


            foreach (Dictionary<string, AssignedGoal> dict in actor.AssignedGoals.Values)
            {
                foreach (AssignedGoal goal in dict.Values)
                {
                    log.InfoFormat("FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  goal '{1}', goal state '{2}' ", actor.Name, goal.Variable, goal.State);
                    if (goal.Applies != Constants.SELF) continue;
                    bool isContinue = false;

                    foreach (Goal sender_goal in sender_action.Goals.Values)
                    {
                        if (goal.Variable == sender_goal.Variable)
                        {
                            isContinue = true;
                            break;
                        }
                    }

                    if (!isContinue) continue;

                    if (goal.Owner == Constants.ACTOR)
                    {
                        bool add = true;
                        foreach (Dictionary<string, AssignedGoal> dictb in actor.AssignedGoals.Values)
                        {
                            foreach (Director.Goal gaolb in dictb.Values)
                            {
                                if (gaolb.Owner == Constants.CHARACTER &&
                                    actor.StatesContradict(goal, gaolb)) add = false;
                            }
                        }
                        if (add)
                        {
                            actor.CurrentGoals.Add(goal.Name, goal);
                            log.InfoFormat("FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  goal '{1}', goal state '{2}' ", actor.Name, goal.Variable, goal.State);
                        }
                    }
                    else
                    {
                        actor.CurrentGoals.Add(goal.Name, goal);
                        log.InfoFormat("FindSpeechGoalsForSelfAndSenderGoal, actor '{0}',  goal '{1}', goal state '{2}' ", actor.Name, goal.Variable, goal.State);
                    }
                }
            }
        }

        
        void FindOptimalStatement(List<Strategy> responces, Dictionary<string, Strategy> goalStrategies)
        {
            log.InfoFormat("Enter FindOptimalStatement");
            Double MAX = 0;
            foreach (Strategy s in goalStrategies.Values)
            {
                log.InfoFormat("Variable '{0}', state '{2}', value '{1}' is being evaluated", s.Variable, s.Value, s.State);
                
                if (s.Value > MAX)
                {
                    responces.Clear();
                    MAX = s.Value;
                    log.InfoFormat("Variable '{0}', state '{2}', is current max, value '{1}'", s.Variable, s.Value, s.State);
                }
                if (s.Value == MAX)
                {
                    responces.Add(s);
                }
            }
        }

        //private void AddContributingArgumentsToResponceRequest(DEDAction action, string sender_name, string knowledge, Actor actor)
        //{
        //    DEDAction a = FindResponceForRequest(action, sender_name, knowledge, actor);
        //    List<DEDAction> actions = new List<DEDAction>();
        //    while (a != null)
        //    {
        //        if (action == null) action = new DEDAction(actor.LogicalTime(), a);
        //        if (action.Msg != "") action.Msg = action.Msg + ", " + a.Msg;
        //        else action.Msg = a.Msg;
        //        actions.Add(new DEDAction(actor.LogicalTime(), a));
        //        a = FindContributingArguments(a, sender_name, knowledge, actor);
        //    }
        //    action.AddressedTo = sender_name;
        //    ///SetForExecution(action);
        //    actor.SlActions[action.Name].NotifyOthers(actions, actor.Name);
        //}

        private void AddContributingArguments(DEDAction action, string sender_name, string knowledge
            , Actor actor, string addressedTo, string target)
        {
            //only one contributing arguments. TODO change later to respond to personality
            DEDAction a = FindContributingArguments(action, sender_name, knowledge, actor);
            if (a == null) return;
            a.Responce_to_variable = action.Variable;
            a.Target = target;
            actor.SetForExecution(a);

            //while (true)
            //{
            //    if (a == null) break;
            //    a.Responce_to_variable = action.Variable;
            //    actor.SetForExecution(a);
            //    a = FindContributingArguments(a, sender_name, knowledge, actor);
            //}
        }

        /// <summary>
        /// Needs to advance a step and find the arument that best suits the current specific goal, e.g is swindler
        /// </summary>
        /// <param name="sender_action"></param>
        /// <param name="sender_name"></param>
        /// <param name="knowledge"></param>
        /// <param name="actor"></param>
        /// <returns></returns>
        DEDAction FindContributingArguments(DEDAction sender_action, string sender_name, string knowledge, Actor actor)
        {
            List<Strategy> responces = new List<Strategy>();
            Dictionary<string, Strategy> goalStrategies = new Dictionary<string, Strategy>();

            string variable = "";
            foreach (Goal g in sender_action.Goals.Values) {
                variable = g.Variable;
            }

            ContextSubnet subnet = actor.KnowledgeBase.GetSubnet(this.constants.SUBNET(variable));

            UtilityGreaterVariable util = new UtilityGreaterVariable(subnet, variable);

            List<Strategy> strategies = GetSpeechResponce(sender_action, sender_name, util, actor);

            // Queries his knowledge base for actions that will satisfy the goal.
            foreach (Strategy s in strategies)
            {  if (!goalStrategies.ContainsKey(s.Variable + s.State)) goalStrategies.Add(s.Variable + s.State, s); }
         
            //Merge the values and return the set of optimal strategies
            FindOptimalStatement(responces, goalStrategies);

            DEDAction action = getRandomAction(sender_action, responces, actor, sender_name, actor.CurrentGoals);
            return action;
        }       

        void PrintActions(List<Strategy> strategies, string name)
        {
            foreach (Strategy strategy in strategies)
            {
                log.InfoFormat("({2}) : contemplates action '{0}', state '{1}'"
                    , strategy.Variable, strategy.State, name);
            }
        }
    }
}
