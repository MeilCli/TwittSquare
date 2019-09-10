using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwittSquare.ASP.Utils.Extensions;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.ASP.Controllers {
    public class AccountController : Controller {

        public IActionResult Index() {
            Token token = this.GetTokenAndAutoRedirect();
            ViewBag.User = token.User;
            return View();
        }

        public IActionResult Login() {
            Token token = this.GetTokenAndAutoRedirect();
            ViewBag.CurrentLogin = this.GetCurrentLogin();
            using(var context = new TwitterContext()) {
                ViewBag.Logins = context.Logins.Where(x => x.UserId == token.UserId).Where(x => x.Expires > DateTime.Now).ToList();
            }
            return View();
        }

        public IActionResult Revoke(string revokeToken) {
            Token token = this.GetTokenAndAutoRedirect();
            using(var context = new TwitterContext())
            using(var md5 = MD5.Create()) {
                Func<string,string> createHash = (x) => {
                    var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(x));
                    var hash = BitConverter.ToString(hashedBytes).Replace("-","").ToLower();
                    return hash;
                };
                Login revokeLogin = context.Logins.Where(x => x.UserId == token.UserId).FirstOrDefault(x => createHash(x.Token) == revokeToken);
                if(revokeLogin == null) {
                    return RedirectToAction("Login");
                }
                context.Logins.Remove(revokeLogin);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }
    }
}
