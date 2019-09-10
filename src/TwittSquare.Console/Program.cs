using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwittSquare.Core.Twitter.Model;
using TwittSquare.Core.Utils.Extensions;
using static System.Console;

namespace TwittSquare.Console {
    public class Program {
        public static void Main(string[] args) {
            OutputEncoding = Encoding.UTF8;
            Out.WriteLine("input screen_name");
            string screenName = In.ReadLine();
            using(var context = new TwitterContext()) {
                long? _id = context.Users.FirstOrDefault(x=>x.ScreenName==screenName)?.Id;
                if(_id==null){
                    Out.WriteLine("not found screen_name");
                }
                long id = _id.Value;
                var users = new Dictionary<long,int>();
                var replys = new Dictionary<long,int>();
                // 自分がフォロー +100
                // ふぁぼ +3 あんふぁぼ -3
                // RT or Quote +1
                // リプライ + 10 リプライイベント数差 -5
                foreach(var following in context.Followings.Where(x=>x.UserId==id)){
                    users.PutOrAdd(following.FollowingId,100);
                }
                foreach(var reactive in context.ReactiveEvents.Where(x=>x.TargetUserId==id)){
                    if(reactive.EventType==ReacteveEventType.Favorite){
                        users.PutOrAdd(reactive.UserId,3);
                    }else if(reactive.EventType==ReacteveEventType.Reply){
                        users.PutOrAdd(reactive.UserId,10);                       
                        replys.PutOrAdd(reactive.UserId,1);
                    }else{
                        users.PutOrAdd(reactive.UserId,1);
                    }
                }
                foreach(var reactive in context.ReactiveEvents.Where(x=>x.UserId==id)){
                    if(reactive.EventType==ReacteveEventType.Favorite){
                        users.PutOrAdd(reactive.TargetUserId,3);
                    }else if(reactive.EventType==ReacteveEventType.Reply){
                        users.PutOrAdd(reactive.TargetUserId,10);
                        replys.PutOrAdd(reactive.TargetUserId,-1);
                    }else{
                        users.PutOrAdd(reactive.TargetUserId,1);
                    }
                }
                foreach(var delete in context.DeleteEvents.Where(x=>x.TargetUserId==id)){
                    if(delete.EventType==DeleteEventType.Favorite){
                        users.PutOrAdd(delete.UserId,-3);
                    }
                }
                foreach(var delete in context.DeleteEvents.Where(x=>x.UserId==id)){
                    if(delete.EventType==DeleteEventType.Favorite&&delete.TargetUserId!=null){
                        users.PutOrAdd(delete.TargetUserId.Value,-3);
                    }
                }
                foreach(var reply in replys){
                    users.PutOrAdd(reply.Key,Math.Abs(reply.Value)*(-5));
                }
                foreach(var user in users.OrderByDescending(x=>x.Value)){
                    var relationUser = context.Users.FirstOrDefault(x=>x.Id==user.Key);
                    if(relationUser==null){
                        continue;
                    }
                    Out.WriteLine($"ScreenName: {relationUser.ScreenName} Name: {relationUser.Name} Point: {user.Value}");
                }
            }
        }
    }

    internal static class Extensions{
        public static void PutOrAdd(this Dictionary<long,int> dictionary,long userId,int i){
            if(dictionary.ContainsKey(userId)){
                dictionary[userId] = i + dictionary[userId];
            }else{
                dictionary[userId] = i;
            }
        }
    }
}
