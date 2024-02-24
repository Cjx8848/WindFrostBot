using System;
using System.Data;
using Sora.Interfaces;

namespace WindFrostBot.SDK
{
    public class MainSDK
    {
        public static Config BotConfig { get; set; }
        public static ISoraService service { get; set; }
        public static IDbConnection Db { get; set; }
    }
    public abstract class Plugin
    {
        //public List<IPrivateCommand> RegisteredPrivateCommands { get; } = new();
        public List<Command> Commands = new List<Command>();
        public abstract string PluginName();
        public abstract string Version();
        public abstract string Author();
        public abstract string Description();
        public abstract void OnLoad();
    }
}