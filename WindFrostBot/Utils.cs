using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
