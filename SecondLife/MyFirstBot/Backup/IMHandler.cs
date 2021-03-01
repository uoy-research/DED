using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class IMHandler
    {
        private SecondLife client = null;
        private InstantMessage im;
        private Simulator sim = null;
        private Array friends = null;

        public IMHandler(SecondLife client, InstantMessage im, Simulator sim, string[] friends)
        {
            this.client = client;
            this.im = im;
            this.sim = sim;
            this.friends = friends;
        }

        public void RespondToMessageFromAgent()
        {

            if (string.Equals(im.Message.ToString(), "right")) {  }
            else if (string.Equals(im.Message.ToString(), "left")) {  }
            else if (string.Compare(im.Message.ToString(), 0, "teleport", 0, 8) == 0)
            {
                Move m = new Move(client);
                m.Teleport(im.Message.Substring(9).Trim());
                    
            }
            else if (string.Compare(im.Message.ToString(), 0, "turn", 0, 4) == 0)
            {
                Move m = new Move(client);
                m.ToCoordinates(im.Message.Substring(5).Trim());
            }
            else if (string.Compare( im.Message.ToString(),0, "goto",0,4) == 0 ) {
                Move m = new Move(client);
                m.ToCoordinates(im.Message.Substring(5).Trim());
            }
            else if  ( string.Equals(im.Message.ToString(), "loggout")) { client.Network.Logout(); }
            else
            {
                client.Self.InstantMessage(im.FromAgentID, im.Message, im.IMSessionID);
            }
            
            //send them an instant message back (this thing will copy any message the bot recieves in an IM)
        }

        public void AcceptFriendshipOffer( )
        {
            client.Friends.AcceptFriendship(im.FromAgentID, im.IMSessionID); // Accept Friendship Offer
            //client.Friends.DeclineFriendship(im.FromAgentID, im.IMSessionID); // Decline Friendship Offer
        }

        public void AcceptGroupInvitation()
        {
            client.Self.InstantMessage(client.Self.Name, im.FromAgentID, string.Empty, im.IMSessionID, InstantMessageDialog.GroupInvitationAccept, InstantMessageOnline.Offline, client.Self.SimPosition, LLUUID.Zero, new byte[0]); // Accept Group Invitation (Join Group)
            //client.Self.InstantMessage(client.Self.Name, im.FromAgentID, string.Empty, im.IMSessionID, InstantMessageDialog.GroupInvitationDecline, InstantMessageOnline.Offline, client.Self.SimPosition, LLUUID.Zero, new byte[0]); // Decline Group Invitation
        }

        public void AcceptInventoryOffered()
        {
            client.Self.InstantMessage(client.Self.Name, im.FromAgentID, String.Empty, im.IMSessionID, InstantMessageDialog.InventoryAccepted, InstantMessageOnline.Offline, client.Self.SimPosition, LLUUID.Zero, new byte[0]); // Accept Inventory Offer
            //Client.Self.InstantMessage(client.Self.Name, im.FromAgentID, string.Empty, im.IMSessionID, InstantMessageDialog.InventoryDeclined, InstantMessageOnline.Offline, client.Self.SimPosition, LLUUID.Zero, new byte[0]); // Decline Inventory Offer               
        }

        public void AcceptTeleportRequest()
        {
            client.Self.TeleportLureRespond(im.FromAgentID, true); //Accept
            //client.Self.TeleportLureRespond(im.FromAgentID, false); //Decline
        }

        
    }
}
