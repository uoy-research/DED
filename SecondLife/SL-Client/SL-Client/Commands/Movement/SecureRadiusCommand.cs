using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class SecureRadiusCommand : Command
    {
        public SecureRadiusCommand(TestClient testClient)
        {
            Name = "secureRadius";
            Description = "enables a check if avatar moves too far from homeLocation";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            Active = !Active;
            String ret = String.Empty;
            if (Active)
                ret = "Enabled secureRadius";
            else
                ret = "Disabled secureRadius";
            return ret;
        }

        public override void Think()
        {
            if (Client.position.GetDistanceTo(Client.Self.Position) > (Client.ACTION_RADIUS + 5.0f))
            {
                //move home!
                //Client.Self.AutoPilot((ulong)Client.position.X + (ulong)Client.regionX, (ulong)Client.position.Y + (ulong)Client.regionY, Client.position.Z);
                Client.Self.AutoPilotLocal((int)Client.position.X, (int)Client.position.Y, Client.position.Z);
                EventSender.Instance.NotifyStandardOutput("Agent out of secureRadius. Attempting to move it back", true);
                Client.agentOutOfRadius = true;
            }
            if (Client.agentOutOfRadius && Client.position.GetDistanceTo(Client.Self.Position) < 3.0F)
            {
                Client.agentOutOfRadius = false;

            }
            base.Think();
        }

    }
}
