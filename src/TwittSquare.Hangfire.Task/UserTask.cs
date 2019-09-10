using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;
using Hangfire;
using TwittSquare.Core.Twitter.Model;
using TwittSquare.Core.Utils.Extensions;

namespace TwittSquare.Hangfire.Task {
    public class UserTask {

        public void TaskCheck() {
            using(var context = new TwitterContext()) {
                foreach(var token in context.Tokens) {
                    AddTask(token);
                }
            }
        }

        public void AddTask(long userId) {
            using(var context =new TwitterContext()) {
                var token = context.Tokens.FirstOrDefault(x => x.UserId == userId);
                if(token == null) {
                    return;
                }
                AddTask(token);
                RecurringJob.Trigger(TaskId.GetUserHomeTimelineId(token.UserId));
                RecurringJob.Trigger(TaskId.GetUserUserTimelineId(token.UserId));
                RecurringJob.Trigger(TaskId.GetUserFollowingId(token.UserId));
            }
        }

        private void AddTask(Token token) {
            RecurringJob.AddOrUpdate(TaskId.GetUserHomeTimelineId(token.UserId),() => HomeTimeline(token.UserId),Cron.MinuteInterval(5));
            RecurringJob.AddOrUpdate(TaskId.GetUserUserTimelineId(token.UserId),() => UserTimeline(token.UserId),Cron.MinuteInterval(10));
            RecurringJob.AddOrUpdate(TaskId.GetUserFollowingId(token.UserId),() => Following(token.UserId),Cron.MinuteInterval(10));
        }

        public void HomeTimeline(long userId) {
            Console.Out.WriteLine($"HomeTimeline {userId}");
            using(var context=new TwitterContext()) {
                var token = context.Tokens.FirstOrDefault(x => x.UserId == userId);
                if(token == null) {
                    return;
                }

                var tokens = Tokens.Create(token.ConsumerKey,token.ConsumerSecret,token.AccessToken,token.AccessTokenSecret);
                var response = tokens.Statuses.HomeTimelineAsync(count: 200,include_entities: true,tweet_mode: TweetMode.extended).Result
                    .Select(x=>new Core.Twitter.Model.Status(x))
                    .ToList();
                int errorCount = 0;
                foreach(var status in response) {
                    try {
                        context.Upsert(status);
                        context.Upsert(new ActiveTime(status.UserId,status.CreatedAt));
                    } catch(Exception e) {
                        errorCount++;
                        Console.Out.WriteLine(e.Message);
                    }
                }

                context.SaveChanges();
                if(errorCount > 0) {
                    Console.Out.WriteLine($"HomeTimeline error: {errorCount}");
                }
            }
        }

        public void UserTimeline(long userId) {
            Console.Out.WriteLine($"UserTimeline {userId}");
            using(var context = new TwitterContext()) {
                var token = context.Tokens.FirstOrDefault(x => x.UserId == userId);
                if(token == null) {
                    return;
                }

                var tokens = Tokens.Create(token.ConsumerKey,token.ConsumerSecret,token.AccessToken,token.AccessTokenSecret);
                var response = tokens.Statuses.UserTimelineAsync(user_id:userId,count: 200,include_rts:true,tweet_mode: TweetMode.extended).Result
                    .Select(x => new Core.Twitter.Model.Status(x))
                    .ToList();
                int errorCount = 0;
                foreach(var status in response) {
                    try {
                        context.Upsert(status);
                        context.Upsert(new ActiveTime(status.UserId,status.CreatedAt));
                    } catch(Exception e) {
                        errorCount++;
                        Console.Out.WriteLine(e.Message);
                    }
                }

                context.SaveChanges();
                if(errorCount > 0) {
                    Console.Out.WriteLine($"UserTimeline error: {errorCount}");
                }
            }
        }

        public void Following(long userId) {
            Console.Out.WriteLine($"Following {userId}");
            using(var context = new TwitterContext()) {
                var token = context.Tokens.FirstOrDefault(x => x.UserId == userId);
                if(token == null) {
                    return;
                }

                var tokens = Tokens.Create(token.ConsumerKey,token.ConsumerSecret,token.AccessToken,token.AccessTokenSecret);
                var followings = new List<long>();
                var cursor = tokens.Friends.IdsAsync(user_id: userId,count:5000).Result;
                followings.AddRange(cursor);
                while(cursor.Count == 5000) {
                    cursor = tokens.Friends.IdsAsync(user_id: userId,cursor:cursor.NextCursor,count: 5000).Result;
                    followings.AddRange(cursor);
                }

                DateTime takeAllAt = DateTime.Now;
                int errorCount = 0;
                foreach(var following in followings.Select(x => new Following(userId,x,takeAllAt))) {
                    try {
                        context.Upsert(following);
                    } catch(Exception e) {
                        errorCount++;
                        Console.Out.WriteLine(e.Message);
                    }
                }
                context.Upsert(new FollowingTask(userId,takeAllAt));

                context.SaveChanges();
                if(errorCount > 0) {
                    Console.Out.WriteLine($"Following error: {errorCount}");
                }
            }
        }
    }
}
