using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;

namespace MyFirstBot
{
    class MyFirstBot
    {
        public static SecondLife client = new SecondLife();

        ////Loggon vars///
        private static string first_name = "Snorri";
        private static string last_name = "Haggwood";
        private static string password = "steingeit";
        
        //LLUUID VallaID = 

        public static void Main()
        {
            client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            //string startLocation = NetworkManager.StartLocation("Korea3", 99, 140, 27);
            string startLocation = NetworkManager.StartLocation("University of York", 94, 88, 27);
            if (client.Network.Login(first_name, last_name, password, "My First Bot", startLocation, "Your name"))
            {
                Console.WriteLine("I logged into Second Life!");
                Console.ReadKey();
            }
            else

            {
                Console.WriteLine("I couldn't log in, here is why: " + client.Network.LoginMessage);
                Console.ReadKey();
            }
        }

        private static void Network_OnConnected(object sender)
        {
            client.Appearance.SetPreviousAppearance(false);
            client.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated); //Callback for sims object update event, this is where we grab the coordinates.
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            Console.WriteLine("I'm connected to the simulator, I'm going to search for Valla Haggwood");

            LLUUID VallaID = client.Directory.StartPeopleSearch(DirectoryManager.DirFindFlags.People, "Valla Haggwood", 0);
            Console.WriteLine("Valla Haggwoods ID is: " + VallaID.ToString());
            client.Self.InstantMessage(VallaID, "Hello, Valla!");
            //client.Self.Chat(message,channel,type)
            //message (string) What the bot is to say
            //channel (int) What channel the bot is to say the message on (use 0 for public chat)
            //type (ChatType) How to say 'message' on 'channel'. Normal = 20m, Whisper = 10m, Shout = 100m
            client.Self.Chat("Hello World!", 0, ChatType.Normal);

            client.Settings.
            
            //LLUUID target = new LLUUID("243dd044-1127-11dc-8314-0800200c9a66");
            //That key goes no where, replace it with the avatar key of the person you want to IM

            
            
            
            //client.Self.InstantMessage(VallaID, "Valla, I'm going away now, bye bye!");
            //Console.WriteLine("Now I am going to logout of SL.. Goodbye!");
            //client.Network.Logout();
        }

        private static void Self_OnInstantMessage(InstantMessage im, Simulator sim)
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

        

        private static void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, LLUUID id, LLUUID ownerid, LLVector3 position)
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

        private static void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation) //Callback for object updates in simulator
        {
            
        }        

        //Definition of Self_OnTeleport:
        private static void Self_OnTeleport(string message, AgentManager.TeleportStatus status, AgentManager.TeleportFlags flags)
        {
            //message contains any messages regarding the teleport
            //status is an enum of the current teleport status
            //flags is various flags regarding the teleport
            
            Console.WriteLine(first_name+" "+last_name+" teleported ");
            Console.WriteLine("Teleport Message: "+message+". Status: "+status+". Flags: "+flags);
        }
    }
}
