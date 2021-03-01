using System;
using System.Collections.Generic;
using System.Text;
using DED.DPGE;
using DED.NPC;
using DED.Decision;
using DED.Director;
using log4net;
using log4net.Config;

namespace DED
{
    class DED
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DED));
        public static void Main()
        {
            XmlConfigurator.Configure();
            InitDED("schemas.txt","actors.txt","dpge.txt","mysterydrama.txt");
            //StartDrama();                    
        }

        //private static void CreatePlot()
        //{
        //    Console.WriteLine("Calling DPGE");
        //    Plot p = new Plot("dpge.txt");
        //    p.DrawNet();
        //    p.WritePlot();
            
         

        //    //calculate
        //    Utility u = new Utility();
        //    u.Evaluate(characters[0].KnowledgeBase, characters[1].Name);

        //    Console.WriteLine("Goodby cruel world, I'm leaving you today!");
        //    string s = Console.Read().ToString();
            
        //}

        private static void InitDED(string schemas, string avatars, string plot, string drama)
        {         
            DirectorDrama dd = new DirectorDrama();
            dd.Init(schemas, avatars, plot, drama);            
            
            Console.ReadKey();
        }
    }
}
