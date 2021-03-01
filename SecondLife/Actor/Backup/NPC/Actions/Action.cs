using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using log4net;
using log4net.Config;


namespace DED.NPC
{
    
    abstract class Action
    {
        public readonly ILog log = LogManager.GetLogger(typeof(Action));

        protected Actor actor;
        string actionName;
        double value;
        public double Value { get { return this.value; } }
        protected SecondLife Client { get { return this.actor.Client; } }
        public string ActionName { get { return this.actionName; } }

        public Action(Actor actor, string actionName, double value)
        {
            this.actor = actor;
            this.actionName = actionName;
            this.value = value;
            XmlConfigurator.Configure();
        }

        public abstract void act();

        public LLVector3 getCoordinates(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            return new LLVector3(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]));
        }

        void ToCoordinates(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            this.actor.Client.Self.AutoPilotLocal(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]));
        }

        protected void ToPosition(string position)
        {
            LLVector3 pVect = getCoordinates(position);
            ToCoordinates(position);
            int count = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(500);
                LLVector3 p = this.actor.Client.Self.RelativePosition;
                //Console.WriteLine(this.actor.Client.Self.Name + ' ' + p);
                if (libsecondlife.LLVector3.Dist(p, pVect) < 2)
                {
                    //Console.WriteLine("MATCH");
                    break;
                }
                count += 1;
            }
        }
    }
}
