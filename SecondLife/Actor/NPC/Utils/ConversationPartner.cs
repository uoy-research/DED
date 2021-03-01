using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DED.NPC;
using DED.NPC.Actions;

using log4net;
using log4net.Config;

namespace DED.Utils
{
    public enum SentenceType{ Question, Claim, Answer, Demand, Request }

    public class Conversation
    {
        
        public Conversation(string actor, Variable stimulyVariable, SentenceType type, DEDAction action)
        {

            this.actor = actor;
            this.stimulyVariable = stimulyVariable;
            this.stimulyType = type;
            this.action = action;
        }

        string actor;
        DateTime timebegin;
        DateTime timeend;
        Variable stimulyVariable;
        Variable speechVariable;
        DEDAction action;

        SentenceType stimulyType;
        SentenceType speechType;

        public DEDAction Action { get { return this.action; } }
        public string Actor { get { return this.actor; } set { this.actor = value; } }
        public DateTime TimeBegin   { get { return this.timebegin;  } }
        public DateTime TimeEnd     { get { return this.timeend;    } }

        public Variable StimulyVariable { get { return this.stimulyVariable;    } set { this.stimulyVariable = value; this.timebegin = DateTime.Now;    } }
        public Variable SpeechVariable  { get { return this.speechVariable;     } set { this.speechVariable = value; this.timeend = DateTime.Now;       } }

        public SentenceType StimulyType { get { return this.stimulyType;    } set { this.stimulyType = value;   } }
        public SentenceType SpeechType  { get { return this.speechType;     } set { this.speechType = value;    } }
    }

    public class Conversations
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Conversations));
        //Conversations indexed by name and variableKey
        Dictionary<string,Dictionary<string,Conversation>> speeches = new Dictionary<string,Dictionary<string,Conversation>>();
        List<Conversation> activeSpeeches = new List<Conversation>();
        public List<Conversation> ActiveSpeeches { get { return this.activeSpeeches; } }
        DateTime timeLastSpeech;

        public Conversations()
        {
            XmlConfigurator.Configure(); 
            UpdateLastSpeechActTime();
        }

        public Conversation getSpeech() {
            if (this.activeSpeeches.Count > 0)
            {
                Conversation c = this.activeSpeeches[0];
                RemoveActiveSpeech(c);
                UpdateLastSpeechActTime();
                return c;                
            }
            return null; 
        }


        public void addSpeech(DEDAction action, string actor, Variable variable, SentenceType type)
        {
            UpdateLastSpeechActTime();
            //add to history
            Conversation c = new Conversation(actor, variable, type, action);
            //add to active speeches
            if (!this.speeches.ContainsKey(c.Actor)) this.speeches.Add(c.Actor, new Dictionary<string, Conversation>());

            if (!this.speeches[c.Actor].ContainsKey(c.StimulyVariable.getHashKey()))            
            {
                //add to history
                this.speeches[c.Actor].Add(c.StimulyVariable.getHashKey(), c);        
            }
            //add to active speeches
            this.activeSpeeches.Add(c);
        }

        public void addResponce(Conversation c, Variable variable)
        {
            UpdateLastSpeechActTime();
            //if
            if (!this.speeches.ContainsKey(c.Actor)) this.speeches.Add(c.Actor, new Dictionary<string, Conversation>());

            //Remove from active speeches
            RemoveActiveSpeech(c);
            //keep history
            this.speeches[c.Actor][c.StimulyVariable.getHashKey()].SpeechVariable = variable;
        }

        void RemoveActiveSpeech(Conversation c)
        {
            int idx = IDX_ActiveSpeech(c);
            while (idx > -1 && idx < this.activeSpeeches.Count)
            {
                this.activeSpeeches.RemoveAt(idx);
                idx = IDX_ActiveSpeech(c);
            }
        }

        int IDX_ActiveSpeech(Conversation c)
        {
            int idx = 0;
            foreach (Conversation speech in this.activeSpeeches)
            {
                if (speech.Actor == c.Actor
                    && speech.StimulyVariable.getHashKey() == c.StimulyVariable.getHashKey()) return idx;
                ++idx;
            }
            return -1;
        }

        public bool IsActiveSpeeches() { if (this.activeSpeeches.Count == 0) return false; return true; }

        public void UpdateLastSpeechActTime() { this.timeLastSpeech = DateTime.Now; }

        public bool IsTimeToEngage()
        {
            TimeSpan duration = DateTime.Now - this.timeLastSpeech;
            //if no conversation for more than 10 seconds 
            //then wait for random 0 to 10 seconds more and then start engaging.
            Random rand = new Random();

            if (duration.TotalSeconds > (rand.Next(5) + 20))
            {
                log.InfoFormat("Time now '{0}', time last speech '{1}', duration in seconds '{2}'", DateTime.Now, this.timeLastSpeech, duration.TotalSeconds);
                UpdateLastSpeechActTime();
                return true;
            }
            return false;
        }        
    }
}
