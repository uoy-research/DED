using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    /*
    [Serializable()]
    public class SayStruct
    {
        public string agent = String.Empty;
        public string user = String.Empty;
        public string message = String.Empty;
    }
    */
    public class ListenCommand : Command
    {
        public ListenCommand(TestClient testClient)
        {
            Name = "listen";
            Description = "listen to chat";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (!Active)
            {
                Active = true;
                Client.Self.OnChat += new MainAvatar.ChatCallback(Self_OnChat);
                //SocketClient.Instance.setClient(this.Client.ClientManager);
                //SocketClient.Instance.openConnection();
                return "Listening is now on.";
            }
            else
            {
                Active = false;
                Client.Self.OnChat -= new MainAvatar.ChatCallback(Self_OnChat);
                //SocketClient.Instance.closeConnection();
                return "Listening is now off.";
            }
        }

        void Self_OnChat(string message, MainAvatar.ChatAudibleLevel audible, MainAvatar.ChatType type,
            MainAvatar.ChatSourceType sourcetype, string fromName, LLUUID id, LLUUID ownerid, LLVector3 position)
        {
            if (message.Length > 0 && fromName != Client.Self.FirstName + " " + Client.Self.LastName)
                System.Diagnostics.Debug.WriteLine(fromName+":  "+message);
            if (message.Length > 0 && fromName != Client.Self.FirstName + " " + Client.Self.LastName && sourcetype == MainAvatar.ChatSourceType.Agent && type == MainAvatar.ChatType.Normal)
            {
                //Ausschliessen, dass Avatar zu weit entfernt ist, dabie gibt es zwei Fälle
                //1: FollowRadius aktiv -> Homeposition ist gesetzt
                //2: inaktiv, d.h. Avatar steht immer auf selber position
                //Homeposition ist gesetzt.
                //Wenn weiter entfernt als ActionRadius + minimaler Avatarabstand
                //dann nicht auf Sätze reagieren
                if (Client.position != LLVector3.Zero
                    && position.GetDistanceTo(Client.position) > Client.FOLLOW_RADIUS_INNER + Client.ACTION_RADIUS)
                {
                    return;
                }
                //HomePosition nicht gesetzt -> Workaraound
                else if (Client.position == LLVector3.Zero
                    && position.GetDistanceTo(Client.Self.Position) > Client.FOLLOW_RADIUS_INNER + Client.ACTION_RADIUS)
                {
                    return;
                }
                EventSender.Instance.NotifyListen(Client.Self.FirstName + " " + Client.Self.LastName, fromName, message);
            }
        }
    }
}
