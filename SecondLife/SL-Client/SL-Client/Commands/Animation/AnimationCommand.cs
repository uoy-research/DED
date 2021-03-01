using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class AnimationCommand : Command
    {

        public AnimationCommand(TestClient testClient)
        {
            Name = "animation";
            Description = "plays or stops a standardanimation.\nUsage: animation start/stop [animationLLUUID]";

        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (args.Length < 1)
                return Description;
            LLUUID target = new LLUUID();
            try
            {
                LLUUID.TryParse(args[1],out target);
            }
            catch (Exception e)
            {
                //Leitet Fehlermeldung an Eventsender weiter
                EventSender.Instance.NotifyStandardOutput(e.StackTrace, true);
            }
            if (args[0].Equals("start"))
                Client.Self.AnimationStart(target);
            else if (args[0].Equals("stop"))
                Client.Self.AnimationStop(target);
            else
                return Description;
            return "Done.";
        }

    }
}
