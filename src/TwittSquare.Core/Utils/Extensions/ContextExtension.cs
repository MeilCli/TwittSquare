using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TwittSquare.Core.Twitter.Model;

namespace TwittSquare.Core.Utils.Extensions {
    public static class ContextExtension {

        public static void AddOrUpdate(this DbContext ctx,object entity) {
            var entry = ctx.Entry(entity);
            switch(entry.State) {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void AddOrUpdate(this DbSet<Login> set,Login entity) {
            if(set.Any(x => x.Id == entity.Id)) {
                set.Update(entity);
            } else {
                set.Add(entity);
            }
        }

        private static string tokenUpsertSql = "INSERT INTO tokens (" +
            "id, access_token, access_token_secret, consumer_key, consumer_secret, user_id" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4, @p5" +
            ") ON CONFLICT (id) DO UPDATE SET " +
            "id = @p0, access_token = @p1, access_token_secret = @p2, consumer_key = @p3, consumer_secret = @p4, user_id = @p5";

        public static void Upsert(this DbContext context,Token entity) {
            context.Database.ExecuteSqlCommand(
                tokenUpsertSql,
                new NpgsqlParameter("p0",entity.Id),
                new NpgsqlParameter("p1",entity.AccessToken),
                new NpgsqlParameter("p2",entity.AccessTokenSecret),
                new NpgsqlParameter("p3",entity.ConsumerKey),
                new NpgsqlParameter("p4",entity.ConsumerSecret),
                new NpgsqlParameter("p5",entity.UserId)
                );
        }

        private static string userUpsertSql = "INSERT INTO users (" +
            "id, created_at, description, favourites_count, followers_count, friends_count, is_protected, is_suspended, is_verified, " +
            "listed_count, location, name, profile_banner_url, profile_image_url, screen_name, statuses_count, url" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16" +
            ") ON CONFLICT (id) DO UPDATE SET " +
            "id = @p0, screen_name = @p14, name = @p11, created_at = @p1, description = @p2, favourites_count = @p3, followers_count = @p4, " +
            "friends_count = @p5, listed_count = @p9, location = @p10, profile_banner_url = @p12, profile_image_url = @p13, is_protected = @p6, " +
            $"statuses_count = @p15, is_suspended = @p7, url = @p16, is_verified = @p8";

        public static void Upsert(this DbContext context,User entity) {
            context.Database.ExecuteSqlCommand(
                userUpsertSql,
                new NpgsqlParameter("p0",entity.Id),
                new NpgsqlParameter("p1",entity.CreatedAt),
                new NpgsqlParameter("p2",entity.Description as object ?? DBNull.Value),
                new NpgsqlParameter("p3",entity.FavouritesCount),
                new NpgsqlParameter("p4",entity.FollowersCount),
                new NpgsqlParameter("p5",entity.FriendsCount),
                new NpgsqlParameter("p6",entity.IsProtected),
                new NpgsqlParameter("p7",entity.IsSuspended),
                new NpgsqlParameter("p8",entity.IsVerified),
                new NpgsqlParameter("p9",entity.ListedCount),
                new NpgsqlParameter("p10",entity.Location as object ?? DBNull.Value),
                new NpgsqlParameter("p11",entity.Name as object ?? DBNull.Value),
                new NpgsqlParameter("p12",entity.ProfileBannerUrl as object ?? DBNull.Value),
                new NpgsqlParameter("p13",entity.ProfileImageUrl as object ?? DBNull.Value),
                new NpgsqlParameter("p14",entity.ScreenName as object ?? DBNull.Value),
                new NpgsqlParameter("p15",entity.StatusesCount),
                new NpgsqlParameter("p16",entity.Url as object ?? DBNull.Value)
                );
        }

        private const string statusUpsertSql = "INSERT INTO statuses (" +
            "id, created_at, favourite_count, in_reply_to_screen_name, in_reply_to_status_id, in_reply_to_user_id, language, quoted_status_id, " +
            "retweet_count, retweeted_status_id, source, text, user_id" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12" +
            ") ON CONFLICT (id) DO UPDATE SET " +
            "id = @p0, created_at = @p1, favourite_count = @p2, text = @p11, in_reply_to_screen_name = @p3, in_reply_to_status_id = @p4, " +
            "in_reply_to_user_id = @p5, language = @p6, quoted_status_id = @p7, retweet_count = @p8, retweeted_status_id = @p9, source = @p10, user_id = @p12";

        public static void Upsert(this DbContext context,Status entity) {
            context.Database.ExecuteSqlCommand(
                statusUpsertSql,
                new NpgsqlParameter("p0",entity.Id),
                new NpgsqlParameter("p1",entity.CreatedAt),
                new NpgsqlParameter("p2",entity.FavoriteCount),
                new NpgsqlParameter("p3",entity.InReplyToScreenName as object ?? DBNull.Value),
                new NpgsqlParameter("p4",entity.InReplyToStatusId as object ?? DBNull.Value),
                new NpgsqlParameter("p5",entity.InReplyToUserId as object ?? DBNull.Value),
                new NpgsqlParameter("p6",entity.Language as object ?? DBNull.Value),
                new NpgsqlParameter("p7",entity.QuotedStatusId as object ?? DBNull.Value),
                new NpgsqlParameter("p8",entity.RetweetCount),
                new NpgsqlParameter("p9",entity.RetweetedStatusId as object ?? DBNull.Value),
                new NpgsqlParameter("p10",entity.Source as object ?? DBNull.Value),
                new NpgsqlParameter("p11",entity.Text as object ?? DBNull.Value),
                new NpgsqlParameter("p12",entity.UserId)
                );
            if(entity.User != null) {
                context.Upsert(entity.User);
            }
            context.SaveChanges();
            if(entity.QuotedStatus != null) {
                context.Upsert(entity.QuotedStatus);
            }
            if(entity.RetweetedStatus != null) {
                context.Upsert(entity.RetweetedStatus);
            }
        }

        public static void Include(this Status status,TwitterContext context) {
            if(status.QuotedStatusId != null) {
                status.QuotedStatus = context.Statuses.FirstOrDefault(x => x.Id == status.QuotedStatusId.Value);
            }
            if(status.RetweetedStatusId != null) {
                status.RetweetedStatus = context.Statuses.FirstOrDefault(x => x.Id == status.RetweetedStatusId.Value);
            }
            if(status.QuotedStatus != null) {
                status.QuotedStatus.Include(context);
            }
            if(status.RetweetedStatus != null) {
                status.RetweetedStatus.Include(context);
            }
            status.User = context.Users.FirstOrDefault(x => x.Id == status.UserId);
        }

        private const string followingUpsertSql = "INSERT INTO followings (" +
            "user_id, following_id, cash_at" +
            ") VALUES(" +
            "@p0, @p1, @p2" +
            ") ON CONFLICT (user_id, following_id) DO UPDATE SET " +
            "user_id = @p0, following_id = @p1, cash_at = @p2";


        public static void Upsert(this DbContext context,Following entity) {
            context.Database.ExecuteSqlCommand(
                followingUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.FollowingId),
                new NpgsqlParameter("p2",entity.CashAt)
                );
        }

        private const string followingTaskUpsertSql = "INSERT INTO following_tasks (" +
            "user_id, take_all_at" +
            ") VALUES(" +
            "@p0, @p1" +
            ") ON CONFLICT (user_id) DO UPDATE SET " +
            "user_id = @p0, take_all_at = @p1";

        public static void Upsert(this DbContext context,FollowingTask entity) {
            context.Database.ExecuteSqlCommand(
                followingTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.TakeAllAt)
                );
        }

        private const string activeTimeTaskUpsertSql = "INSERT INTO active_times (" +
            "user_id, active_at" +
            ") VALUES(" +
            "@p0, @p1" +
            ") ON CONFLICT (user_id, active_at) DO UPDATE SET " +
            "user_id = @p0, active_at = @p1";

        public static void Upsert(this DbContext context,ActiveTime entity) {
            context.Database.ExecuteSqlCommand(
                activeTimeTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.ActiveAt)
                );
        }

        private const string spanTimeTaskUpsertSql = "INSERT INTO span_times (" +
            "user_id, active_at, span" +
            ") VALUES(" +
            "@p0, @p1, @p2" +
            ") ON CONFLICT (user_id, active_at) DO UPDATE SET " +
            "user_id = @p0, active_at = @p1, span = @p2";

        public static void Upsert(this DbContext context,SpanTime entity) {
            context.Database.ExecuteSqlCommand(
                spanTimeTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.ActiveAt),
                new NpgsqlParameter("p2",entity.Span)
                );
        }

        private const string deleteEventTaskUpsertSql = "INSERT INTO delete_events (" +
            "user_id, status_id, delete_at, event_type, target_user_id" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4" +
            ") ON CONFLICT (user_id, status_id, delete_at, event_type) DO UPDATE SET " +
            "user_id = @p0, status_id = @p1, delete_at = @p2, event_type = @p3, target_user_id = @p4";

        public static void Upsert(this DbContext context,DeleteEvent entity) {
            context.Database.ExecuteSqlCommand(
                deleteEventTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.StatusId),
                new NpgsqlParameter("p2",entity.DeleteAt),
                new NpgsqlParameter("p3",entity.EventTypeString),
                new NpgsqlParameter("p4",entity.TargetUserId as object ?? DBNull.Value)
                );
        }

        private const string reactiveEventTaskUpsertSql = "INSERT INTO reactive_events (" +
            "user_id, status_id, reactive_at, event_type, source_status_id, target_user_id" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4, @p5" +
            ") ON CONFLICT (user_id, status_id, reactive_at, event_type) DO UPDATE SET " +
            "user_id = @p0, status_id = @p1, reactive_at = @p2, event_type = @p3, source_status_id = @p4, target_user_id = @p5";

        public static void Upsert(this DbContext context,ReactiveEvent entity) {
            context.Database.ExecuteSqlCommand(
                reactiveEventTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.StatusId),
                new NpgsqlParameter("p2",entity.ReactiveAt),
                new NpgsqlParameter("p3",entity.EventTypeString),
                new NpgsqlParameter("p4",entity.SourceStatusId as object ?? DBNull.Value),
                new NpgsqlParameter("p5",entity.TargetUserId as object ?? DBNull.Value)
                );
        }

        private const string userEventTaskUpsertSql = "INSERT INTO user_events (" +
            "user_id, event_at, event_type, list_id, target_user_id" +
            ") VALUES(" +
            "@p0, @p1, @p2, @p3, @p4" +
            ") ON CONFLICT (user_id, event_at, event_type) DO UPDATE SET " +
            "user_id = @p0, event_at = @p1, event_type = @p2, list_id = @p3, target_user_id = @p4";

        public static void Upsert(this DbContext context,UserEvent entity) {
            context.Database.ExecuteSqlCommand(
                userEventTaskUpsertSql,
                new NpgsqlParameter("p0",entity.UserId),
                new NpgsqlParameter("p1",entity.EventAt),
                new NpgsqlParameter("p2",entity.EventTypeString),
                new NpgsqlParameter("p3",entity.ListId as object ?? DBNull.Value),
                new NpgsqlParameter("p4",entity.TargetUserId as object ?? DBNull.Value)
                );
        }

    }
}
