using Google.Protobuf.WellKnownTypes;
using Sora.Entities.Info;
using System.Drawing;
using System.IO;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class Admin
    {
        public static void Init(Plugin plugin)
        {
            CommandManager.InitGroupCommand(plugin, AddAdmin, "添加管理员指令", "添加管理");
            CommandManager.InitGroupCommand(plugin, RemoveAdmin, "移除管理员指令", "删除管理","移除管理");
            CommandManager.InitGroupCommand(plugin, GetAdminList, "获取管理列表", "管理列表");
        }
        public static void AddAdmin(CommandArgs args)
        {
            if (args.IsOwnner())
            {
                if (args.Parameters.Count < 1)
                {
                    args.Api.SendTextMessage("参数不足:添加管理 <qq>");
                    return;
                }
                if (!long.TryParse(args.Parameters[0], out var qq))
                {
                    args.Api.SendTextMessage("参数错误.");
                    return;
                }
                if(Utils.IsAdmin(qq))
                {
                    args.Api.SendTextMessage("此用户已经是管理了.");
                    return;
                }
                MainSDK.BotConfig.Admins.Add(qq);
                args.Api.SendTextMessage("操作成功.");
                ConfigWriter.Config.WriteConfig();
            }
            else
            {
                args.Api.SendTextMessage("无权操作!");
            }
        }
        public static void RemoveAdmin(CommandArgs args)
        {
            if (args.IsOwnner())
            {
                if (args.Parameters.Count < 1)
                {
                    args.Api.SendTextMessage("参数不足:删除管理 <qq>");
                    return;
                }
                if (!long.TryParse(args.Parameters[0], out var qq))
                {
                    args.Api.SendTextMessage("参数错误.");
                    return;
                }
                if (!Utils.IsAdmin(qq))
                {
                    args.Api.SendTextMessage("此用户非管理.");
                    return;
                }
                MainSDK.BotConfig.Admins.Remove(qq);
                args.Api.SendTextMessage("操作成功.");
                ConfigWriter.Config.WriteConfig();
            }
            else
            {
                args.Api.SendTextMessage("无权操作!");
            }
        }
        public static void GetAdminTextList(CommandArgs args)
        {
            List<string> listtext = new List<string>();
            listtext.Add($"[{MainSDK.BotConfig.BotName}]管理列表:");
            foreach (var qq in MainSDK.BotConfig.Owners)
            {
                listtext.Add($"QQ:[{qq}][Owner]");
            }
            foreach (var qq in MainSDK.BotConfig.Admins)
            {
                listtext.Add($"QQ:[{qq}][Admin]");
            }
            string text = string.Join("\n", listtext);
            args.Api.SendTextMessage(text);
        }
        public static void GetAdminList(CommandArgs args)
        {
            int page = 0;
            if(args.Parameters.Count > 0)
            {
                if (!int.TryParse(args.Parameters[0], out page))
                {
                    GetAdminTextList(args);
                    return;
                }
            }
            Image img = DrawAdminList(args.Api.GetGroupMemembers(), page);
            if(img != null)
            {
                args.Api.SendImage(img);
            }
            else
            {
                args.Api.SendTextMessage("图像输出出错!Cjx Baka!");
                Message.LogErro("图像输出出错!:[/Pictures/admin.png]");
            }
        }
        #region CA1416
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        #endregion
        public static Image DrawAdminList(List<GroupMemberInfo> groupMembers, int pageNumber)
        {
            const int itemsPerPage = 20;
            const int imageWidth = 1536;
            const int startX = 0;
            const int startY = 250;
            const int itemHeight = 150;
            const int itemWidth = 350;
            Image image = Image.FromFile(Directory.GetCurrentDirectory() + "/Pictures/admin.png");
            if(image == null)
            {
                return null;
            }
            using (Graphics graphics = Graphics.FromImage(image))
            {
                DrawText(graphics, "CSFT亚共体机器人系统", new Font("Heavy", 100), Color.White, new PointF(320, 20));
                DrawText(graphics, "管理员列表", new Font("Heavy", 60), Color.White, new PointF(730, 150));
                var allAdmins = GetCombinedAdminList();
                int totalPages = (int)Math.Ceiling(allAdmins.Count / (double)itemsPerPage);
                pageNumber = Math.Clamp(pageNumber, 1, totalPages);
                int startItemIndex = (pageNumber - 1) * itemsPerPage;
                int endItemIndex = Math.Min(startItemIndex + itemsPerPage, allAdmins.Count);
                int x = startX, y = startY;
                for (int i = startItemIndex; i < endItemIndex; i++)
                {
                    if (x > imageWidth)
                    {
                        x = startX;
                        y += itemHeight;
                    }
                    DrawAdminItem(graphics, allAdmins[i], groupMembers, new Rectangle(x, y, 100, 100));
                    x += itemWidth;
                }
                DrawText(graphics, $"当前页码{pageNumber}/{totalPages}", new Font("Heavy", 20), Color.White, new PointF(10, 10));
            }
            return image;
        }
        #region CA1416
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        #endregion
        private static void DrawAdminItem(Graphics graphics, long adminId, List<GroupMemberInfo> groupMembers, Rectangle rect)
        {
            string nickname = "此管理不在此群";
            foreach (GroupMemberInfo member in groupMembers)
            {
                if (member.UserId == adminId)
                {
                    nickname = $"管理昵称: {member.Nick}";
                    break;
                }
            }
            Image adminImage = GetQQImgAsync(adminId).Result;
            graphics.DrawImageUnscaledAndClipped(adminImage, rect);
            using (Font font = new Font("Heavy", 20))
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                graphics.DrawString($"QQ号: {adminId}", font, brush, new PointF(rect.X + rect.Width + 10, rect.Y + 20));
                graphics.DrawString(nickname, font, brush, new PointF(rect.X + rect.Width + 10, rect.Y + rect.Height / 2 + 10));
            }
        }
        private static List<long> GetCombinedAdminList()
        {
            // Combine Admins and SAdmins from config and remove duplicates
            var admins = new HashSet<long>(MainSDK.BotConfig.Owners);
            admins.UnionWith(MainSDK.BotConfig.Admins);
            return admins.ToList();
        }
        #region CA1416
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        #endregion
        private static void DrawText(Graphics graphics, string text, Font font, Color color, PointF point)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.DrawString(text, font, brush, point);
            }
        }
        #region CA1416
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        #endregion
        public static async Task<Image> GetQQImgAsync(long qq)
        {
            HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(180) };
            try
            {
                string url = $"http://q2.qlogo.cn/headimg_dl?dst_uin={qq}&spec=100";
                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode(); // 确保响应状态码表示成功
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Message.LogErro(ex.ToString());
                return null;
            }
        }
    }
}
