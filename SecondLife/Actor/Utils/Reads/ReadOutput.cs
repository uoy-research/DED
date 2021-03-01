using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;
using DED.Director;

namespace DED.Utils.Reads
{
    public class Sentence
    {
        public Sentence(string action, string speech, List<string> attributes, string state, string target, string variable
            , string goalstate, List<Gesture> gestures)
        {
            this.action = action;
            this.speech = speech;
            this.attributes = new List<string>(attributes);
            this.state = state;
            this.target = target;
            this.variable = variable;
            this.goalstate = goalstate;
            foreach (Gesture g in gestures)
                this.Gestures.Add(new Gesture(g));
        }
        public Sentence(Sentence s)
        {
            this.action = s.Action;
            this.speech = s.Speech;
            this.attributes = new List<string>(s.attributes);
            this.state = s.State;
            this.target = s.Target;
            this.variable = s.Variable;
            this.goalstate = s.Goalstate;
            this.gestures = s.Gestures;
        }

        string action = ""; string speech = ""; List<string> attributes; string state = "";
        string target = ""; string variable = ""; string goalstate = "";

        public string Action { get { return this.action; } }
        public string Speech { get { return this.speech; } set { this.speech = value; } }
        public List<string> Attributes { get { return this.attributes; } }
        List<Gesture> gestures = new List<Gesture>(); public List<Gesture> Gestures { get { return this.gestures; } }
        public string State { get { return this.state; } }
        public string Target { get { return this.target; } }
        public string Variable { get { return this.variable; } }
        public string Goalstate { get { return this.goalstate; } }
        public override string ToString()
        {
            return string.Format(", Variable '{0}', state '{2}', target '{3}', goal '{4}', goal state '{5}' the sentence: '{1}'", action, speech, state, target, variable, goalstate);
        }
    }

    public class Gesture{
        public Gesture(string name)
        {
            this.name = name;
        }

        public Gesture(Gesture g)
        {
            this.name = g.Name;
        }

        string name;
        public string Name { get { return this.name; } }
    }

    public class ReadOutput: Read
    {
        //variable, goal, goalstate, variablestate
        Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Sentence>>>>> characterSpeech;
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Sentence>>>>> CharacterSpeech { get { return this.characterSpeech; } }

        public List<Sentence> Sentences(string variable, Goal goal, string variablestate)
        {
            if (!ContainsKey(variable, goal.Variable, goal.State, variablestate)) return new List<Sentence>();
            List<Sentence> sentences = new List<Sentence>();

            foreach (Sentence s in this.characterSpeech[variable][goal.Variable][goal.State][variablestate])
            {
                log.InfoFormat("The sentence introduced: The sentence is '{0}', the target is '{1}'.", s.Speech, s.Target);

                if (s.Target == Constants.DEFAULT) { sentences.Add(s); continue; }

                if (goal.Applies == Constants.SELF && s.Target != Constants.SELF) continue;
                if (goal.Applies != Constants.SELF && s.Target == Constants.SELF) continue;
                log.InfoFormat("The sentence chosen: The sentence is '{0}', the target is '{1}'.", s.Speech, s.Target);
                sentences.Add(s);
            }
            return sentences;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(ReadOutput));

        public ReadOutput() { XmlConfigurator.Configure(); }

        private bool ContainsKey(string action, string variable, string goalstate, string state)
        {
            log.InfoFormat("The sentence chosen: The variable is '{0}' and its state is '{2}'. The goal is '{1}' and its state is '{3}'.", action, variable, goalstate, state);
            if (!this.characterSpeech.ContainsKey(action))
            {
                log.InfoFormat("Cant find action '{0}' ", action);
                return false;
            }
            if (!this.characterSpeech[action].ContainsKey(variable))
            {
                log.InfoFormat("Cant find variable '{0}' ", variable);
                return false;
            }
            if (!this.characterSpeech[action][variable].ContainsKey(goalstate))
            {
                log.InfoFormat("Cant find goal '{0}' ", goalstate);
                return false;
            }
            if (!this.characterSpeech[action][variable][goalstate].ContainsKey(state))
            {
                log.InfoFormat("Cant find variable state '{0}' ", state);
                return false;
            }
            return true;
        }

        public override void ReadFile(string file)
        {
            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string text = ""; string action = ""; string speech = ""; string attribute = ""; string state = "";
            string target = ""; string variable = ""; string goalstate = ""; string gesture_name = "";

            List<Sentence> sentences = new List<Sentence>();
            List<Gesture> gestures = new List<Gesture>();
            List<string> attributes = new List<string>();
            this.characterSpeech = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Sentence>>>>>();

            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        text = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "sentences":
                                sentences = new List<Sentence>();
                                state = "";
                                break;
                            case "user":
                                action = "";
                                break;
                            case "sentence":
                                speech = "";
                                attribute = "";
                                target = "";
                                break;
                            case "goal":
                                variable = "";
                                goalstate = "";
                                break;
                            case "attributes":
                                attributes = new List<string>();
                                break;
                            case "gesture":
                                gesture_name = "";
                                break;
                            case "gestures":
                                gestures = new List<Gesture>();
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "attribute":
                                attributes.Add(attribute);
                                break;
                            case "sentence":
                                sentences.Add(new Sentence(action, speech, attributes, state, target, variable, goalstate, gestures));
                                break;
                            case "goal":
                                sentences = new List<Sentence>();
                                break;
                            case "sentences":
                                if (!this.characterSpeech.ContainsKey(action)) this.characterSpeech.Add(action, new Dictionary<string, Dictionary<string, Dictionary<string, List<Sentence>>>>());
                                if (!this.characterSpeech[action].ContainsKey(variable)) this.characterSpeech[action].Add(variable, new Dictionary<string, Dictionary<string, List<Sentence>>>());
                                if (!this.characterSpeech[action][variable].ContainsKey(goalstate)) this.characterSpeech[action][variable].Add(goalstate, new Dictionary<string, List<Sentence>>());
                                this.characterSpeech[action][variable][goalstate].Add(state, sentences);
                                break;
                            case "gesture":
                                gestures.Add(new Gesture(gesture_name));
                                break;                            
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (text)
                        {
                            case "action":
                                action = objXmlTextReader.Value;
                                break;
                            case "speech":
                                speech = objXmlTextReader.Value;
                                break;
                            case "attribute":
                                attribute = objXmlTextReader.Value;
                                break;
                            case "state":
                                state = objXmlTextReader.Value;
                                break;
                            case "target":
                                target = objXmlTextReader.Value.ToUpper();
                                break;
                            case "variable":
                                variable = objXmlTextReader.Value;
                                break;
                            case "goalstate":
                                goalstate = objXmlTextReader.Value;
                                break;
                            case "gesture_name":
                                gesture_name = objXmlTextReader.Value;
                                break;
                        }
                        break;
                }
            }
            //PrintOutput();
        }

        void PrintOutput()
        {
            foreach (Dictionary<string, Dictionary<string, Dictionary<string, List<Sentence>>>> i in this.characterSpeech.Values)
                foreach (Dictionary<string, Dictionary<string, List<Sentence>>> x in i.Values)
                    foreach (Dictionary<string, List<Sentence>> z in x.Values)
                        foreach (List<Sentence> y in z.Values)
                            foreach (Sentence each in y) log.Info(each.ToString());
        }

    }
    
}
