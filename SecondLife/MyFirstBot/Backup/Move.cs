using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class Move
    {
        //Construct variables
        private SecondLife client = null;

        ////Follow code vars///
        private static float followDistance = 0; //How far, in meters, the target AV can go before we start moving
        public static bool followon = true; //A boolean switch to enable or disable following
        public static string followName = "Test User"; //This is the name of the target avatar

        public Move(SecondLife client) {
            this.client = client;
        }

        public void Gesture()
        {
            //client.Self.Movement.TurnLeft();
        }

        public void Teleport(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            client.Self.Teleport(ar[3], new LLVector3(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]))); 
        }

        public void ToCoordinates(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            client.Self.AutoPilotLocal(Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2])); 
        }

        public void Turn(string coordinates)
        {
            string[] ar = coordinates.Split('.');
            client.Self.Movement.TurnToward(new LLVector3( Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1]), Convert.ToInt32(ar[2]))); 
        }

        public void FollowAgent(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            //There seems to be a problem with the bot following you on the Z axis
            //The live performance of the bots follow coordinates updating seems to be based on the simulator's current performance. 
            if (followon == true) //Check to make sure we need to be following
            {
                if (!update.Avatar) { return; }//Exit this event if it's not an avatar update
                Avatar av;
                client.Network.CurrentSim.Objects.TryGetAvatar(update.LocalID, out av);
                if (av == null) return;
                if (av.Name == followName)
                {
                    LLVector3 pos = av.Position;

                    if (LLVector3.Dist(pos, client.Self.SimPosition) > followDistance)
                    {
                        int followRegionX = (int)(regionHandle >> 32);
                        int followRegionY = (int)(regionHandle & 0xFFFFFFFF);
                        int followRegionZ = (int)(regionHandle);
                        ulong x = (ulong)(pos.X + followRegionX);
                        ulong y = (ulong)(pos.Y + followRegionY);

                        client.Self.AutoPilotCancel();
                        if (pos.Z > 1)
                        {
                            client.Self.AutoPilotLocal(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), pos.Z);
                        }
                        else
                        {
                            client.Self.AutoPilotCancel();
                        }
                    }
                }
            }
        }
    }
}
