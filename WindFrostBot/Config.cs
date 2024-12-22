using System;
using JsonTool;
using Newtonsoft.Json;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class ConfigWriter
    {
        public static JsonRw<Config> Config;
        public static Config GetConfig()
        {
            return MainSDK.BotConfig;
        }
        public static void ReadConfig()
        {
            Config.ReadConfig();
            MainSDK.BotConfig = Config.ConfigObj;
        }
        public static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "Config.json");
        public static void InitConfig()
        {
            var setting = new Config();
            var json = new JsonRw<Config>(ConfigPath, setting);
            Config = json;
            MainSDK.BotConfig = json.ConfigObj;
        }
    }
}
