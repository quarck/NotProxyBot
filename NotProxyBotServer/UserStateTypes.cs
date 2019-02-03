using System;
using System.Collections.Generic;
using System.Text;

namespace NotProxyBotServer
{
    [Serializable]
    public class AuthValidFlag
    {
        public bool AuthValid = true;
    }

    [Serializable]
    public class RssEntry
    {
        public string Url;
        public string[] Keywords;
    }

    [Serializable]
    public class UserRssSubscriptions
    { 
        public List<RssEntry> RssEntries; 
    }

    [Serializable]
    public class UserMuteState
    {
        public bool Muted;
        public bool Stopped;
    }
}
