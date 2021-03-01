using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;
using DED.Director;


namespace DED.Utils.Reads
{
    class Stimuly {
        public Stimuly(string title, string msg, List<Variable> variables, List<Goal> goals, bool isQuestion) {
            this.title = title;
            this.msg = msg;
            this.variables = new List<Variable>();
            foreach (Variable v in variables) this.variables.Add(v);
            this.goals = new List<Goal>();
            this.question = isQuestion;
            foreach ( Goal g in goals)
                this.goals.Add(g);
        }

        List<Variable> variables; public List<Variable> Variables { get { return this.variables; } }
        List<Goal> goals; public List<Goal> Goals { get { return this.goals; } }
        string title; public string Title { get { return this.title; } }
        string msg; public string Msg { get { return this.msg; } set { this.msg = value; } }
        int id = -1; public int ID { get { return this.id; } set { this.id = value; } }
        bool question; public bool isQuestion { get { return this.question; } set { this.question = value; } }
    }

    class ReadStimuly: Read
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadOutput));

        public ReadStimuly() { XmlConfigurator.Configure(); }

        List<Stimuly> stimulies = new List<Stimuly>(); public List<Stimuly> Stimulies { get { return this.stimulies; } }

        public override void ReadFile(string file)
        {
            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string text = ""; string name = ""; string msg = ""; string subnet = ""; int state = -1;
            string var = ""; int value = -1; string title = "";
            List<Variable> variables = new List<Variable>();
            List<Goal> goals = new List<Goal>();
            bool question = false;

            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        text = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "input":
                                question = false;
                                name = "";
                                msg = "";
                                subnet = "";
                                state = -1;
                                var = "";
                                value = -1;
                                variables.Clear();
                                goals.Clear();
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "input": 
                                this.stimulies.Add(new Stimuly(title,msg,variables,goals, question));
                                break;
                            case "variable":
                                variables.Add(new Variable(name,subnet,state));
                                break;
                            case "goal":
                                goals.Add( new Goal(name,value,var,Convert.ToString(state)));
                                break;
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (text)
                        {
                            case "name":
                                name = objXmlTextReader.Value;
                                break;
                            case "title":
                                title = objXmlTextReader.Value;
                                break;
                            case "msg":
                                msg = objXmlTextReader.Value;
                                break;
                            case "subnet":
                                subnet = objXmlTextReader.Value;
                                break;
                            case "var":
                                var = objXmlTextReader.Value;
                                break;
                            case "state":
                                state = Convert.ToInt32( objXmlTextReader.Value );
                                break;
                            case "value":
                                value = Convert.ToInt32( objXmlTextReader.Value );
                                break;
                            case "question":
                                question = Convert.ToBoolean(objXmlTextReader.Value);
                                break;

                        }
                        break;
                }
            }
        }
    }
}
