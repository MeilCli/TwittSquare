using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.Hangfire.Task {
    public class LoginTask {

        public void DeleteExpirationLogin() {
            int deleteCount = 0;
            using(var context=new TwitterContext()) {
                try {
                    foreach(var login in context.Logins.Where(x => x.Expires < DateTime.Now).ToList()) {
                        context.Logins.Remove(login);
                        deleteCount++;
                    }
                    context.SaveChanges();
                } catch(Exception) {
                    deleteCount = 0;
                    Console.Out.WriteLine("DeleteExpirationLogin: happen exception");
                }
            }
            Console.Out.WriteLine($"DeleteExpirationLogin: {deleteCount}");
        }
    }
}
