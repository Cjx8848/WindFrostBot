using System;

namespace WindFrostBot.SDK
{
    public class GroupAtArgs : EventArgs
    {
        public long Account;
        public QCommand Api;
        public string Message;
        public GroupAtArgs(QCommand qcmd, string message)
        {
            Account = qcmd.Account;
            Api = qcmd;
            Message = message;
        }
    }
}
