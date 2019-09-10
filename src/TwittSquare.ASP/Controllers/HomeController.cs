using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwittSquare.ASP.Utils.Extensions;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.ASP.Controllers {
    public class HomeController : Controller {

        [AllowAnonymous]
        public IActionResult Index() {
            Token token = this.GetToken();
            return View();
        }

        public IActionResult About() {
            Token token = this.GetTokenAndAutoRedirect();

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact() {
            Token token = this.GetTokenAndAutoRedirect();

            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error() {
            return View();
        }
    }
}
