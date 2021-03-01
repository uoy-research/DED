using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class LocationCommand: Command
    {
        public LocationCommand(TestClient testClient)
		{
			Name = "location";
			Description = "Show the location. Set the location to remember. Use: location [set]";
		}

		public override string Execute(string[] args, LLUUID fromAgentID)
		{
            String locationSet = String.Empty;
            if (args.Length == 1 && args[0].Equals("set"))
            {
                Client.position = Client.Self.Position;
                locationSet = "  --Location set--";
            }

            return "CurrentSim: '" + Client.Network.CurrentSim.ToString() + "' Position: " + 
                Client.Self.Position.ToString() + locationSet;
		}
    }
}
