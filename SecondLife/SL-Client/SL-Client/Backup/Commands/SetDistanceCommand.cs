using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class SetDistanceCommand : Command
    {

        public SetDistanceCommand(TestClient testClient)
        {
            Name = "setDistance";
            Description = "sets distance for actionRadius and distance between Agent and Avatar";
        }


        public override string Execute(string[] args, LLUUID fromAgentID)
        {

            if (args.Length != 2)
            {
                return "usage: setDistance action/agent number";
            }
            else
            {
                float number = 10F;
                if (!float.TryParse(args[1], out number))
                    return "usage: setDistance agent/action number";
                if (args[0].Equals("agent"))
                    Client.FOLLOW_RADIUS_INNER = number;
                if (args[0].Equals("action"))
                    Client.ACTION_RADIUS = number;
                return "Set " + args[0] + " distance to " + number;
            }
        }
    }
}
