using System.Drawing;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using Sora.Entities.Info;
using System.IO;

namespace WindFrostBot.SDK
{
    public class QCommand
    {
        public long Account = 0;
        public long Group = 0;
        public int Type = 0;  //0为Sora Group;1为Sora Private;
        public Group? SourceGroup {  get; set; }
        public GroupMessageEventArgs? GroupMessageEvent { get; set; }
        public PrivateMessageEventArgs? PrivateMessageEvent { get; set; }
        public QCommand(GroupMessageEventArgs eventArgs) //type0,Sora Group;
        {
            Account = eventArgs.Sender.Id;
            Group = eventArgs.SourceGroup.Id;
            Type = 0;
            SourceGroup = eventArgs.SourceGroup;
            GroupMessageEvent = eventArgs;
        }
        public QCommand(PrivateMessageEventArgs eventArgs) //type1,Sora Private
        {
            Account = eventArgs.Sender.Id;
            //Group = eventArgs.SourceGroup.Id;
            Type = 1;
            PrivateMessageEvent = eventArgs;
        }
        public void SendTextMessage(string message)
        {
            switch(Type)
            {
                case 0:
                    MessageBody body = message;
                    SourceGroup?.SendGroupMessage(body);
                    //MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group , body);
                    break;
                case 1:
                    body = message;
                    PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                    //MainSDK.service.GetApi(MainSDK.service.ServiceId).SendPrivateMessage(Account, body);
                    break;
                default:
                    break;
            }
        }
        public void ReplyMessage(string message)
        {
            int id = GroupMessageEvent.Message.MessageId;
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                        SoraSegment.Reply(id),
                        SoraSegment.Text(message)
             });
            switch (Type)
            {
                case 0:
                    SourceGroup?.SendGroupMessage(body);
                    break;
                case 1:
                    PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                    break;
                default:
                    break;
            }
        }
        public void SendImage(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            Stream stream = new MemoryStream(ms.ToArray());
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                SoraSegment.Image(stream) // 生成图片消息段
            });
            switch (Type)
            {
                case 0:
                    GroupMessageEvent?.SourceGroup.SendGroupMessage(body);
                    break;
                case 1:
                    PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                    break;
            }
        }
        public void SendImage(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                  SoraSegment.Image(stream)// 生成图片消息段
            });
            switch (Type)
            {
                case 0:
                    SourceGroup?.SendGroupMessage(body);
                    break;
                case 1:
                    PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                    break;
            }
        }
        public List<GroupMemberInfo>? GetGroupMemembers()
        {
            switch(Type) { 
                case 0:
                    if(GroupMessageEvent != null)
                    {
                        return MainSDK.service.GetApi(GroupMessageEvent.ServiceId).GetGroupMemberList(Group).Result.groupMemberList;
                    }
                    return null;
            }
            return null;
        }
    }
}
