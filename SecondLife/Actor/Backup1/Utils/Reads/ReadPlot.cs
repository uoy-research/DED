using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;
using System.Collections;

namespace DED.Utils.Reads
{    

    public class ReadPlot: Read
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadPlot));
        
        /// <summary>
        /// A list of the acts
        /// </summary>
        Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public Dictionary<string,Variable> Variables { get { return this.variables; } }

        public ReadPlot() { XmlConfigurator.Configure(); }

        public override void ReadFile(string file)
        {
            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string name = ""; string subnet = ""; bool singular = false;
            string text = ""; int state = -1; string parent = "";
            bool setParent = false;

            ArrayList parents = new ArrayList();

            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        text = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "variable":
                                name = "";
                                subnet = "";
                                state = -1;
                                singular = false;
                                setParent = false;
                                parent = "";
                                parents = new ArrayList();
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "variable":
                                variables.Add(name,new Variable(name, subnet, state, singular, setParent, parents));
                                break;
                            case "parent":
                                parents.Add(parent);
                                break;
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (text)
                        {                            
                            case "state":
                                state = Convert.ToInt32(objXmlTextReader.Value);
                                break;
                            case "name":
                                name = objXmlTextReader.Value;
                                break;
                            case "subnet":
                                subnet = objXmlTextReader.Value;
                                break;
                            case "parent":
                                parent = objXmlTextReader.Value.Trim();
                                break;
                            case "singular":
                                if (objXmlTextReader.Value.ToUpper() == Constants.TRUE.ToUpper())
                                    singular = true;
                                break;
                            case "setParent":
                                if (objXmlTextReader.Value.ToUpper() == Constants.TRUE.ToUpper())
                                    setParent = true;
                                break;
                        }
                        break;

                }
            }
        }
    }
}
