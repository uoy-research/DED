using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;
using DED.Director;
using System.Xml.Serialization;

namespace DED.Utils.Reads
{
    class ReadDrama: Read
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadDrama));
        
        /// <summary>
        /// A list of the acts
        /// </summary>
        List<Act> acts = new List<Act>();
        public List<Act> Acts { get { return this.acts; } }

        public ReadDrama() { XmlConfigurator.Configure(); }


        public override void ReadFile(string file)
        {         

            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string name = ""; int value = 0; string applies = "";
            string text = ""; string aim = ""; string die = "";
            int number = 0;
            Dictionary<string, DramaGoal> goals = new Dictionary<string, DramaGoal>();
            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        text = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "goal":
                                die = "";
                                name = "";
                                value = 0;
                                applies = "";
                                aim = "";
                                break;
                            case "act":
                                number = 0;
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "goal":
                                DramaGoal dg = new DramaGoal(name, applies, value, die);
                                goals.Add(name, dg);
                                break;
                            case "act":
                                acts.Add(new Act(new Dictionary<string, DramaGoal>(goals), number));
                                goals.Clear();
                                break;
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (text)
                        {
                            case "number":
                                number = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "aim":
                                aim = objXmlTextReader.Value.ToUpper();
                                break;
                            case "name":
                                name = objXmlTextReader.Value;
                                break;
                            case "value":
                                value = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "applies":
                                applies = objXmlTextReader.Value;
                                break;
                            case "die":
                                die = objXmlTextReader.Value;
                                break;
                        }
                        break;

                }
            }

           
        }
    }
}
