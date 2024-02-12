using System;
using Newtonsoft.Json;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class ConfigWriter
    {
        public static Action<Config> ConfigR;
        public static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "Config.json");
        public static Config Read(string Path)
        {
            bool flag = !File.Exists(Path);
            Config result;
            if (flag)
            {
                result = new Config
                {

                };
            }
            else
            {
                using (FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    result = Read(fileStream);
                }
            }
            return result;
        }
        public static void WriteConfig()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(MainSDK.BotConfig));
            ReloadConfig();
        }
        public static void ReloadConfig()
        {
            try
            {
                MainSDK.BotConfig = Read(ConfigPath);
                Write(ConfigPath,MainSDK.BotConfig);
            }
            catch (Exception ex)
            {

            }
        }
        public static Config Read(Stream stream)
        {
            Config result;
            using (StreamReader streamReader = new StreamReader(stream))
            {
                Config configFile = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
                bool flag = ConfigR != null;
                if (flag)
                {
                    ConfigR(configFile);
                }
                result = configFile;
            }
            return result;
        }

        public static void Write(string Path,Config config)
        {
            using (FileStream fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                string value = JsonConvert.SerializeObject(config, Formatting.Indented);
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(value);
                }
            }
        }
    }
}
