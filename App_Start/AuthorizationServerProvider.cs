//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.OAuth;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace xmkgl.App_Start
//{
//    public class AuthorizationServerProvider
//    {
//        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
//        {
//            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

//            var user = await userManager.FindByNameAsync(context.UserName);
//            if (user == null)
//            {
//                var message = "Invalid credentials. Please try again.";
//                context.SetError("invalid_grant", message);
//                return;
//            }

//            var validCredentials = await userManager.FindAsync(context.UserName, context.Password);
//            var enableLockout = await userManager.GetLockoutEnabledAsync(user.Id);

//            if (await userManager.IsLockedOutAsync(user.Id))
//            {
//                var message = string.Format(
//                    "Your account has been locked out for {0} minutes due to multiple failed login attempts.",
//                    AppSetting.DefaultAccountLockoutTimeSpan.TotalMinutes);
//                ;
//                context.SetError("invalid_grant", message);
//                return;
//            }

//            if (enableLockout & validCredentials == null)
//            {
//                string message;
//                await userManager.AccessFailedAsync(user.Id);

//                if (await userManager.IsLockedOutAsync(user.Id))
//                {
//                    message =
//                        string.Format(
//                            "Your account has been locked out for {0} minutes due to multiple failed login attempts.",
//                            AppSetting.DefaultAccountLockoutTimeSpan.TotalMinutes);
//                }
//                else
//                {
//                    var accessFailedCount = await userManager.GetAccessFailedCountAsync(user.Id);
//                    var attemptsLeft = AppSetting.MaxFailedAccessAttemptsBeforeLockout -
//                                       accessFailedCount;
//                    message =
//                        string.Format(
//                            "Invalid credentials. You have {0} more attempt(s) before your account gets locked out.",
//                            attemptsLeft);
//                }

//                context.SetError("invalid_grant", message);
//                return;
//            }
//            if (validCredentials == null)
//            {

//                var message = "Invalid credentials. Please try again.";
//                context.SetError("invalid_grant", message);
//                return;
//            }
//            await userManager.ResetAccessFailedCountAsync(user.Id);
//            var oAuthIdentity = await userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
//            var properties = CreateProperties(user.UserName);


//            var oAuthTicket = new AuthenticationTicket(oAuthIdentity, properties);
//            context.Validated(oAuthTicket);
//        }
//    }
//}