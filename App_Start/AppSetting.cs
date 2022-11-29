using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace xmkgl.App_Start
{
    public class AppSetting
    {
        private static TimeSpan? s_defaultAccountLockoutTimeSpan;
        private static int? s_maxFailedAccessAttemptsBeforeLockout;
        private static bool? s_userLockoutEnabledByDefault;

        public static TimeSpan DefaultAccountLockoutTimeSpan
        {
            get
            {
                if (!s_defaultAccountLockoutTimeSpan.HasValue)
                {
                    double result;
                    if (double.TryParse(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"], out result))
                    {
                        s_defaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(result);
                    }
                    else
                    {
                        s_defaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(20);
                    }
                }
                return s_defaultAccountLockoutTimeSpan.Value;
            }
            set { s_defaultAccountLockoutTimeSpan = value; }
        }

        public static int MaxFailedAccessAttemptsBeforeLockout
        {
            get
            {
                if (!s_maxFailedAccessAttemptsBeforeLockout.HasValue)
                {
                    int result;

                    if (int.TryParse(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"],
                                     out result))
                    {
                        s_maxFailedAccessAttemptsBeforeLockout = result;
                    }
                    else
                    {
                        s_maxFailedAccessAttemptsBeforeLockout = 5;
                    }
                    s_maxFailedAccessAttemptsBeforeLockout = result;
                }
                return s_maxFailedAccessAttemptsBeforeLockout.Value;
            }
            set { s_maxFailedAccessAttemptsBeforeLockout = value; }
        }

        public static bool UserLockoutEnabledByDefault
        {
            get
            {
                if (!s_userLockoutEnabledByDefault.HasValue)
                {
                    bool result;
                    if (bool.TryParse(ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"], out result))
                    {
                        s_userLockoutEnabledByDefault = result;
                    }
                    else
                    {
                        s_userLockoutEnabledByDefault = true;
                    }

                    s_userLockoutEnabledByDefault = result;
                }
                return s_userLockoutEnabledByDefault.Value;
            }
            set { s_userLockoutEnabledByDefault = value; }
        }

    }
}