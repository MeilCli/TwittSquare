using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwittSquare.ASP.Utils.Extensions;
using TwittSquare.Core.Twitter;
using TwittSquare.Hangfire.Task;
using static CoreTweet.OAuth;

namespace TwittSquare.ASP.Controllers {
    public class OauthController : Controller {

        private const string requestToken = "requestToken";
        private const string requestTokenSecret = "requestTokenSecret";

        [AllowAnonymous]
        public IActionResult Index() {
            return RedirectToAction(actionName:"Index",controllerName:"Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login() {
            string callbackUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}".Replace("Login","Callback");
            var session = await OAuth.AuthorizeAsync(Constant.ConsumerKey,Constant.ConsumerSecret,callbackUrl);
            TempData[requestToken] = session.RequestToken;
            TempData[requestTokenSecret] = session.RequestTokenSecret;
            return Redirect(session.AuthorizeUri.AbsoluteUri);
        }

        [AllowAnonymous]
        public IActionResult Logout() {
            this.LogoutToken();
            return RedirectToAction(actionName: "Index",controllerName: "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Callback(string oauth_token,string oauth_verifier) {
            var session = new OAuthSession() {
                ConsumerKey = Constant.ConsumerKey,
                ConsumerSecret = Constant.ConsumerSecret,
                RequestToken = TempData[requestToken].ToString(),
                RequestTokenSecret = TempData[requestTokenSecret].ToString(),
                ConnectionOptions = new ConnectionOptions()
            };
            var token = await session.GetTokensAsync(oauth_verifier);
            await this.SaveLogin(token);

            BackgroundJob.Enqueue(() => new UserTask().AddTask(token.UserId));

            return RedirectToAction(actionName: "Index",controllerName: "Home");
        }
    }
}
