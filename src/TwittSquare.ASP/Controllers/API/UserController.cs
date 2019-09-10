using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwittSquare.ASP.Utils;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.ASP.Controllers.API {

    [Route("api/[controller]")]
    [Api]
    public class UserController : Controller {

        // GET: api/user/{id}/active_time
        [HttpGet("{id}/active_time")]
        public object GetActiveTime(long id) {
            using(var context = new TwitterContext()) {
                var list = context.ActiveTimes.Where(x => x.UserId == id).Where(x => x.ActiveAt >= DateTime.Now.AddDays(-7)).ToList();

                var times = list.Select(x => x.ActiveAt);

                var _hourGroup = times.GroupBy(x => x.Hour).OrderBy(x=>x.Key).Select(x => new { time = x.Key,count = x.Count() });
                var hourGroup = Enumerable.Range(0,24)
                    .Select(x => {
                        int? _count = _hourGroup.FirstOrDefault(y => y.time == x)?.count;
                        return new { time = x,count = _count ?? 0 };
                     });

                var _dayGroup = times
                    .GroupBy(x => $"{x.Year}/{x.Month}/{x.Day}").OrderBy(x => x.Key)
                    .Select(x => new { time = x.Key,count = x.Count() });
                var dayGroup = Enumerable.Range(0,7)
                    .Select(x => DateTime.Now.AddDays(-x))
                    .Reverse()
                    .Select(x=> $"{x.Year}/{x.Month}/{x.Day}")
                    .Select(x => {
                        int? _count = _dayGroup.FirstOrDefault(y => y.time == x)?.count;
                        return new { time = x,count = _count ?? 0 };
                    });

                var hourOfDayGroup = Enumerable.Range(0,7)
                    .Select(x => DateTime.Now.AddDays(-x))
                    .Reverse()
                    .Select(x => {
                        var _hour = times
                            .Where(y => y.Year == x.Year && y.Month == x.Month && y.Day == x.Day)
                            .GroupBy(y => y.Hour)
                            .Select(y => new { time = y.Key,count = y.Count() });
                        var hour = Enumerable.Range(0,24)
                            .Select(y => {
                                int? _count = _hour.FirstOrDefault(z => z.time == y)?.count;
                                return new { time = y,count = _count ?? 0 };
                            });
                        return new { time = $"{x.Year}/{x.Month}/{x.Day}",hour = hour };
                    }).ToDictionary(x=>x.time);
                var count = list.Count;
                return new { count = count,times = times,hour_group = hourGroup,day_group=dayGroup,hour_of_day_group=hourOfDayGroup };
            }
        }

        // GET: api/user/search/screen_name
        [HttpGet("search/screen_name")]
        public object GetScreenName(string term) {
            using(var context = new TwitterContext()) {
                var users = context.Users.Where(x => x.ScreenName.ToLower().StartsWith(term.ToLower())).Take(7).ToList();
                var response = users.Select(x => x.ScreenName);
                return response;
            }
        }

    }
}
