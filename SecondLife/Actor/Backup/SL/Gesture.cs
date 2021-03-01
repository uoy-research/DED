using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading;
using libsecondlife;

namespace DED
{
    class Action
    {
        
        //Construct variables
        private SecondLife client = null;

        public Action(SecondLife client) {
            this.client = client;
        }

        public void Gesture()
        {
            //client.Self.Movement.TurnLeft();
        }

        public LLVector3 Teleport(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            LLVector3 vect = new LLVector3(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]));
            client.Self.Teleport(ar[3], vect);
            return vect;
        }

        public LLVector3 getCoordinates(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            return new LLVector3(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]));
        }

        public void ToCoordinates(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            client.Self.AutoPilotLocal(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]));
        }
 
        public void  ToPosition(string position) {
            LLVector3 pVect = getCoordinates(position);
            ToCoordinates(position);
            int count = 0;
            while (true){
                System.Threading.Thread.Sleep(500);
                LLVector3 p = client.Self.RelativePosition;
                Console.WriteLine(client.Self.Name+ ' ' + p);
                if ( libsecondlife.LLVector3.Dist(p,pVect) < 2 ){
                    Console.WriteLine("MATCH");
                    break;
                }
                count += 1;
            }
        }
        
        public void  Sit( string val ){            
            LLUUID target = new LLUUID(val);
            string msg = string.Format("Attempting sit, val '{0}', UUID = '{1}'", val, target.UUID.ToString());
            //client.Log(msg, Helpers.LogLevel.Info);
            client.Self.RequestSit(target, LLVector3.Zero);
            client.Self.Sit();
        }

        public void Sit(string val, string direction)
        {
            //client.Log("LOOKING", Helpers.LogLevel.Info);
            LookToward(direction);
            
            System.Threading.Thread.Sleep(2000);
            //client.Log("SITTING", Helpers.LogLevel.Info);
            Sit(val);
        }

        public void StandUp()
        {
            client.Self.Stand();
        }

        public bool LookToward(string position)
        {
            if (client.Settings.SEND_AGENT_UPDATES)
            {
                logCamera();
                LLVector3 target = getCoordinates(position);
                client.Self.Movement.TurnToward(target);
                client.Self.Movement.Camera.LookDirection(target);
                logCamera();             

                client.Self.Movement.SendUpdate();

                return true;
            }
            else
            {
                //client.Log("Attempted TurnToward but agent updates are disabled", Helpers.LogLevel.Warning);
                return false;
            }
        }

        public void LookAT(string position)
        {
            Console.WriteLine("Head postition {0}", client.Self.Movement.HeadRotation.ToString());
            client.Self.Movement.Camera.SetPositionOrientation(getCoordinates(position),0,-30,0);
            client.Self.Movement.SendUpdate();
        }

        public void LookAround()
        {
            Console.WriteLine("Head postition: W '{0}', X '{1}, Y '{2}, Z '{3}",
                                client.Self.Movement.HeadRotation.W,
                                client.Self.Movement.HeadRotation.X,
                                client.Self.Movement.HeadRotation.Y,
                                client.Self.Movement.HeadRotation.Z);
            client.Self.Movement.HeadRotation.W += 1; 
            client.Self.Movement.HeadRotation.X += 1;
            client.Self.Movement.HeadRotation.Y += 1;
            client.Self.Movement.HeadRotation.Z += 1;
            client.Self.Movement.SendUpdate();
        }

        public void logCamera(){
            string msg = string.Format("Position '{0}', AtAxis '{1}', LeftAxis '{2}', UpAxis '{3}'",
                client.Self.Movement.Camera.Position.ToString(),
                client.Self.Movement.Camera.AtAxis.ToString(),
                client.Self.Movement.Camera.LeftAxis.ToString(),
                client.Self.Movement.Camera.UpAxis.ToString());

            //client.Log(msg, Helpers.LogLevel.Info);
        }

        public void Yaw(string value)
        {
            logCamera();
            float yaw = (float)Convert.ToDouble(value);
            client.Self.Movement.Camera.Yaw(yaw);
            client.Self.Movement.SendUpdate();
            logCamera();
        }

        public void Pitch(string value)
        {
            logCamera();
            float pitch = (float)Convert.ToDouble(value);
            client.Self.Movement.Camera.Pitch(pitch);
            client.Self.Movement.SendUpdate();
            logCamera();
        }        
    }
}

