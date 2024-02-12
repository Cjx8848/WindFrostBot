using System;
using Sora;
using System.Drawing;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;

namespace WindFrostBot.SDK
{
    public class QCommand
    {
        public long Account = 0;
        public long Group = 0;
        public int Type = 0; //0为Sora Group;1为Sora Private;
        public QCommand(GroupMessageEventArgs eventArgs) //type0,Sora Group;
        {
            Account = eventArgs.Sender.Id;
            Group = eventArgs.SourceGroup.Id;
            Type = 0;
        }
        public void SendTextMessage(string message)
        {
            switch(Type)
            {
                case 0:
                    MessageBody body = message;
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group , body);
                    break;
                default:
                    break;
            }
        }
        public void SendImage(Image img)
        {
            switch(Type)
            {
                case 0:
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    Stream stream = new MemoryStream(ms.ToArray());
                    MessageBody body = new MessageBody(new List<SoraSegment>()
                    {
                             SoraSegment.Image(stream), // 生成图片消息段
                         });
                    MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group, body);
                    break;
            }
        }
    }
}
