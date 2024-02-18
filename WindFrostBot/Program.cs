using System;
using System.Drawing;
using WindFrostBot.SDK;
using Spectre.Console;
using Sora.Net.Config;
using Sora;
using ProtoBuf.Meta;
using YukariToolBox.LightLog;
using System.Threading.Tasks;
using Sora.Util;
using System.Reflection;
using System.Runtime.Loader;

namespace WindFrostBot
{
    public class Program
    {
        static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                string assemblyName = new AssemblyName(eventArgs.Name).Name;
                string path = Path.Combine(AppContext.BaseDirectory, "bin", $"{assemblyName}.dll");

                if (File.Exists(path))
                {
                    return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                }

                return null;
            };
        }
        static void Main(string[] arg)
        {
            Init();
            AnsiConsole.Write(new FigletText("WindFrostBot").Color(Spectre.Console.Color.Aqua));
            ConfigWriter.ReloadConfig();
            Message.LogWriter.StartLog();
            Message.BlueText("WindFrostBot1.0 正在启动...");
            if (MainSDK.BotConfig.EnableLog)
            {
                Message.BlueText("日志功能已开启.");
            }
            if (!File.Exists(PluginLoader.PluginsDirectory))
            {
                Directory.CreateDirectory(PluginLoader.PluginsDirectory);
            }
            PluginLoader.LoadPlugins();
            Message.BlueText("WindFrostBot1.0 启动成功!");
            StartSora();
            for(; ;)
                Console.ReadLine();
        }
        public static  async void StartSora()
        {
            Log.LogConfiguration.EnableConsoleOutput().SetLogLevel(LogLevel.Info);
            MainSDK.service = SoraServiceFactory.CreateService(new ClientConfig()
            {
                Host = MainSDK.BotConfig.HostIP,
                Port = MainSDK.BotConfig.Port
            });
            #region Log
            if (MainSDK.BotConfig.EnableLog)
            {
                MainSDK.service.Event.OnGroupMessage += (sender, eventArgs) =>
                {
                    Message.LogInfo($"收到来自群 {eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo.GroupName}({eventArgs.SourceGroup.Id}) 内 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的消息：{eventArgs.Message.RawText}");
                    return ValueTask.CompletedTask;
                };
                MainSDK.service.Event.OnPrivateMessage += (sender, eventArgs) =>
                {
                    Message.LogInfo($"收到来自 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的私聊消息：{eventArgs.Message.RawText}");
                    return ValueTask.CompletedTask;
                };
                MainSDK.service.Event.OnSelfGroupMessage += (sender, eventArgs) =>
                {
                    Message.LogInfo($"向群 {eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo.GroupName}({eventArgs.SourceGroup.Id}) 发送了消息：{eventArgs.Message.RawText}");
                    return ValueTask.CompletedTask;
                };
                MainSDK.service.Event.OnSelfPrivateMessage += (sender, eventArgs) =>
                {
                    Message.LogInfo($"向 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 发送了私聊消息：{eventArgs.Message.RawText}");
                    return ValueTask.CompletedTask;
                };
            }
            #endregion
            MainSDK.service.Event.OnGroupRequest += async (msgType, eventArgs) =>
            {
                if (Utils.IsAdmin(eventArgs.Sender.Id))
                {
                    await eventArgs.Accept();
                }
            };
            CommandManager.InitCommandToSora();
            await MainSDK.service.StartService().RunCatch(e => Log.Error("Sora Service", Log.ErrorLogBuilder(e)));
            await Task.Delay(-1);
        }
    }
}