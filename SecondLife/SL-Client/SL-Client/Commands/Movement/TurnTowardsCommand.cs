using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class TurnTowardsCommand : Command
    {
        public TurnTowardsCommand(TestClient testClient)
        {
            Name = "turnTowards";
            Description = "Turns body towards a given avatar";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                return "Wrong number of parameters (turnToward Personname)";
            }
            string target = String.Empty;
            for (int ct = 0; ct < args.Length; ct++)
                target = target + args[ct] + " ";
                target = target.TrimEnd();
            if (target.Length > 0)
            {
                foreach (Avatar av in Client.AvatarList.Values)
                {
                    if (av.Name == target)
                    {
                        Client.Self.TurnToward(av.Position);
                        return "Turning towards "+av.Name+".";
                    }
                }
            }
            return "Turning towards "+target+" failed.";
        }

    }
}
