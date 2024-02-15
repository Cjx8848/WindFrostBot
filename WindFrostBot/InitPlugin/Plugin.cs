using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class InitPlugin : Plugin
    {
        public override string PluginName()
        {
            return "InitPlugin";
        }

        public override string Version()
        {
            return "1.0.1";
        }

        public override string Author()
        {
            return "Cjx";
        }
        public override string Description()
        {
            return "初始插件";
        }
        public override void OnLoad()
        {
            Admin.Init(this);//添加管理指令
            Group.Init(this);//添加群聊指令
        }
    }
}
