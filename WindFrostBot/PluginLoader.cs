using System.Reflection;
using WindFrostBot.SDK;
namespace WindFrostBot;

public static class PluginLoader
{
    public static readonly string PluginsDirectory = Path.Combine(AppContext.BaseDirectory, "Plugins");
    public static readonly Dictionary<string, Assembly> LoadedAssemblies = new();
    public static readonly List<Plugin> Plugins = new();
    private static FileSystemWatcher? _watcher;
    public static List<string> RemovedPluginPaths = new List<string>();
    public static List<string> ChangedPluginPaths = new List<string>();
    public static List<string> NewPluginPaths = new List<string>();
    public static void ReloadLoadChange()
    {
        var copyremove = new List<string>(RemovedPluginPaths);
        var copychanged = new List<string>(ChangedPluginPaths);
        foreach (var path in copyremove)
        {
            UnloadPlugin(path);
        }
        foreach (var path in copychanged)
        {
            UnloadPlugin(path);
            LoadPlugin(path);
        }
        foreach(var path in NewPluginPaths)
        {
            LoadPlugin(path);
        }
    }
    public static void Init()
    {
        if (!Directory.Exists(PluginsDirectory))
        {
            Directory.CreateDirectory(PluginsDirectory);
        }
        _watcher = new FileSystemWatcher(PluginsDirectory, "*.dll")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
        };
        _watcher.Changed += OnPluginChanged;
        _watcher.Created += OnPluginChanged;
        _watcher.Deleted += OnPluginDeleted;
        _watcher.EnableRaisingEvents = true;
    }
    private static void OnPluginDeleted(object sender, FileSystemEventArgs e)
    {
        try
        {
            if (LoadedAssemblies.ContainsKey(e.FullPath))
            {
                if (ChangedPluginPaths.Contains(e.FullPath))
                {
                    ChangedPluginPaths.Remove(e.FullPath);
                }
                if (!RemovedPluginPaths.Contains(e.FullPath))
                {
                    RemovedPluginPaths.Add(e.FullPath);
                }
            }
            else
            {
                if (NewPluginPaths.Contains(e.FullPath))
                {
                    NewPluginPaths.Remove(e.FullPath);
                }
            }
        }
        catch (Exception ex)
        {
            Message.Erro($"卸载插件时出错: {ex.Message}");
        }
    }
    private static void OnPluginChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
            {
                if (!LoadedAssemblies.ContainsKey(e.FullPath))
                {
                    if (!ChangedPluginPaths.Contains(e.FullPath))
                    {
                        ChangedPluginPaths.Add(e.FullPath);
                    }
                }
                else
                {
                    if (!NewPluginPaths.Contains(e.FullPath))
                    {
                        NewPluginPaths.Add(e.FullPath);
                    }
                }
            }
        }
        catch
        {

        }
    }
    public static bool LoadPlugin(string pluginPath)
    {
        if (LoadedAssemblies.TryGetValue(pluginPath, out _))
            return false;

        Assembly assembly;
        try
        {
            byte[] assemblyData = File.ReadAllBytes(pluginPath);
            assembly = Assembly.Load(assemblyData);
            LoadedAssemblies.Add(pluginPath, assembly);
            ReloadPluginInstances(assembly, pluginPath);
            if (NewPluginPaths.Contains(pluginPath))
            {
                NewPluginPaths.Remove(pluginPath);
            }
            return true;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }
    static void ReloadPluginInstances(Assembly assembly, string path)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (!type.IsSubclassOf(typeof(Plugin)) || !type.IsPublic || type.IsAbstract)
                continue;

            Plugin pluginInstance;
            try
            {
                pluginInstance = (Activator.CreateInstance(type) as Plugin)!;
                pluginInstance.PluginPath = path;
                Plugins.Add(pluginInstance);
                pluginInstance.OnLoad();
                Message.LogInfo($"插件 {pluginInstance.PluginName()} 已热重载!");

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载插件 \"{type.FullName}\" 时出错.", ex);
            }
        }
    }
    public static void LoadPlugins()
    {
        void CreateAndAddPluginInstances(Assembly assembly, string path = "")
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (!type.IsSubclassOf(typeof(Plugin)) || !type.IsPublic || type.IsAbstract)
                    continue;

                Plugin pluginInstance;
                try
                {
                    pluginInstance = (Activator.CreateInstance(type) as Plugin)!;
                    pluginInstance.PluginPath = path;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"加载插件 \"{type.FullName}\" 时出错.", ex);
                }

                Plugins.Add(pluginInstance);
            }
        }
        Init();
        CreateAndAddPluginInstances(Assembly.GetExecutingAssembly());
        var pluginPaths = Directory.GetFiles(PluginsDirectory, "*.dll");
        foreach (var pluginPath in pluginPaths)
            try
            {
                if (LoadedAssemblies.TryGetValue(pluginPath, out _))
                    continue;

                Assembly assembly;
                try
                {
                    byte[] assemblyData = File.ReadAllBytes(pluginPath);
                    assembly = Assembly.Load(assemblyData);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
                LoadedAssemblies.Add(pluginPath, assembly);
                CreateAndAddPluginInstances(assembly, pluginPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载 \"{pluginPath}\" 失败.", ex);
            }
        foreach (var p in Plugins)
        {
            try
            {
                p.OnLoad();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"插件 \"{p.PluginName()}\" 在初始化时出错.", ex);
            }
            Message.LogInfo($"{p.PluginName()} Version:{p.Version()}(by {p.Author()}) 成功加载!");
        }
        Message.LogInfo($"总共加载 {Plugins.Count} 个插件.");
    }
    public static void UnloadPlugin(string pluginPath)
    {
        if (LoadedAssemblies.TryGetValue(pluginPath, out var assembly))
        {
            var pluginsToRemove = Plugins.Where(p => p.GetType().Assembly == assembly).ToList();

            foreach (var plugin in pluginsToRemove)
            {
                try
                {
                    plugin.OnDispose();
                    Message.LogInfo($"插件 {plugin.PluginName()} 已卸载.");
                }
                catch (Exception ex)
                {
                    Message.Erro($"卸载插件 \"{plugin.PluginName()}\" 时出错: {ex.Message}");
                }
                Plugins.Remove(plugin);
            }
            LoadedAssemblies.Remove(pluginPath); 
            if (ChangedPluginPaths.Contains(pluginPath))
            {
                ChangedPluginPaths.Remove(pluginPath);
            }
            if (RemovedPluginPaths.Contains(pluginPath))
            {
                RemovedPluginPaths.Remove(pluginPath);
            }
        }
    }
}