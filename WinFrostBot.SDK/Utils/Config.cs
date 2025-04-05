using System;

namespace WindFrostBot.SDK
{
    public class Config
    {
        public bool SoraLog = true;
        public bool IsServer = false;
        public string BotName = "WF";
        public bool EnableLog = true;
        public string HostIP = "127.0.0.1";
        public ushort Port = 8081;
        public string SQLtype = "sqlite";
        public string MySqlHost = "";
        public string MySqlDbName = "";
        public string MySqlUsername = "";
        public string MySqlPassword = "";
        public List<long> Admins = new List<long>();
        public List<long> Owners = new List<long>();
        public List<long> QGroups = new List<long>();
    }
}
