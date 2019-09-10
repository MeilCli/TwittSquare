using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwittSquare.ASP.Model {

    [Table("active_times")]
    public class ActiveTime {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }

        [Column("active_at",Order =1)]
        public DateTime ActiveAt { get; set; }

        public ActiveTime() { }

        public ActiveTime(long userId,DateTime activeAt) {
            UserId = userId;
            ActiveAt = activeAt;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ActiveTime>().HasKey(x => new { x.UserId,x.ActiveAt });
        }
    }

    [Table("span_times")]
    public class SpanTime {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }

        [Column("active_at",Order =1)]
        public DateTime ActiveAt { get; set; }

        [Column("span")]
        public TimeSpan Span { get; set; }

        public SpanTime() { }

        public SpanTime(long userId,DateTime activeAt,TimeSpan span) {
            UserId = userId;
            ActiveAt = activeAt;
            Span = span;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SpanTime>().HasKey(x => new { x.UserId,x.ActiveAt });
        }
    }

    public enum DeleteEventType {
        Status,
        Favorite
    }

    [Table("delete_events")]
    public class DeleteEvent {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }      

        [Column("status_id",Order =1)]
        public long StatusId { get; set; }

        [Column("delete_at",Order =2)]
        public DateTime DeleteAt { get; set; }

        [Column("target_user_id")]
        public long? TargetUserId { get; set; }

        [Column("event_type",Order =3)]
        public string EventTypeString { get; set; }

        [NotMapped]
        internal DeleteEventType EventType {
            get {
                if(EventTypeString == nameof(DeleteEventType.Status)) {
                    return DeleteEventType.Status;
                }
                if(EventTypeString == nameof(DeleteEventType.Favorite)) {
                    return DeleteEventType.Favorite;
                }
                return DeleteEventType.Favorite;
            }
            set {
                if(value == DeleteEventType.Status) {
                    EventTypeString = nameof(DeleteEventType.Status);
                }
                if(value == DeleteEventType.Favorite) {
                    EventTypeString = nameof(DeleteEventType.Favorite);
                }
                if(EventTypeString == null) {
                    EventTypeString = nameof(DeleteEventType.Favorite);
                }
            }
        }

        public DeleteEvent() { }

        public DeleteEvent(long userId,long statusId,DateTime deleteAt,long? targetUserId,DeleteEventType eventType) {
            UserId = userId;
            StatusId = statusId;
            DeleteAt = deleteAt;
            TargetUserId = targetUserId;
            EventType = eventType;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DeleteEvent>().HasKey(x => new { x.UserId,x.StatusId,x.DeleteAt,x.EventTypeString });
            modelBuilder.Entity<DeleteEvent>().Ignore(x => x.EventType);
        }

    }

    public enum ReacteveEventType{
        Retweet,
        Quote,
        Reply,
        Favorite
    }

    [Table("reactive_events")]
    public class ReactiveEvent {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }

        [Column("status_id",Order =1)]
        public long StatusId { get; set; }

        [Column("reactive_at",Order =2)]
        public DateTime ReactiveAt { get; set; }

        [Column("target_user_id")]
        public long TargetUserId { get; set; }

        [Column("source_status_id")]
        public long? SourceStatusId { get; set; }

        [Column("event_type",Order =3)]
        public string EventTypeString { get; set; }

        [NotMapped]
        internal ReacteveEventType EventType {
            get {
                if(EventTypeString == nameof(ReacteveEventType.Retweet)) {
                    return ReacteveEventType.Retweet;
                }
                if(EventTypeString == nameof(ReacteveEventType.Quote)) {
                    return ReacteveEventType.Quote;
                }
                if(EventTypeString == nameof(ReacteveEventType.Reply)) {
                    return ReacteveEventType.Reply;
                }
                if(EventTypeString == nameof(ReacteveEventType.Favorite)) {
                    return ReacteveEventType.Favorite;
                }
                return ReacteveEventType.Favorite;
            }
            set {
                if(value == ReacteveEventType.Retweet) {
                    EventTypeString = nameof(ReacteveEventType.Retweet);
                }
                if(value == ReacteveEventType.Quote) {
                    EventTypeString = nameof(ReacteveEventType.Quote);
                }
                if(value == ReacteveEventType.Reply) {
                    EventTypeString = nameof(ReacteveEventType.Reply);
                }
                if(value == ReacteveEventType.Favorite) {
                    EventTypeString = nameof(ReacteveEventType.Favorite);
                }
                if(EventTypeString == null) {
                    EventTypeString = nameof(ReacteveEventType.Favorite);
                }
            }
        }

        public ReactiveEvent() { }

        public ReactiveEvent(long userId,long statusId,DateTime reactiveAt,long targetUserId,long? sourceStatusId,ReacteveEventType eventType) {
            UserId = userId;
            StatusId = statusId;
            ReactiveAt = reactiveAt;
            TargetUserId = targetUserId;
            SourceStatusId = sourceStatusId;
            EventType = eventType;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ReactiveEvent>().HasKey(x => new { x.UserId,x.StatusId,x.ReactiveAt,x.EventTypeString });
            modelBuilder.Entity<ReactiveEvent>().Ignore(x => x.EventType);
        }
    }

    public enum UserEventType {
        Follow,
        UnFollow,
        ListCreate,
        ListDelete,
        ListUpdate,
        ListMemberAdd,
        ListMemberRemove,
        ListUserSubscribe,
        ListUserUnSubscribe,
        UserUpdate
    }

    [Table("user_events")]
    public class UserEvent {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }

        [Column("event_at",Order =1)]
        public DateTime EventAt { get; set; }

        /// <summary>
        /// target or owner user id
        /// </summary>
        [Column("target_user_id")]
        public long? TargetUserId { get; set; }

        [Column("list_id")]
        public long? ListId { get; set; }

        [Column("event_type",Order =2)]
        public string EventTypeString { get; set; }

        [NotMapped]
        public UserEventType EventType {
            get {
                if(EventTypeString == nameof(UserEventType.Follow)) {
                    return UserEventType.Follow;
                }
                if(EventTypeString == nameof(UserEventType.UnFollow)) {
                    return UserEventType.UnFollow;
                }
                if(EventTypeString == nameof(UserEventType.ListCreate)) {
                    return UserEventType.ListCreate;
                }
                if(EventTypeString == nameof(UserEventType.ListDelete)) {
                    return UserEventType.ListDelete;
                }
                if(EventTypeString == nameof(UserEventType.ListUpdate)) {
                    return UserEventType.ListUpdate;
                }
                if(EventTypeString == nameof(UserEventType.ListMemberAdd)) {
                    return UserEventType.ListMemberAdd;
                }
                if(EventTypeString == nameof(UserEventType.ListMemberRemove)) {
                    return UserEventType.ListMemberRemove;
                }
                if(EventTypeString == nameof(UserEventType.ListUserSubscribe)) {
                    return UserEventType.ListUserSubscribe;
                }
                if(EventTypeString == nameof(UserEventType.ListUserUnSubscribe)) {
                    return UserEventType.ListUserUnSubscribe;
                }
                if(EventTypeString == nameof(UserEventType.UserUpdate)) {
                    return UserEventType.UserUpdate;
                }
                return UserEventType.UserUpdate;
            }
            set {
                if(value == UserEventType.Follow) {
                    EventTypeString = nameof(UserEventType.Follow);
                }
                if(value == UserEventType.UnFollow) {
                    EventTypeString = nameof(UserEventType.UnFollow);
                }
                if(value == UserEventType.ListCreate) {
                    EventTypeString = nameof(UserEventType.ListCreate);
                }
                if(value == UserEventType.ListDelete) {
                    EventTypeString = nameof(UserEventType.ListDelete);
                }
                if(value == UserEventType.ListUpdate) {
                    EventTypeString = nameof(UserEventType.ListUpdate);
                }
                if(value == UserEventType.ListMemberAdd) {
                    EventTypeString = nameof(UserEventType.ListMemberAdd);
                }
                if(value == UserEventType.ListMemberRemove) {
                    EventTypeString = nameof(UserEventType.ListMemberRemove);
                }
                if(value == UserEventType.ListUserSubscribe) {
                    EventTypeString = nameof(UserEventType.ListUserSubscribe);
                }
                if(value == UserEventType.ListUserUnSubscribe) {
                    EventTypeString = nameof(UserEventType.ListUserUnSubscribe);
                }
                if(value == UserEventType.UserUpdate) {
                    EventTypeString = nameof(UserEventType.UserUpdate);
                }
                if(EventTypeString == null) {
                    EventTypeString = nameof(UserEventType.UserUpdate);
                }
            }
        }

        public UserEvent() { }

        public UserEvent(long userId,DateTime eventAt,long? targetUserId,long? listId,UserEventType eventType) {
            UserId = userId;
            EventAt = eventAt;
            TargetUserId = targetUserId;
            ListId = listId;
            EventType = eventType;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserEvent>().HasKey(x => new { x.UserId,x.EventAt,x.EventTypeString });
            modelBuilder.Entity<UserEvent>().Ignore(x => x.EventType);
        }
    }
}
