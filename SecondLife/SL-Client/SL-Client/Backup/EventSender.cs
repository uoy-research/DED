using System;
using System.Collections.Generic;
using System.Text;

namespace libsecondlife.TestClient
{
    public class NewListenEventArgs : EventArgs
    {
        public NewListenEventArgs(string agent, string user, string message)
        {
            this.agent = agent;
            this.user = user;
            this.message = message;
        }
        public string Message
        {
            get
            {
                return (message);
            }
        }
        public string User
        {
            get
            {
                return (user);
            }
        }
        public string Agent
        {
            get
            {
                return (agent);
            }
        }
        string message;
        string user;
        string agent;

    }

    public class NewScanRadiusEventArgs : EventArgs
    {
        public NewScanRadiusEventArgs(string agentname, bool add, Avatar user)
        {
            this.agentname = agentname;
            this.add = add;
            this.user = user;
        }
        public Avatar User
        {
            get
            {
                return (user);
            }
        }
        public bool Add
        {
            get
            {
                return (add);
            }
        }
        public string AgentName
        {
            get
            {
                return (agentname);
            }
        }
        Avatar user;
        bool add;
        string agentname;

    }

    public class NewStandardOutputEventArgs : EventArgs
    {
        public NewStandardOutputEventArgs(string message, bool isError)
        {
            this.message = message;
            this.isError = isError;
        }
        public string Message
        {
            get
            {
                return (message);
            }
        }
        string message;

        public bool IsError
        {
            get
            {
                return (isError);
            }
        }
        bool isError;

    }

    public class NewLoginEventArgs : EventArgs
    {
        public NewLoginEventArgs(bool isLoggedIn)
        {
            this.isLoggedIn = isLoggedIn;
        }
        public bool IsLoggedIn
        {
            get
            {
                return (isLoggedIn);
            }
        }
        bool isLoggedIn;

    }

    public class NewFollowSuccessfulEventArgs : EventArgs
    {
        public NewFollowSuccessfulEventArgs(string agentname, Avatar user)
        {
            this.agentname = agentname;
            this.user = user;
        }
        public Avatar User
        {
            get
            {
                return (user);
            }
        }
        public string AgentName
        {
            get
            {
                return (agentname);
            }
        }
        Avatar user;
        string agentname;

    }
    public sealed class EventSender
    {
        static readonly EventSender instance = new EventSender();

        public EventSender()
        {

        }
        
        static EventSender()
        {
        }
        public static EventSender Instance
        {
            get
            {
                return instance;
            }
        }

        public delegate void NewListenEventHandler(object sender, NewListenEventArgs e);
        public event NewListenEventHandler OnNewListenHandler;

        public delegate void NewScanRadiusEventHandler(object sender, NewScanRadiusEventArgs e);
        public event NewScanRadiusEventHandler OnNewScanRadiusHandler;

        public delegate void NewStandardOutputEventHandler(object sender, NewStandardOutputEventArgs e);
        public event NewStandardOutputEventHandler OnNewStandardOutputHandler;

        public delegate void NewLoginEventHandler(object sender, NewLoginEventArgs e);
        public event NewLoginEventHandler OnNewLoginHandler;

        public delegate void NewFollowSuccessfulEventHandler(object sender, NewFollowSuccessfulEventArgs e);
        public event NewFollowSuccessfulEventHandler OnNewFollowSuccessfulHandler;

        public void OnNewListen(NewListenEventArgs e)
        {
            if (OnNewListenHandler != null)
                OnNewListenHandler(this, e);
        }
        public void OnNewScanRadius(NewScanRadiusEventArgs e)
        {
            if (OnNewScanRadiusHandler != null)
                OnNewScanRadiusHandler(this, e);
        }
        public void OnNewStandardOutput(NewStandardOutputEventArgs e)
        {
            if (OnNewStandardOutputHandler != null)
                OnNewStandardOutputHandler(this, e);
        }
        public void OnNewLogin(NewLoginEventArgs e)
        {
            if (OnNewLoginHandler != null)
                OnNewLoginHandler(this, e);
        }
        public void OnNewFollowSuccessful(NewFollowSuccessfulEventArgs e)
        {
            if (OnNewFollowSuccessfulHandler != null)
                OnNewFollowSuccessfulHandler(this, e);
        }

        public void NotifyStandardOutput(string message, bool isError)
        {
            NewStandardOutputEventArgs e = new NewStandardOutputEventArgs(message, isError);
            OnNewStandardOutput(e);
        }
        public void NotifyLogin (bool isLoggedIn)
        {
            NewLoginEventArgs e = new NewLoginEventArgs(isLoggedIn);
            OnNewLogin(e);
        }
        public void NotifyListen(string agent, string user, string message)
        {
            NewListenEventArgs e = new NewListenEventArgs(agent, user, message);
            OnNewListen(e);
        }
        /// <summary>
        /// Leitet weiter, dass ein neuer User im Radius ist/ein User weg ist
        /// </summary>
        /// <param name="agentname">Agent</param>
        /// <param name="add">true := add; false :=  remove</param>
        /// <param name="user">Avatar</param>
        public void NotifyScanRadius(string agentname, bool add, Avatar user)
        {
            NewScanRadiusEventArgs e = new NewScanRadiusEventArgs(agentname, add, user);
            OnNewScanRadius(e);
        }
        public void NotifyFollowSuccessfulRadius(string agentname, Avatar user)
        {
            NewFollowSuccessfulEventArgs e = new NewFollowSuccessfulEventArgs(agentname, user);
            OnNewFollowSuccessful(e);
        }
    }
}
