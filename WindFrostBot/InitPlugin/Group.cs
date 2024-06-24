using System;
using System.Collections.Generic;
using System.Numerics;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class Group
    {
        public static void Init(Plugin plugin)
        {
            CommandManager.InitGroupCommand(plugin, AddGroup, "添加管理员指令", "添加群聊");
            CommandManager.InitGroupCommand(plugin, RemoveGroup, "移除管理员指令", "删除群聊","移除群聊");
            CommandManager.InitGroupCommand(plugin, GetGroupList, "获取管理列表", "群聊列表");
        }
        public static void AddGroup(CommandArgs args)
        {
            if (args.IsOwnner())
            {
                if (args.Parameters.Count < 1)
                {
                    args.Api.SendTextMessage("参数不足:添加群聊 <group>");
                    return;
                }
                if (!long.TryParse(args.Parameters[0], out var group))
                {
                    args.Api.SendTextMessage("参数错误.");
                    return;
                }
                if(MainSDK.BotConfig.QGroups.Contains(group))
                {
                    args.Api.SendTextMessage("此群聊已在列表.");
                    return;
                }
                MainSDK.BotConfig.QGroups.Add(group);
                args.Api.SendTextMessage("操作成功.");
                ConfigWriter.Config.WriteConfig();
            }
            else
            {
                args.Api.SendTextMessage("无权操作!");
            }
        }
        public static void RemoveGroup(CommandArgs args)
        {
            if (args.IsOwnner())
            {
                if (args.Parameters.Count < 1)
                {
                    args.Api.SendTextMessage("参数不足:删除群聊 <group>");
                    return;
                }
                if (!long.TryParse(args.Parameters[0], out var group))
                {
                    args.Api.SendTextMessage("参数错误.");
                    return;
                }
                if (!MainSDK.BotConfig.QGroups.Contains(group))
                {
                    args.Api.SendTextMessage("此用户非管理.");
                    return;
                }
                MainSDK.BotConfig.QGroups.Remove(group);
                args.Api.SendTextMessage("操作成功.");
                ConfigWriter.Config.WriteConfig();
            }
            else
            {
                args.Api.SendTextMessage("无权操作!");
            }
        }
        public static void GetGroupList(CommandArgs args)
        {
            if(!args.IsAdmin())
            {
                args.Api.SendTextMessage("无权操作!");
                return;
            }
            List<string> listtext = new List<string>();
            listtext.Add($"[{MainSDK.BotConfig.BotName}]管理列表:");
            foreach(var group in MainSDK.BotConfig.QGroups)
            {
                listtext.Add($"群号:[{group}]");
            }
            string text = string.Join("\n", listtext);
            args.Api.SendTextMessage(text);
        }
    }
}
