using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class Teleport
    {
        private SecondLife client = null;
        public Teleport(SecondLife client) { this.client = client;  }

        /// <summary>
        /// With a name of a sim and a (new!)location
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="location"></param>
        private void TeleportToLocation(string sim, LLVector3 location)
        {
            client.Self.Teleport(sim, location);
        }

        /// <summary>
        /// With the key of a (new!)landmark 
        /// </summary>
        /// <param name="landmark"></param>
        private void TeleportToLandmark(LLUUID landmark)
        {
            client.Self.Teleport(landmark);
        }
    }
}
