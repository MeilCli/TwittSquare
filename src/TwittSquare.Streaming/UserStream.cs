using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreTweet;
using CoreTweet.Streaming;
using TwittSquare.Core.Twitter.Model;
using TwittSquare.Core.Utils.Extensions;

namespace TwittSquare.Streaming {
    public class UserStream {

        public bool IsConnect { get; private set; }
        private long userId;
        private IDisposable disposable;
        private Tokens tokens;

        public UserStream(long userId) {
            this.userId = userId;
        }

        public void Connect() {
            if(IsConnect) {
                return;
            }
            using(var context = new TwitterContext()) {
                var token = context.Tokens.FirstOrDefault(x => x.UserId == userId);
                if(token == null) {
                    return;
                }
                tokens = Tokens.Create(token.ConsumerKey,token.ConsumerSecret,token.AccessToken,token.AccessTokenSecret);
            }
            if(tokens == null) {
                return;
            }
            var parameters = new Dictionary<string,object>();
            parameters["include_followings_activity"] = true;
            parameters["with"] = "followings";
            parameters["replies"] = "all";
            var observable = tokens.Streaming.UserAsObservable(parameters);
            var connectable = observable.Publish();

            connectable
                .OfType<FriendsMessage>()
                .Select(x => x.Friends)
                .Subscribe(onFriend);
            connectable
                .OfType<StatusMessage>()
                .Subscribe(onStatus);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.QuotedTweet)
                .Subscribe(onQuote);
            connectable
                .OfType<DeleteMessage>()
                .Where(x => x.Type == MessageType.DeleteStatus)
                .Subscribe(onDeleteStatus);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.Favorite)
                .Subscribe(onFavorite);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.Unfavorite)
                .Subscribe(onUnFavorite);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.Follow)
                .Subscribe(onFollow);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.Unfollow)
                .Subscribe(onUnFollow);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListCreated)
                .Subscribe(onListCreate);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListDestroyed)
                .Subscribe(onListDelete);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListUpdated)
                .Subscribe(onListUpdate);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListMemberAdded)
                .Subscribe(onListMemberAdd);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListMemberRemoved)
                .Subscribe(onListMemberRemove);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListUserSubscribed)
                .Subscribe(onListUserSubscribe);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.ListUserUnsubscribed)
                .Subscribe(onListUserUnSubscribe);
            connectable
                .OfType<EventMessage>()
                .Where(x => x.Event == EventCode.UserUpdate)
                .Subscribe(onUserUpdate);

            connectable.Subscribe(
                x => {
                    //Console.Out.WriteLine(x);
                },
                e => {
                    Console.Out.WriteLine(e.ToString());
                    IsConnect = false;
                },
                ()=> {
                    IsConnect = false;
                });
            disposable = connectable.Connect();
            Console.Out.WriteLine($"user stream connect: {userId}");
            IsConnect = true;
        }

        public void DisConnect() {
            if(disposable != null) {
                disposable.Dispose();
                disposable = null;
                Console.Out.WriteLine($"user stream disconnect: {userId}");
            }
            IsConnect = false;
        }

        private void onFriend(long[] friends) {
            try {
                using(var context = new TwitterContext()) {
                    DateTime takeAllAt = DateTime.Now;
                    foreach(var following in friends.Select(x => new Following(userId,x,takeAllAt))) {
                        context.Upsert(following);
                    }
                    context.Upsert(new FollowingTask(userId,takeAllAt));
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onFriend : {e.Message}");
            }
        }

        private void onStatus(StatusMessage statusMessage) {
            try {
                using(var context = new TwitterContext()) {
                    var status = statusMessage.Status;
                    context.Upsert(new Core.Twitter.Model.Status(status));
                    if(status.RetweetedStatus != null&&status.User?.Id!=null&&status.RetweetedStatus.User?.Id!=null) {
                        //RT
                        context.Upsert(new ReactiveEvent(
                            status.User.Id.Value,
                            status.RetweetedStatus.Id,
                            status.CreatedAt.LocalDateTime,
                            status.RetweetedStatus.User.Id.Value,
                            status.Id,
                            ReacteveEventType.Retweet
                            ));
                        context.Upsert(new SpanTime(status.User.Id.Value,status.CreatedAt.LocalDateTime,status.CreatedAt - status.RetweetedStatus.CreatedAt));
                    }else if(status.QuotedStatus!=null && status.User?.Id != null && status.QuotedStatus.User?.Id != null) {
                        //Quote
                        context.Upsert(new ReactiveEvent(
                            status.User.Id.Value,
                            status.QuotedStatus.Id,
                            status.CreatedAt.LocalDateTime,
                            status.QuotedStatus.User.Id.Value,
                            status.Id,
                            ReacteveEventType.Quote
                            ));
                        context.Upsert(new SpanTime(status.User.Id.Value,status.CreatedAt.LocalDateTime,status.CreatedAt - status.QuotedStatus.CreatedAt));
                    }else if(status.InReplyToStatusId != null && status.InReplyToUserId != null && status.User?.Id != null) {
                        //Reply
                        context.Upsert(new ReactiveEvent(
                            status.User.Id.Value,
                            status.InReplyToStatusId.Value,
                            status.CreatedAt.LocalDateTime,
                            status.InReplyToUserId.Value,
                            status.Id,
                            ReacteveEventType.Reply
                            ));
                        Core.Twitter.Model.Status targetStatus = null;
                        try {
                            targetStatus = 
                                context.Statuses.FirstOrDefault(x => x.Id == status.InReplyToStatusId.Value) 
                                ?? 
                                new Core.Twitter.Model.Status(tokens.Statuses.ShowAsync(id: status.InReplyToStatusId.Value).Result);
                            context.Upsert(targetStatus);
                        } catch(Exception) { }
                        if(targetStatus != null) {
                            context.Upsert(new SpanTime(status.User.Id.Value,status.CreatedAt.LocalDateTime,status.CreatedAt - targetStatus.CreatedAt));
                        }
                    }
                    if(status.User?.Id != null) {
                        context.Upsert(new ActiveTime(status.User.Id.Value,status.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onStatus : {e.Message}");
            }
        }

        private void saveEventMessage(TwitterContext context,EventMessage eventMessage) {
            try {
                if(eventMessage.Source?.ProfileImageUrl!=null) {
                    context.Upsert(new Core.Twitter.Model.User(eventMessage.Source));
                }else if(eventMessage.Source?.Id != null) {
                    context.Upsert(new Core.Twitter.Model.User(tokens.Users.ShowAsync(user_id: eventMessage.Source.Id.Value,include_entities: true).Result));
                }
                if(eventMessage.Target?.ProfileImageUrl != null) {
                    context.Upsert(new Core.Twitter.Model.User(eventMessage.Target));
                }else if(eventMessage.Target?.Id != null) {
                    context.Upsert(new Core.Twitter.Model.User(tokens.Users.ShowAsync(user_id: eventMessage.Target.Id.Value,include_entities: true).Result));
                }
                if(eventMessage.TargetStatus != null) {
                    if(eventMessage.TargetStatus.User != null && eventMessage.TargetStatus.User?.ProfileImageUrl != null) {
                        context.Upsert(new Core.Twitter.Model.Status(eventMessage.TargetStatus));
                    } else if(eventMessage.TargetStatus.User != null && eventMessage.TargetStatus.User?.Id != null) {
                        context.Upsert(new Core.Twitter.Model.Status(tokens.Statuses.ShowAsync(
                            id: eventMessage.TargetStatus.Id,include_my_retweet: true,include_entities: true,tweet_mode: TweetMode.extended).Result));
                    } else {
                        context.Upsert(new Core.Twitter.Model.Status(eventMessage.TargetStatus));
                    }
                }
            }catch(Exception e) {
                Console.Out.WriteLine($"saveEventMessage : {e.ToString()}");
            }
        }

        private void onQuote(EventMessage eventMessage) {
            if(eventMessage.TargetStatus == null) {
                return;
            }
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    var status = eventMessage.TargetStatus;
                    if(status.QuotedStatus != null && status.User?.Id != null && status.QuotedStatus.User?.Id != null) {
                        context.Upsert(new ReactiveEvent(
                            status.User.Id.Value,
                            status.QuotedStatus.Id,
                            status.CreatedAt.LocalDateTime,
                            status.QuotedStatus.User.Id.Value,
                            status.Id,
                            ReacteveEventType.Quote
                            ));
                        context.Upsert(new SpanTime(status.User.Id.Value,status.CreatedAt.LocalDateTime,status.CreatedAt - status.QuotedStatus.CreatedAt));
                    }
                    if(status.User?.Id != null) {
                        context.Upsert(new ActiveTime(status.User.Id.Value,status.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onQuote : {e.Message}");
            }
        }

        private void onDeleteStatus(DeleteMessage deleteMessage) {
            try {
                using(var context = new TwitterContext()) {
                    context.Upsert(new DeleteEvent(deleteMessage.UserId,deleteMessage.Id,deleteMessage.Timestamp.LocalDateTime,null,DeleteEventType.Status));
                    context.Upsert(new ActiveTime(deleteMessage.UserId,deleteMessage.Timestamp.LocalDateTime));
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onDeleteStatus : {e.Message}");
            }
        }

        private void onFavorite(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetStatus!=null&&eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new ReactiveEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.TargetStatus.Id,
                            eventMessage.TargetStatus.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            null,
                            ReacteveEventType.Favorite
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onFavorite : {e.Message}");
            }
        }

        private void onUnFavorite(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetStatus != null && eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new DeleteEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.TargetStatus.Id,
                            eventMessage.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            DeleteEventType.Favorite
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onUnFavorite : {e.Message}");
            }
        }

        private void onFollow(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if((eventMessage.Source?.Id ?? -1) == userId && eventMessage.Target?.Id != null) {
                        context.Upsert(new Following(userId,eventMessage.Target.Id.Value,DateTime.Now));
                    }
                    if(eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new UserEvent(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime,eventMessage.Target.Id.Value,null,UserEventType.Follow));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onFollow : {e.Message}");
            }
        }

        private void onUnFollow(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if((eventMessage.Source?.Id ?? -1) == userId && eventMessage.Target?.Id != null) {
                        context.Followings.Remove(context.Followings.FirstOrDefault(x => x.UserId == userId && x.FollowingId == eventMessage.Target.Id));
                    }
                    if(eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new UserEvent(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime,eventMessage.Target.Id.Value,null,UserEventType.UnFollow));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onUnFollow : {e.Message}");
            }
        }

        private void onListCreate(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList!=null&&eventMessage.Source?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            null,
                            eventMessage.TargetList.Id,
                            UserEventType.ListCreate
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListCreate : {e.Message}");
            }
        }

        private void onListDelete(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            null,
                            eventMessage.TargetList.Id,
                            UserEventType.ListDelete
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListDelete : {e.Message}");
            }
        }

        private void onListUpdate(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            null,
                            eventMessage.TargetList.Id,
                            UserEventType.ListUpdate
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListUpdate : {e.Message}");
            }
        }

        private void onListMemberAdd(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null&&eventMessage.Target?.Id!=null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            eventMessage.TargetList.Id,
                            UserEventType.ListMemberAdd
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListMemberAdd : {e.Message}");
            }
        }

        private void onListMemberRemove(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            eventMessage.TargetList.Id,
                            UserEventType.ListMemberRemove
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListMemberRemove : {e.Message}");
            }
        }

        private void onListUserSubscribe(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            eventMessage.TargetList.Id,
                            UserEventType.ListUserSubscribe
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListUserSubscribe : {e.Message}");
            }
        }

        private void onListUserUnSubscribe(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    saveEventMessage(context,eventMessage);

                    if(eventMessage.TargetList != null && eventMessage.Source?.Id != null && eventMessage.Target?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            eventMessage.CreatedAt.LocalDateTime,
                            eventMessage.Target.Id.Value,
                            eventMessage.TargetList.Id,
                            UserEventType.ListUserUnSubscribe
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,eventMessage.CreatedAt.LocalDateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onListUserUnSubscribe : {e.Message}");
            }
        }

        private void onUserUpdate(EventMessage eventMessage) {
            try {
                using(var context = new TwitterContext()) {
                    DateTime dateTime = eventMessage.CreatedAt.LocalDateTime;
                    if(DateTime.Now - dateTime > TimeSpan.FromHours(1)) {
                        dateTime = DateTime.Now;
                    }

                    saveEventMessage(context,eventMessage);

                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new UserEvent(
                            eventMessage.Source.Id.Value,
                            dateTime,
                            null,
                            null,
                            UserEventType.UserUpdate
                            ));
                    }
                    if(eventMessage.Source?.Id != null) {
                        context.Upsert(new ActiveTime(eventMessage.Source.Id.Value,dateTime));
                    }
                    context.SaveChanges();
                }
            } catch(Exception e) {
                Console.Out.WriteLine($"onUserUpdate : {e.Message}");
            }
        }

    }
}
