using System;
using System.Collections.Generic;
using System.Text;

namespace DED.Utils
{
    class Constants
    {
        public const string PREMISE = "PREM";
        public const string INFERENCE = "INF";
        public const string EVALUATION = "EVALUATION";
        public const string DECISION = "DECISION";
        public const string UTILITY = "UTILITY";
        public const string INTEGRITY = "INTEGRITY";
        public const string EMOTIONAL = "EMOTIONAL";
        public const string CHATTY = "CHATTY";
        public const string CONTRADICTION = "CONTRADICTION";
        public const string ATTRIBUTE = "ATTRIBUTE";
        public const string TRUE = "True";
        public const string FALSE = "False";
        public const string PRO = "PRO";
        public const string CON = "CON";
        public const string OTHER = "OTHER";
        public const string ALL = "ALL";
        public const string SELF = "SELF";
        public const int ANY = -2;
        public const string ANY_ROLE = "ANY";
        public const string SAY = "say";
        public const string DEFAULT = "DEFAULT";
        public const string VICTIM = "victim";
        public const string ADDRESSED_TO = "addressedto";
        public const string CHARACTER = "CHARACTER";
        public const string ACTOR = "ACTOR";
        public const string USER = "USER";
        public const string GREATER = "GREATER";
        public const string MALE = "MALE";
        public const string FEMALE = "FEMALE";
        public const string BOTH = "BOTH";
        public const string DIE = "DIE";
        //public const string GREET = "GREET";

        

        /// <summary>
        /// Flagg to indicate priority.
        /// </summary>
        public const int RESPONCE_FLAGG = 5;

        public const int MAX = 1000;
        public const int SLEEP = 3000;
        public const int SLEEP_SHORT = 300;
        public const int SLEEP_CHAT = 3000;

        public Constants() {}
        public string SUBNET(string prefix)
        {
            switch (prefix.Split('_')[0])
            {
                case "S":
                    return "suspect";
                case "V":
                    return "victim";
                case "MS":
                    return "murder_scene";
                case "MW":
                    return "murder_weapon";
                case "W":
                    return "weapon";
                case "P":
                    return "object";
                case "M":
                    return "murderer";
                case "C":
                    return "chat";
                default: return "";
            }
        }
    }
}
