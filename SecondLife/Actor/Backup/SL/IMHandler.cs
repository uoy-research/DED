using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace DED
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
                Action m = new Action(client);
                m.Teleport(im.Message.Substring(9).Trim());
                    
            }
            else if (string.Compare(im.Message.ToString(), 0, "stand", 0, 4) == 0)
            {
                Action m = new Action(client);
                m.StandUp();
            }
            else if (string.Compare(im.Message.ToString(), 0, "sit", 0, 3) == 0)
            {
                Action m = new Action(client);
                m.Sit(im.Message.Substring(3).Trim());
            }

            else if (string.Compare(im.Message.ToString(), 0, "yaw", 0, 3) == 0)
            {
                Action m = new Action(client);

                m.Yaw(im.Message.Substring(3).Trim());
            }
            else if (string.Compare(im.Message.ToString(), 0, "pitch", 0, 5) == 0)
            {
                Action m = new Action(client);

                m.Pitch(im.Message.Substring(5).Trim());
            }

            else if (string.Compare(im.Message.ToString(), 0, "head", 0, 5) == 0)
            {
                client.Self.Movement.Camera.Pitch((float)Convert.ToDouble(im.Message.Substring(5).Trim()));
                client.Self.Movement.SendUpdate();
            }
            else if (string.Compare(im.Message.ToString(), 0, "look", 0, 4) == 0)
            {
                Action m = new Action(client);
                m.LookToward(im.Message.Substring(5).Trim());
                //m.LookAT(im.Message.Substring(5).Trim());
            }
            
            else if (string.Compare(im.Message.ToString(), 0, "search", 0, 6) == 0)
            {
                Search s = new Search(client);
                List<Primitive> prims = s.FindAllInRadius(250);
                foreach (Primitive p in prims)
                {
                    Console.WriteLine("Prim Name '{0}', ID '{1}', LocalID '{2}', Position {3}"
                            , p.Properties.Name, p.ID, p.LocalID, p.Position);
                    if ((p.Position.X > 90 && p.Position.X < 102 && p.Position.Y > 140 && p.Position.Y < 155) || (p.Position.X < 1))
                    { 
                        Console.WriteLine("Match prims");
                    }
                }
            }
            
            else if (string.Compare( im.Message.ToString(),0, "goto",0,4) == 0 ) {
                Action m = new Action(client);
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
