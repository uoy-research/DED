using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using log4net;
using log4net.Config;

namespace DED
{
    class Avatar 
    {
        public SecondLife client = null;

        ////Loggon vars///
        private string first_name = "";
        private string last_name = "";
        private string password = "";
        private string startLocation = NetworkManager.StartLocation("University of York", 94, 88, 27);
        private static readonly ILog log = LogManager.GetLogger(typeof(DED));

        public string FirstName { get { return this.first_name; } }
        public string LastName { get { return this.last_name; } }

        public Avatar(string first_name, string last_name, string password)
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.password = password;
            if (this.client != null) this.client = null;
            this.client = new SecondLife();

            this.client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);       
            
        }

        public void Login()
        {
            if (this.client.Network.Login(this.first_name, this.last_name, this.password, "My First Bot", this.startLocation, "Your name"))
            {
                Console.WriteLine("{0} {1}I logged into Second Life!", first_name, last_name);
            }
            else
            {
                Console.WriteLine("I couldn't log in, here is why: " + this.client.Network.LoginMessage);
            }
        }

        public void GameControler()
        {
            Action a = new Action(this.client);
            //Initial position          
            a.ToPosition("61.143.25");
            a.ToPosition("94.138.26");
            //To the table
            a.ToPosition("96.146.26");
            a.LookToward("97.146.25");
            System.Threading.Thread.Sleep(2000);
            client.Self.Chat(client.Self.FirstName+" Pour drinks", 0, ChatType.Normal);
            System.Threading.Thread.Sleep(2000);
            //Leave
            a.ToPosition("94.138.26");
            System.Threading.Thread.Sleep(10000);
            //To the platform 
            a.ToPosition("97.144.26");
            a.LookToward("97.146.25");
            client.Self.Chat(client.Self.FirstName + " State brand names", 0, ChatType.Normal);
            System.Threading.Thread.Sleep(2000);       
            
        }


        public void Contestant()
        {
            Action a = new Action(this.client);
            //Initial position
            a.ToPosition("61.143.25");
            a.ToPosition("91.138.26");
            a.ToPosition("94.144.26");
            //If its his turn
                a.Sit("5c9cd0bf-22bf-2a42-5877-ce225b83aff6", "94.146.25");
                for (int i = 0; i < 3; ++i )
                {
                    System.Threading.Thread.Sleep(2000);
                    client.Self.Chat(client.Self.FirstName + " SIPPING", 0, ChatType.Normal);
                    System.Threading.Thread.Sleep(2000);
                    client.Self.Chat(client.Self.FirstName + " State brand name", 0, ChatType.Normal);
                    System.Threading.Thread.Sleep(2000);
                }
                a.StandUp();
                a.ToPosition("101.146.26");
                a.LookToward("98.146.26");
            //end if
        }

        public void CaseStudy(){
            Console.WriteLine("Case study Enter!");
            Action a = new Action(this.client);
            a.ToPosition("91.138.26");
            a.ToPosition("94.145.26");
            a.Sit("5c9cd0bf-22bf-2a42-5877-ce225b83aff6", "94.146.25");
        }

        private void Network_OnConnected(object sender)
        {
            client.Appearance.SetPreviousAppearance(false);
            client.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated); //Callback for sims object update event, this is where we grab the coordinates.
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            log.Info("I'm connected to the simulator");

            //LLUUID VallaID = client.Directory.StartPeopleSearch(DirectoryManager.DirFindFlags.People, "Valla Haggwood", 0);
            //Console.WriteLine("Valla Haggwoods ID is: " + VallaID.ToString());
            //client.Self.InstantMessage(VallaID, "Hello, Valla!");
            //client.Self.Chat(message,channel,type)
            //message (string) What the bot is to say
            //channel (int) What channel the bot is to say the message on (use 0 for public chat)
            //type (ChatType) How to say 'message' on 'channel'. Normal = 20m, Whisper = 10m, Shout = 100m
            client.Self.Chat("Hello World!", 0, ChatType.Normal);

           
            
            //LLUUID target = new LLUUID("243dd044-1127-11dc-8314-0800200c9a66");
            //That key goes no where, replace it with the avatar key of the person you want to IM

            
            
            
            //client.Self.InstantMessage(VallaID, "Valla, I'm going away now, bye bye!");
            //Console.WriteLine("Now I am going to logout of SL.. Goodbye!");
            //client.Network.Logout();
        }

        private void Self_OnInstantMessage(InstantMessage im, Simulator sim)
        {
            string[] friends = null;
            IMHandler imhandler = new IMHandler(client, im, sim, friends);

            switch (im.Dialog)
            {
                case InstantMessageDialog.MessageFromAgent:
                    imhandler.RespondToMessageFromAgent();                    
                    break;
                case InstantMessageDialog.FriendshipOffered:
                    imhandler.AcceptFriendshipOffer();
                    break;
                case InstantMessageDialog.GroupInvitation:
                    imhandler.AcceptGroupInvitation();
                    break;
                case InstantMessageDialog.InventoryOffered:
                    imhandler.AcceptInventoryOffered();
                    break;
                // someone sent a teleport lure
                case InstantMessageDialog.RequestTeleport:
                    imhandler.AcceptTeleportRequest();
                    break;
                default:
                    break;
            }
        }

        

        private void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, LLUUID id, LLUUID ownerid, LLVector3 position)
        {
                //message (string) What was said
                //audible (not used)
                //type (ChatType) How it was said (see above)
                //sourceType (ChatSourceType) What kind of thing said it (System, Object, or Avatar)
                //fromName (string) Whats the name of the person/object that said it
                //id (LLUUID) Whats the key of the person/object that said it
                //ownerid (LLUUID) If it was an object, who owns it?
                //position (LLVector3) Where is the person/object that said it 
            switch (sourceType){
                case ChatSourceType.System:
                    Console.WriteLine("System Message: " + message);
                    break;
                case ChatSourceType.Object:
                    Console.WriteLine("Object " + fromName + " Message: " + message);
                    break;
                case ChatSourceType.Agent:
                    Console.WriteLine("Agent " + fromName + " Message: " + message);
                    break;
                default:
                    break;
            }
        }

        private void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation) //Callback for object updates in simulator
        {
            
        }        

        //Definition of Self_OnTeleport:
        private void Self_OnTeleport(string message, AgentManager.TeleportStatus status, AgentManager.TeleportFlags flags)
        {
            //message contains any messages regarding the teleport
            //status is an enum of the current teleport status
            //flags is various flags regarding the teleport
            
            Console.WriteLine(first_name+" "+last_name+" teleported ");
            Console.WriteLine("Teleport Message: "+message+". Status: "+status+". Flags: "+flags);
        }
    }
}
