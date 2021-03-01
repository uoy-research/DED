using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DED.Director;
using DED.DPGE;
using DED.Utils;
using log4net;
using log4net.Config;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace DED.Utils.Reads
{
    class ReadFiles
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectorDrama));
        MySQL mysql;

        public ReadFiles(MySQL mysql) { this.mysql = mysql; }

        public List<Avatar> LoadAvatarFromDB(string query)
        {
            log.Info("Reading Avatars Query: "+ query );

            MySqlDataReader reader = this.mysql.Query(query);
            List<Avatar> list = new List<Avatar>();

            while (reader.Read())
            {
                Avatar av = new Avatar(reader["NPCID"].ToString(), reader["title"].ToString(), reader["first_name"].ToString(),
                    reader["last_name"].ToString(), reader["img"].ToString());
                list.Add(av);
            }
            reader.Close();
            return list;            
        }

        public PlotNetwork ReadPlotFile( string plotfile ){
            log.Info("Creating plot");
            PlotNetwork plot = new PlotNetwork(plotfile);
            plot.DrawNet();
            plot.WritePlot();
            return plot;
        }

        public ReadPlot ReadPlot(string plotsettingsfile) {
            ReadPlot plotSettings = new ReadPlot();
            plotSettings.ReadFile(plotsettingsfile);
            return plotSettings;
        }

        public ReadOutput ReadOutputFile(string file)
        {
            ReadOutput output = new ReadOutput();
            output.ReadFile(file);  
            return output;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
        }

        public List<Act> ReadDramaFile(string file)
        {
            ReadDrama r = new ReadDrama();
            r.ReadFile(file);
            List<Act> acts = r.Acts;
            return acts;
        }
        
        public Dictionary<string, Schema> ReadSchemaFile(string schemafile)
        {
            log.Info("Reading " + schemafile);
            string s = System.IO.File.ReadAllText(schemafile);
            Dictionary<string, Schema> schemas = new Dictionary<string,Schema>();

            foreach (string str in s.Trim().Split(';'))
            {
                if (str.Length == 0) continue;
                Schema schema = new Schema();
                schema.Init("schemas/"+str.Trim());
                
                schemas.Add(str.Trim(), schema);
            }
            log.Info("Schemas are read");
            return schemas;
        }
    }
}
