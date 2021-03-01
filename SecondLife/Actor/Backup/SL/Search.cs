using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using libsecondlife;

namespace DED
{
    class Search
    {
        //Construct variables
        private SecondLife client = null;
        Dictionary<LLUUID, Primitive> PrimsWaiting = new Dictionary<LLUUID, Primitive>();
        Dictionary<LLUUID, Parcel> ParcelsWaiting = new Dictionary<LLUUID, Parcel>();
        AutoResetEvent AllPropertiesReceived = new AutoResetEvent(false);

        public Search(SecondLife client)
        { this.client = client; }

        public List<Primitive> FindAllInRadius(float radius )
        {
            LLVector3 camPos = client.Self.Movement.Camera.Position;
            Console.WriteLine("Position {0}", camPos.ToString());
            camPos.X = (float)98.5;
            camPos.Y = (float)147.5;
            camPos.Z = (float)25.5;
            Console.WriteLine("camPos {0}", camPos.ToString());
            
            
            client.Self.Movement.TurnToward(camPos);
            client.Self.Movement.Camera.LookDirection(camPos);
            camPos = client.Self.Movement.Camera.Position;
            Console.WriteLine("Position {0}", camPos.ToString());
            List<Simulator> sims = client.Network.Simulators;
            foreach (Simulator s in sims)
                    Console.WriteLine("Sim name '{0}' number of prims is {1} and number of avatars is {2} ",
                        s.Name, client.Network.CurrentSim.Stats.Objects, s.ObjectsAvatars.Count);

            LLVector3 location = client.Self.SimPosition;
            List<Primitive> prims = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                    delegate(Primitive prim)
                    {
                        LLVector3 pos = prim.Position; 
                        //(prim.ParentID == 0) && && (LLVector3.Dist(pos, location) < radius)
                        return ((pos != LLVector3.Zero) && (LLVector3.Dist(pos, location) < radius));
                    }
               );

            Console.WriteLine("PRIMS COUNT = {0} in sim '{1}'", prims.Count, client.Network.CurrentSim.Name);
            // *** request properties of these objects ***
            bool complete = RequestObjectProperties(prims, 250);            

            if (!complete)
            {
                Console.WriteLine("Warning: Unable to retrieve full properties for: {0} prims", PrimsWaiting.Count);
                //foreach (LLUUID uuid in PrimsWaiting.Keys)
                //    Console.WriteLine(uuid);
            }
            return prims;
        }

        public List<Primitive> GetPropps(float radius)
        {            
            LLVector3 location = client.Self.SimPosition;
            List<Primitive> prims = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                    delegate(Primitive prim)
                    {
                        LLVector3 pos = prim.Position;
                        //(prim.ParentID == 0) && && (LLVector3.Dist(pos, location) < radius)
                        return ((pos != LLVector3.Zero) );
                    }
               );

            Console.WriteLine("PRIMS COUNT = {0} in sim '{1}'", prims.Count, client.Network.CurrentSim.Name);
            // *** request properties of these objects ***
            bool complete = RequestObjectProperties(prims, 250);

            if (!complete)
            {
                Console.WriteLine("Warning: Unable to retrieve full properties for: {0} prims", PrimsWaiting.Count);                
            }
            return prims;
        }

        public List<Parcel> FindAllParcels( )
        {
            List<Simulator> sims = client.Network.Simulators;
            Console.WriteLine("Number of sims is {0}", sims.Count);
            foreach (Simulator s in sims)
                    Console.WriteLine("Sim name '{0}' number of prims is {1} and number of avatars is {2} ",
                        s.Name, s.ObjectsPrimitives.Count, s.ObjectsAvatars.Count);
            
            LLVector3 location = client.Self.SimPosition;
            List<Parcel> parcels = client.Network.CurrentSim.Parcels.FindAll(
                    delegate(Parcel parcel)
                    {
                        //LLVector3 pos = parcel.l;
                        //(prim.ParentID == 0) && (pos != LLVector3.Zero) && (LLVector3.Dist(pos, location) < radius)
                        return (true );
                    }
               );

            Console.WriteLine("Parcel COUNT = {0} in sim '{1}'", parcels.Count, client.Network.CurrentSim.Name);
            // *** request properties of these objects ***

            return parcels;
        }

        private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
        {
            // Create an array of the local IDs of all the prims we are requesting properties for
            uint[] localids = new uint[objects.Count];

            lock (PrimsWaiting)
            {
                PrimsWaiting.Clear();

                for (int i = 0; i < objects.Count; ++i)
                {
                    localids[i] = objects[i].LocalID;
                    if (!PrimsWaiting.ContainsKey(objects[i].ID))

                        PrimsWaiting.Add(objects[i].ID, objects[i]);
                }
            }

            client.Objects.SelectObjects(client.Network.CurrentSim, localids);

            return AllPropertiesReceived.WaitOne(2000 + msPerRequest, false);
        }        

        void Objects_OnObjectProperties(Simulator simulator, LLObject.ObjectProperties properties)
        {
            lock (PrimsWaiting)
            {
                Primitive prim;
                if (PrimsWaiting.TryGetValue(properties.ObjectID, out prim))
                {
                    prim.Properties = properties;
                }
                PrimsWaiting.Remove(properties.ObjectID);

                if (PrimsWaiting.Count == 0)
                    AllPropertiesReceived.Set();
            }            
        }
    }
}
