using System;

namespace WindFrostBot.SDK
{
    public class Config
    {
        public string BotName = "WF";
        public bool EnableLog = true;
        public string HostIP = "127.0.0.1";
        public ushort Port = 8081;
        public List<long> Admins = new List<long>();
        public List<long> QGroups = new List<long>();
    }
}
