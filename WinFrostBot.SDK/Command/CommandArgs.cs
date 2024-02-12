using System;

namespace WindFrostBot.SDK
{
    public delegate void ComDelegate(CommandArgs args);
    public class CommandArgs : EventArgs
    {
        public string Message { get; private set; }
        public List<string> Parameters { get; private set; }
        public long Account = 0;
        public long Group = 0;
        public QCommand Api { get; private set; }
        public CommandArgs(string msg,List<string> args, QCommand cmd)
        {
            Parameters = args;
            Message = msg;
            Account = cmd.Account;
            Group = cmd.Group;
            Api = cmd;
        }
    }
    public class CommandManager
    {
        public static List<Command> Coms = new List<Command>();
        public static void InitCommandToSora()
        {
            MainSDK.service.Event.OnGroupMessage += (sender, eventArgs) =>
            {
                if (!Utils.CanSend(eventArgs.SourceGroup.Id))
                {
                    return ValueTask.CompletedTask;
                }
                string text = eventArgs.Message.ToString();//接收的所有消息
                string msg = text.Split(" ")[0];//指令消息
                List<string> arg = text.Split(" ").ToList();
                arg.Remove(text.Split(" ")[0]);//除去指令消息的其他段消息
                var cmd =  Coms.Find(c => c.Names.Contains(msg));
                if(cmd != null)
                {
                    if (cmd.Type == 0)
                    {
                        try
                        {
                            cmd.Run(msg, arg, new QCommand(eventArgs));
                        }
                        catch (Exception ex)
                        {
                            Message.LogErro(ex.Message);
                        }
                    }
                }
                return ValueTask.CompletedTask;
            };
        }
        public static void InitSoraCommand(Plugin plugin,ComDelegate cmd,string cmdinfo,params string[] cmdnames)
        {
            Command com = new Command(cmd, cmdinfo, 0, cmdnames);
            plugin.Commands.Add(com);
            Coms.Add(com);
        }
    }
    public class Command
    {
        private ComDelegate cd;
        public int Type;
        public ComDelegate Cd
        {
            get
            {
                return cd;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                cd = value;
            }
        }
        public Command(ComDelegate cmd, int type,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = "此指令没有帮助.";
            Type = type;
        }
        public Command(ComDelegate cmd, string help, int type ,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = help;
            Type = type;
        }
        public List<string> Names = new List<string>();
        public string HelpText = "";
        public bool Run(string msg,List<string> parms,QCommand cmd)
        {
            try
            {
                cd(new CommandArgs(msg, parms, cmd));
            }
            catch(Exception ex)
            {
                Message.Erro("指令出错!:" + ex.ToString());
            }
            return true;
        }
    }
}
