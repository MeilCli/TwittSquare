using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwittSquare.Core.Twitter.Model;
using DetectionCore;
using Microsoft.AspNetCore.Http;
using CoreTweet;
using TUser = TwittSquare.Core.Twitter.Model.User;
using TwittSquare.Core.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace TwittSquare.ASP.Utils.Extensions {
    public static class OauthExtension {

        private const string savedToken = "TwittSquareToken";
        private const string contextToken = "token";
        private const string currentLogin = "currentLogin";
        private static Random random = new Random();

        public static Token CheckLogin(this HttpContext httpContext) {
            var savedToken = httpContext.Request.Cookies[OauthExtension.savedToken];
            if(savedToken == null) {
                return null;
            }
            using(var context = new TwitterContext()) {
                Login login = context.Logins.FirstOrDefault(x => x.Token == savedToken);
                if(login == null) {
                    return null;
                }
                if(login.Expires <= DateTime.Now) {
                    context.Logins.Remove(login);
                    return null;
                }

                string userAgent = httpContext.Request.Headers.ContainsKey("User-Agent") ? httpContext.Request.Headers["User-Agent"].ToString() : null;
                login.Browser = userAgent?.Browser() ?? Browser.Unknown;
                login.Platform = userAgent?.Platform() ?? Platform.Unknown;

                login.LastLoginAt = DateTime.Now;

                string newToken = login.UserId.MakeUniqueToken(context);
                login.Token = newToken;
                var cookieOption = new CookieOptions() {
                    Expires = login.Expires,
                    HttpOnly = true
                };
                httpContext.Response.Cookies.Append(OauthExtension.savedToken,newToken,cookieOption);

                context.SaveChanges();
                var token = context.Tokens.Include(x => x.User).FirstOrDefault(x => x.UserId == login.UserId);
                httpContext.Items[contextToken] = token;
                httpContext.Items[OauthExtension.savedToken] = newToken;
                httpContext.Items[currentLogin] = login;
                return token;
            }
        }

        public static Token GetToken(this Controller controller) {
            var token = controller.HttpContext.Items[contextToken] as Token;
            if(token == null) {
                controller.ViewBag.IsLogin = false;
                return null;
            }
            controller.ViewBag.IsLogin = true;
            controller.ViewBag.LoginUser = $"@{token.User.ScreenName}";
            controller.ViewBag.LoginUserIcon = token.User.ProfileImageUrl;
            controller.ViewBag.LoginUserId = token.UserId;
            return token;
        }

        public static Token GetTokenAndAutoRedirect(this Controller controller) {
            var token = controller.GetToken();
            if(token == null) {
                controller.RedirectToAction("Index","Home");
            }
            return token;
        }

        public static Login GetCurrentLogin(this Controller controller) {
            return controller.HttpContext.Items[currentLogin] as Login;
        }

        public static async Task SaveLogin(this Controller controller,Tokens tokens) {
            using(var context = new TwitterContext()) {
                var user = new TUser((await tokens.Account.VerifyCredentialsAsync()));
                context.Upsert(user);
                context.SaveChanges();

                var token = new Token(tokens,user);
                context.Upsert(token);
                context.SaveChanges();

                var login = new Login();

                string userAgent = controller.Request.Headers.ContainsKey("User-Agent") ? controller.Request.Headers["User-Agent"].ToString() : null;
                login.Browser = userAgent?.Browser() ?? Browser.Unknown;
                login.Platform = userAgent?.Platform() ?? Platform.Unknown;

                login.UserId = user.Id;
                login.Expires = DateTime.Now.AddDays(1);
                login.LastLoginAt = DateTime.Now;
                login.Token = user.Id.MakeUniqueToken(context);
                
                var cookieOption = new CookieOptions() {
                    Expires = login.Expires,
                    HttpOnly = true
                };
                controller.Response.Cookies.Append(savedToken,login.Token,cookieOption);

                context.Logins.AddOrUpdate(login);
                context.SaveChanges();
            }
        }

        public static void LogoutToken(this Controller controller) {
            controller.Response.Cookies.Delete(savedToken);

            var token = controller.HttpContext.Items[savedToken] as string;
            if(token == null) {
                return;
            }
            
            using(var context = new TwitterContext()) {
                Login login = context.Logins.FirstOrDefault(x => x.Token == token);
                if(login == null) {
                    return;
                }
                context.Logins.Remove(login);
                context.SaveChanges();
            }
        }

        public static string MakeToken(this long userId) {
            return $"{userId}-{random.Next()}";
        }

        public static string MakeUniqueToken(this long userId,TwitterContext context) {
            string token = userId.MakeToken();
            if(context.Logins.Count() == 0) {
                return token;
            }
            while(context.Logins.Where(x => x.Token == token).Count() > 0) {
                token = userId.MakeToken();
            }
            return token;
        }
    }
}
