using System;

namespace WindFrostBot.SDK
{
    public class Utils
    {
        public static bool CanSend(long group)
        {
            bool cansend = false;
            foreach(var g in MainSDK.BotConfig.QGroups)
            {
                if (g == group)
                {
                    cansend = true;
                    break;
                }
            }
            return cansend;
        }
        public static bool IsAdmin(long account)
        {
            bool isadmin = false;
            foreach(var acc in MainSDK.BotConfig.Admins)
            {
                if (acc == account)
                {
                    isadmin = true;
                    break;
                }
            }
            return isadmin;
        }
    }
}
