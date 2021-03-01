using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class ScanRadiusCommand : Command
    {
        List<Avatar> avatarnames_in_range = new List<Avatar>();
        public ScanRadiusCommand(TestClient testClient)
        {
            Name = "scanRadius";
            Description = "scans for Users in given radius. Usage: scanRadius [list]";
        }


        public override string Execute(string[] args, LLUUID fromAgentID)
        {

            if (args.Length > 1)
            {
                return Description;
            }
            if (args.Length == 0)
            {
                if (!Active)
                {
                    Active = true;
                    //Client.Self.OnChat += new MainAvatar.ChatCallback(Self_OnChat);
                    //SocketClient.Instance.setClient(this.Client.ClientManager);
                    //SocketClient.Instance.openConnection();
                    return "scanning for avatars is now on with radius " + Client.ACTION_RADIUS;
                }
                else
                {
                    Active = false;
                    return "scanning for avatars is now off.";
                }
            }
            else if (args[0].ToLower().Equals("list"))
            {
                EventSender.Instance.NotifyStandardOutput("SR in Avatarlist: ", false);
                foreach (Avatar av in Client.AvatarList.Values)
                {
                    if (av.ID != Client.Self.ID)
                        EventSender.Instance.NotifyStandardOutput("SR " + av.Name + " " + Helpers.VecDist(av.Position, Client.Self.Position), false);
                }
                EventSender.Instance.NotifyStandardOutput("SR in avatarnames_in_range: ", false);
                foreach (Avatar av in this.avatarnames_in_range)
                {
                    EventSender.Instance.NotifyStandardOutput("SR " + av.Name + " " + Helpers.VecDist(av.Position, Client.Self.Position), false);
                }
                return String.Empty;
            }
            else
            {
                return Description;
            }
        }

        public override void Think()
        {
            //Sicherheitsüberprüfung, ob Agent oor war
            if (!Client.agentOutOfRadius)
            {
                int avatarcount = 0;
                foreach (Avatar av in Client.AvatarList.Values)
                {
                    //Scan for users in radius, user must not be the agent itself
                    if (av.ID != Client.Self.ID && Helpers.VecDist(av.Position, Client.Self.Position) < Client.ACTION_RADIUS)
                    {
                        //Anzahl User mitzaehlen, falls am Ende der Forschleife ungleich zu avatarnames_in_range,
                        // dann sind welche aus der Range verschwunden
                        avatarcount++;
                        //Wenn neuer User in Range auftritt, füge ihn der Liste hinzu
                        if (!this.avatarnames_in_range.Contains(av))
                        {
                            this.avatarnames_in_range.Add(av);
                            EventSender.Instance.NotifyStandardOutput("ScanRadius: " + av.Name + " added.", false);
                            EventSender.Instance.NotifyScanRadius(Client.Self.FirstName + " " + Client.Self.LastName, true, av);
                        }
                    }
                }
                //Falls User aus Range verschwunden, muss dies ungleich sein!
                if (avatarcount != this.avatarnames_in_range.Count)
                {
                    foreach (Avatar av in this.avatarnames_in_range)
                    {
                        // User gar nicht mehr in Avatarliste
                        if (!Client.AvatarList.ContainsValue(av))
                        {
                            this.avatarnames_in_range.Remove(av);
                            EventSender.Instance.NotifyStandardOutput("ScanRadius: " + av.Name + " removed (not in Avatarlist anymore).", false);
                            EventSender.Instance.NotifyScanRadius(Client.Self.FirstName + " " + Client.Self.LastName, false, av);
                        }
                        //falls noch in Avatarliste, aber nicht mehr in Range
                        else
                        {
                            if (Helpers.VecDist(av.Position, Client.Self.Position) > Client.ACTION_RADIUS)
                            {
                                this.avatarnames_in_range.Remove(av);
                                EventSender.Instance.NotifyStandardOutput("ScanRadius: " + av.Name + " removed (oor).", false);
                                EventSender.Instance.NotifyScanRadius(Client.Self.FirstName + " " + Client.Self.LastName, false, av);
                            }
                        }

                    }
                }
                base.Think();
            }
        }

    }
}
