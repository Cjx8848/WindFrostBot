using System;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public static class Utils
    {
        public static bool ReloadPlugin(this Plugin plugin)
        {
            if (string.IsNullOrEmpty(plugin.PluginPath))
            {
                return false;
            }
            string path = plugin.PluginPath;
            string originname = plugin.PluginName();
            PluginLoader.UnloadPlugin(path);
            return PluginLoader.LoadPlugin(path);
        }
    }
}
