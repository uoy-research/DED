using System;
using System.Collections.Generic;
using System.Text;

namespace libsecondlife.TestClient.Commands.Movement
{
    class MoveHomeCommand : Command
    {
        public MoveHomeCommand(TestClient client)
        {
            Name = "moveHome";
            Description = "Moves the avatar to the former set location.";
        }
        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (Client.position != LLVector3.Zero)
            {
                //Client.Self.AutoPilot((ulong)Client.position.X + (ulong)Client.regionX, (ulong)Client.position.Y + (ulong)Client.regionY, Client.position.Z);
                Client.Self.AutoPilotLocal((int)Client.position.X, (int)Client.position.Y, Client.position.Z);
                return "Attempting to move home";
            }
            else
                return "Going home failed: HomeLocation not set";
        }
    }
}
