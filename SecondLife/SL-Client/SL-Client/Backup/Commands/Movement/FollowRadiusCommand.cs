using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class FollowRadiusCommand : Command
    {
        public FollowRadiusCommand(TestClient testClient)
        {
            Name = "followRadius";
            Description = "Follow another avatar only in given radius from setLocation";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (Client.position == LLVector3.Zero)
                return "Cannot follow: Location not set.";
            if (args.Length == 1 && args[0].Equals("false")) {
                Active = false;
                return "Stopped Following";
            }
            if (args.Length < 2)
                return "Wrong Format: followRadius false/{true FirstName LastName}";
            else
            {
                if (!args[0].Equals("true"))
                    return "Wrong Format: followRadius false/{true FirstName LastName}";
                else
                {
                    string target = String.Empty;
                    for (int ct = 1; ct < args.Length; ct++)
                        target = target + args[ct] + " ";
                    target = target.TrimEnd();
                    if (target.Length > 0)
                    {
                        if (Follow(target))
                            return "Following " + target;
                        else
                            return "Unable to follow " + target + ".  Client may not be able to see that avatar.";
                    }
                }
            }
            return String.Empty;
        }

        Avatar followAvatar;

        bool Follow(string name)
        {
            foreach (Avatar av in Client.AvatarList.Values)
            {
                if (av.Name == name)
                {
                    followAvatar = av;
                    Active = true;
                    return true;
                }
            }
            return false;
        }

        public override void Think()
        {
            if (Client.agentOutOfRadius)
            {
                this.Active = false;
            }
            else
            {
                if (Client.position.GetDistanceTo(Client.Self.Position) < Client.ACTION_RADIUS
                    && followAvatar.Position.GetDistanceTo(Client.Self.Position) > Client.FOLLOW_RADIUS_INNER
                    && followAvatar.Position.GetDistanceTo(Client.position) < Client.FOLLOW_RADIUS_INNER + Client.ACTION_RADIUS)
                {
                    //move toward target
                    LLVector3 avPos = followAvatar.Position;
                    //Client.Self.AutoPilot((ulong)avPos.X + (ulong)Client.regionX, (ulong)avPos.Y + (ulong)Client.regionY, avPos.Z);
                    Client.Self.AutoPilotLocal((int)avPos.X, (int)avPos.Y, avPos.Z);
                }
                else
                {
                    //Follow stoppen
                    this.Active = false;
                    //Agent ist Client.FOLLOW_RADIUS_INNER in der Nähe vom Avatar
                    if (Client.position.GetDistanceTo(Client.Self.Position) < Client.ACTION_RADIUS)
                    {
                        //Ziel erreicht, sende Event
                        //LLVector3 myPos = Client.Self.Position;
                        //Client.Self.AutoPilot((ulong)myPos.X + (ulong)Client.regionX, (ulong)myPos.Y + (ulong)Client.regionY, myPos.Z);
                        EventSender.Instance.NotifyStandardOutput("Follow position reached :" + followAvatar.Name, false);
                        EventSender.Instance.NotifyFollowSuccessfulRadius(Client.Self.FirstName + " " + Client.Self.LastName, followAvatar);
                    }
                    //Nicht in der Nähe, heisst out of Followradius und anderer Avatar ist nicht erreichtbar.
                    //Zurück zum HomePoint
                    else
                    {
                        //Client.Self.AutoPilot((ulong)Client.position.X + (ulong)Client.regionX, (ulong)Client.position.Y + (ulong)Client.regionY, Client.position.Z);
                        Client.Self.AutoPilotLocal((int)Client.position.X, (int)Client.position.Y, Client.position.Z);
                        EventSender.Instance.NotifyStandardOutput("Follow position unreachable :" + followAvatar.Name, true);
                        EventSender.Instance.NotifyStandardOutput("Attempting to move home", true);
                    }

                }

                base.Think();
            }
        }

    }
}
