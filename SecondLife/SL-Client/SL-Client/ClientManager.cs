using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Threading;
using libsecondlife;
using libsecondlife.Packets;
//using libsecondlife.AssetSystem;

namespace libsecondlife.TestClient
{
    public class LoginDetails
    {
        public string FirstName;
        public string LastName;
        public string Password;
        public string StartLocation;
        public string MasterName;
        public LLUUID MasterKey;
        public string URI;
    }

    public class StartPosition
    {
        public string sim;
        public int x;
        public int y;
        public int z;

        public StartPosition()
        {
            this.sim = null;
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
    }

    public class ClientManager
    {
        public Dictionary<LLUUID, SecondLife> Clients = new Dictionary<LLUUID, SecondLife>();
        public Dictionary<Simulator, Dictionary<uint, Primitive>> SimPrims = new Dictionary<Simulator, Dictionary<uint, Primitive>>();

        public bool Running = true;

        string contactPerson = String.Empty;
        private LLUUID resolvedMasterKey = LLUUID.Zero;
        private ManualResetEvent keyResolution = new ManualResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accounts"></param>
        public ClientManager(List<LoginDetails> accounts, string c)
        {
            this.contactPerson = c;
            foreach (LoginDetails account in accounts)
                Login(account);
        }

        public ClientManager(List<LoginDetails> accounts, string c, string s)
        {
            this.contactPerson = c;
            char sep = '/';
            string[] startbits = s.Split(sep);

            foreach (LoginDetails account in accounts)
            {
                account.StartLocation = NetworkManager.StartLocation(startbits[0], Int32.Parse(startbits[1]),
                    Int32.Parse(startbits[2]), Int32.Parse(startbits[3]));
                Login(account);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public TestClient Login(LoginDetails account)
        {
            // Check if this client is already logged in
            foreach (TestClient c in Clients.Values)
            {
                if (c.Self.FirstName == account.FirstName && c.Self.LastName == account.LastName)
                {
                    Logout(c);
                    break;
                }
            }

            TestClient client = new TestClient(this);

            // Optimize the throttle
            client.Throttle.Wind = 0;
            client.Throttle.Cloud = 0;
            client.Throttle.Land = 1000000;
            client.Throttle.Task = 1000000;

			client.SimPrims = SimPrims;
			client.MasterName = account.MasterName;
            client.MasterKey = account.MasterKey;

            NetworkManager.LoginParams loginParams = client.Network.DefaultLoginParams(
                    account.FirstName, account.LastName, account.Password, "TestClient", contactPerson);
            if (!String.IsNullOrEmpty(account.StartLocation))
                loginParams.Start = account.StartLocation;

            if (!String.IsNullOrEmpty(account.URI))
                loginParams.URI = account.URI;

            if (!client.Network.Login(loginParams))
            {
                EventSender.Instance.NotifyStandardOutput("Failed to login " + account.FirstName + " " + account.LastName + ": " +
                    client.Network.LoginMessage, true);
            }

            if (client.Network.Connected)
            {
                client.MasterKey = account.MasterKey;

                Clients[client.Network.AgentID] = client;
                EventSender.Instance.NotifyLogin(true);
                EventSender.Instance.NotifyStandardOutput("Logged in " + client.ToString(), false);
            }

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TestClient Login(string[] args)
        {
            LoginDetails account = new LoginDetails();
            account.FirstName = args[0];
            account.LastName = args[1];
            account.Password = args[2];

            if (args.Length >= 4)
            {
                account.StartLocation = NetworkManager.StartLocation(args[3], 128, 128, 40);
            }

            return Login(account);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="fromAgentID"></param>
        /// <param name="imSessionID"></param>
        public void DoCommandAll(string cmd, LLUUID fromAgentID, LLUUID imSessionID)
        {
            string[] tokens = cmd.Trim().Split(new char[] { ' ', '\t' });
            string firstToken = tokens[0].ToLower();

            if (tokens.Length == 0)
                return;

            if (firstToken == "login")
            {
                // Special login case: Only call it once, and allow it with
                // no logged in avatars
                string[] args = new string[tokens.Length - 1];
                Array.Copy(tokens, 1, args, 0, args.Length);
                Login(args);
            }
            else if (firstToken == "quit")
            {
                Quit();
                EventSender.Instance.NotifyStandardOutput("All clients logged out and program finished running.", false);
            }
            else
            {
                // make a copy of the clients list so that it can be iterated without fear of being changed during iteration
                Dictionary<LLUUID, SecondLife> clientsCopy = new Dictionary<LLUUID, SecondLife>(Clients);

                foreach (TestClient client in clientsCopy.Values)
                    client.DoCommand(cmd, fromAgentID, imSessionID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public void Logout(TestClient client)
        {
            Clients.Remove(client.Network.AgentID);
            client.Network.Logout();
            EventSender.Instance.NotifyLogin(false);
        }

        public void logoutAllClients()
        {
            foreach (SecondLife client in Clients.Values)
            {
                if (client.Network.Connected)
                    client.Network.Logout();
            }
            EventSender.Instance.NotifyLogin(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LogoutAll()
        {
            // make a copy of the clients list so that it can be iterated without fear of being changed during iteration
            Dictionary<LLUUID, SecondLife> clientsCopy = new Dictionary<LLUUID, SecondLife>(Clients);

            foreach (TestClient client in clientsCopy.Values)
                Logout(client);
            EventSender.Instance.NotifyLogin(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Quit()
        {
            LogoutAll();
            Running = false;
            // TODO: It would be really nice if we could figure out a way to abort the ReadLine here in so that Run() will exit.
        }
    }
}
