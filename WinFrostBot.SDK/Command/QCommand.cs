using System;
using Sora;
using System.Drawing;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using Sora.Entities.Info;

namespace WindFrostBot.SDK
{
    public class QCommand
    {
        public long Account = 0;
        public long Group = 0;
        public int Type = 0; //0为Sora Group;1为Sora Private;
        public Group SourceGroup {  get; set; }
        public QCommand(GroupMessageEventArgs eventArgs) //type0,Sora Group;
        {
            Account = eventArgs.Sender.Id;
            Group = eventArgs.SourceGroup.Id;
            Type = 0;
            SourceGroup = eventArgs.SourceGroup;
        }
        public QCommand(PrivateMessageEventArgs eventArgs) //type1,Sora Private
        {
            Account = eventArgs.Sender.Id;
            //Group = eventArgs.SourceGroup.Id;
            Type = 1;
        }
        public void SendTextMessage(string message)
        {
            switch(Type)
            {
                case 0:
                    MessageBody body = message;
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group , body);
                    break;
                case 1:
                    body = message;
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendPrivateMessage(Account, body);
                    break;
                default:
                    break;
            }
        }
        public void SendMessage(string message,Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            Stream stream = new MemoryStream(ms.ToArray());
            MessageBody body = new MessageBody(new List<SoraSegment>()
                    {
                             SoraSegment.Text(message),
                             SoraSegment.Image(stream), // 生成图片消息段
                     });
            switch (Type)
            {
                case 0:
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group, body);
                    break;
                case 1:
                    body = message;
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendPrivateMessage(Account, body);
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
                             SoraSegment.Image(stream), // 生成图片消息段
                         });
            switch (Type)
            {
                case 0:
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group, body);
                    break;
                case 1:
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendPrivateMessage(Account, body);
                    break;
            }
        }
        public List<GroupMemberInfo> GetGroupMemembers()
        {
            switch(Type) { 
                case 0:
                    return MainSDK.service.GetApi(MainSDK.service.ServiceId).GetGroupMemberList(Group).Result.groupMemberList;
            }
            return null;
        }
    }
}
