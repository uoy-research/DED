using System;
using System.Collections.Generic;
using System.Text;
using DED.DPGE;
using DED.NPC;
using DED.Decision;
using DED.Director;
using log4net;
using log4net.Config;
using System.Threading;
using DED.Utils;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace DED
{
    class DED
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DED));
        public static void Main()
        {
            XmlConfigurator.Configure();            
            NewGame();
                    
        }                

        private static void NewGame()
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            //FileStream fs = System.IO.File.ReadAllText("files.txt");
            string fs = System.IO.File.ReadAllText("files.txt");
            
            string[] ar = fs.Split('\n');
            foreach (string s in ar)
            {
                string[] ar_s = s.Split(',');
                files.Add(ar_s[0].Trim(), ar_s[1].Trim());
            }

            //Connect to MySQL DB 
            MySQL mysql = new MySQL();
            mysql.Connect();

            //A list to keep ID of all players that are asking for a new game.
            List<int> ng = new List<int>();
            MySqlDataReader reader;
            //A dict to keep active directors
            Dictionary<int, DirectorDrama> directors = new Dictionary<int, DirectorDrama>();
            //and threads
            Dictionary<int, Thread>  threads = new Dictionary<int,Thread>();
            
            while (true)
            {
                ng.Clear();
                Thread.Sleep(Constants.SLEEP);
                
                try
                {
                    reader = mysql.Query("select * from ded_newgame");
                }
                catch (Exception e)
                {
                    log.Info(e.Message);
                    continue;
                }

                while (reader.Read())
                {
                    ng.Add(Convert.ToInt32(reader["UID"]));
                }

                reader.Close();

                foreach (int uid in ng)
                {
                    //int uid = Convert.ToInt32(reader["UID"]);
                    // delete this record from the DB so that we do not start many games for this player
                    mysql.Delete("delete from ded_newgame where UID = " + uid);
                    if (directors.ContainsKey(uid)) {
                        directors[uid].STOP = false; //stop director
                        threads[uid].Abort(); //Abort thread
                    }
                    directors[uid] = new DirectorDrama();

                    directors[uid].Init(uid, "schemas/schemas.txt", "dpge.txt", "mysterydrama.cix", "prims.cix"
                                  , "speech/useroutput.cix", "drinks.xml", "plot.xml", files, "traits.xml"); //start a new director
                    // New thread + start the director off in a new game
                    Thread t = new Thread(directors[uid].StartGame);
                    if (!threads.ContainsKey(uid)) threads.Add(uid, t);
                    t.Name = Convert.ToString(uid);
                    t.Start();
                             
                }
                    
            }
        }        

    }

    
}