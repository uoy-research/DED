using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;
using DED.Director;
using DED.NPC.Actions;

namespace DED.Utils.Reads
{
    class ReadSchemas : Read
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadSchemas));

        Schema schema;

        public ReadSchemas(Schema schema)
        { 
            XmlConfigurator.Configure();
            this.schema = schema;
        }

        public override void ReadFile(string file) 
        {
            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string name = ""; int act = 0; string applies = ""; string submodel = "";
            bool essential = false; string action_precondition_name = ""; bool action_precondition_satisfied = false;
            string action_precondition_satisfier = ""; int nrExecutions = 0; string role = ""; 
            int value = 0; string drama_goal = ""; string txt = ""; string owner = "";
            string variable = ""; string state = ""; string action_name = ""; string baseAction = "";
            string lastAction = ""; int action_rank = 0; int rank = 0; bool isSpeech = false;
            string schema_precondition_name = ""; bool schema_precondition_satisfied = false;
            string role_variable = ""; string role_subnet = "";
            string action_variable = ""; string action_subnet = "";
            string action_trait = ""; double action_trait_value = 0;

            Dictionary<string, Trait> traits = new Dictionary<string, Trait>();
            Dictionary<string, Precondition> precon = new Dictionary<string, Precondition>();
            Dictionary<string, Goal> actionGoals = new Dictionary<string, Goal>();

            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        txt = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "role":
                                name = "";
                                applies = "";
                                essential = false;
                                role_variable = ""; 
                                role_subnet = "";
                                break;
                            case "action":
                                name = "";
                                role = "";
                                essential = false;
                                nrExecutions = 0;
                                precon.Clear();
                                baseAction = "";
                                lastAction = "";
                                action_rank = 0;
                                rank = 0;
                                isSpeech = false;
                                action_variable = ""; 
                                action_subnet = "";
                                break;
                            case "precondition":
                                action_precondition_name = "";
                                action_precondition_satisfier = "";
                                action_precondition_satisfied = false;
                                break;
                            case "schema_precondition":
                                schema_precondition_name = "";
                                schema_precondition_satisfied = false;
                                break;
                            case "trait":
                                action_trait = "";
                                action_trait_value = 0;
                                break;
                            case "traits":
                                traits.Clear();
                                break;
                            case "action_goal":
                                actionGoals.Clear();
                                action_name = "";
                                value = 0;
                                variable = "";
                                state = "";
                                owner = "";
                                break;
                            case "goal":
                                name = "";
                                owner = "";
                                role = "";
                                applies = "";
                                value = 0;
                                drama_goal = "";
                                submodel = "";
                                variable = "";
                                state = "";
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "role":
                                bool ess = Convert.ToBoolean(essential);
                                Role r = new Role(name, this.schema.Name, ess, role_variable, role_subnet);

                                if (applies == "+" || applies == "*" || applies == Constants.ALL) r.MaxNr = Constants.MAX;
                                else if (applies.Length > 0)
                                {
                                    r.MaxNr = Convert.ToInt32(applies);
                                    r.MinNr = Convert.ToInt32(applies);
                                }
                                if (applies == Constants.ALL) r.MinNr = Constants.MAX;
                                if (applies == "+") r.MinNr = 1;
                                this.schema.PossibleRoles.Add(r.Name, r);
                                break;

                            case "act":
                                this.schema.Acts.Add(act);
                                break;

                            case "trait":
                                traits.Add(action_trait, new Trait(action_trait, action_trait_value));
                                break;

                            case "rank":
                                this.schema.Rank = rank;
                                break;
                                                          

                            case "precondition":
                                precon.Add(action_precondition_name, new Precondition(action_precondition_name, action_precondition_satisfier, action_precondition_satisfied));
                                break;

                            case "schema_precondition":
                                this.schema.Preconditions.Add(schema_precondition_name, new Precondition(schema_precondition_name, schema_precondition_satisfied));
                                break;

                            case "action":
                                Dictionary<string, Schema> schemas = new Dictionary<string, Schema>();
                                schemas.Add(this.schema.Name, this.schema);
                                DEDAction action = new DEDAction(0, name, essential, new Dictionary<string, Precondition>(precon)
                                    , nrExecutions, new Dictionary<string, Goal>(actionGoals), schemas, role, baseAction
                                    , lastAction, action_rank, isSpeech
                                    , action_variable, action_subnet, traits, "");
                                this.schema.PossibleRoles[role].Actions.Add(action);
                                break;

                            case "action_goal":
                                actionGoals.Add(action_name, new Goal(action_name, value, variable, state, owner, applies));
                                break;

                            case "goal":
                                SchemaGoal goal = new SchemaGoal(name, owner, this.schema.PossibleRoles[role], applies, null, submodel
                                    , variable, state, drama_goal, value);
                                this.schema.Goals.Add(goal.Name, goal);
                                break;
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (txt)
                        {
                            case "schema_name":
                                this.schema.Name = objXmlTextReader.Value;
                                break;
                            case "trait_name":
                                action_trait = objXmlTextReader.Value;
                                break;
                            case "trait_value":
                                action_trait_value = Convert.ToDouble(objXmlTextReader.Value);
                                break;                            
                            case "action_name":
                                action_name = objXmlTextReader.Value;
                                break;
                            case "role_variable":
                                role_variable = objXmlTextReader.Value;
                                break;
                            case "role_subnet":
                                role_subnet = objXmlTextReader.Value;
                                break;
                            case "action_variable":
                                action_variable = objXmlTextReader.Value;
                                break;
                            case "action_subnet":
                                action_subnet = objXmlTextReader.Value;
                                break;
                            case "name":
                                name = objXmlTextReader.Value;
                                break;
                            case "rank":
                                rank = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "action_rank":
                                action_rank = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "instances":
                                this.schema.Instances = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "act":
                                act = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "applies":
                                applies = objXmlTextReader.Value.ToUpper();
                                break;
                            case "precondition_satisfied":
                                action_precondition_satisfied = Convert.ToBoolean(objXmlTextReader.Value);
                                break;
                            case "schema_precondition_satisfied":
                                schema_precondition_satisfied = Convert.ToBoolean(objXmlTextReader.Value);
                                break;
                            case "essential":
                                essential = Convert.ToBoolean(objXmlTextReader.Value);
                                break;
                            case "isSpeech":
                                isSpeech = Convert.ToBoolean(objXmlTextReader.Value);
                                break;
                            case "schema_precondition_name":
                                schema_precondition_name = objXmlTextReader.Value;
                                break;
                            case "precondition_name":
                                action_precondition_name = objXmlTextReader.Value;
                                break;
                            case "precondition_satisfier":
                                action_precondition_satisfier = objXmlTextReader.Value;
                                break;
                            case "nrExecutions":
                                nrExecutions = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "role":
                            case "action_role":
                            case "goal_role":
                                role = objXmlTextReader.Value;
                                break;
                            case "value":
                                value = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "owner":
                                owner = objXmlTextReader.Value.ToUpper();
                                break;
                            case "drama_goal":
                                drama_goal = objXmlTextReader.Value;
                                break;
                            case "submodel":
                                submodel = objXmlTextReader.Value;
                                break;
                            case "variable":
                                variable = objXmlTextReader.Value;
                                break;
                            case "state":
                                state = objXmlTextReader.Value;
                                break;
                            case "base_action":
                                baseAction = objXmlTextReader.Value;
                                break;
                            case "last_action":
                                lastAction = objXmlTextReader.Value;
                                break;
                        }
                        break;

                }
            }
        }
    }
}
