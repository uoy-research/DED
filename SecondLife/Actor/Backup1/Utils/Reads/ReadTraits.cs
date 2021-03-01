using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;

using System.Xml;

namespace DED.Utils.Reads
{
    public class ReadTraits : Read
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadTraits));

        public ReadTraits() { XmlConfigurator.Configure(); }

        Dictionary<string, Dictionary<string, double>> traits = new Dictionary<string, Dictionary<string, double>>();

        public double GetValue(string character, string trait) { return this.traits[character][trait]; }

        public override void ReadFile(string file)
        {
            log.Info("Reading " + file);

            XmlTextReader objXmlTextReader = new XmlTextReader(file);
            string text = ""; string character = ""; string name = ""; double value = 0; 

            while (objXmlTextReader.Read())
            {
                switch (objXmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        text = objXmlTextReader.Name;
                        switch (objXmlTextReader.Name)
                        {
                            case "trait":                                
                                name = "";                                
                                character = "";
                                value = 0;
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (objXmlTextReader.Name)
                        {
                            case "trait":
                                if (!this.traits.ContainsKey(character)) this.traits.Add(character, new Dictionary<string, double>());
                                this.traits[character].Add(name, value);
                                break;                            
                        }
                        break;

                    case XmlNodeType.Text:
                        switch (text)
                        {
                            case "name":
                                name = objXmlTextReader.Value.Trim().ToUpper();
                                break;
                            case "character":
                                character = objXmlTextReader.Value.Trim().ToLower();
                                break;
                            case "value":
                                value = Convert.ToDouble(objXmlTextReader.Value);
                                break;
                            
                        }
                        break;
                }
            }
        }
    }
}
