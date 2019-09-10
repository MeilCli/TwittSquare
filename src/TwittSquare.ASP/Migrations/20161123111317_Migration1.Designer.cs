using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TwittSquare.ASP.Model;

namespace TwittSquare.ASP.Migrations
{
    [DbContext(typeof(TwitterContext))]
    [Migration("20161123111317_Migration1")]
    partial class Migration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("TwittSquare.ASP.Model.ActiveTime", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("ActiveAt")
                        .HasColumnName("active_at");

                    b.HasKey("UserId", "ActiveAt");

                    b.ToTable("active_times");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.DeleteEvent", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<long>("StatusId")
                        .HasColumnName("status_id");

                    b.Property<DateTime>("DeleteAt")
                        .HasColumnName("delete_at");

                    b.Property<string>("EventTypeString")
                        .HasColumnName("event_type");

                    b.Property<long?>("TargetUserId")
                        .HasColumnName("target_user_id");

                    b.HasKey("UserId", "StatusId", "DeleteAt", "EventTypeString");

                    b.ToTable("delete_events");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.Following", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<long>("FollowingId")
                        .HasColumnName("following_id");

                    b.Property<DateTime>("CashAt")
                        .HasColumnName("cash_at");

                    b.HasKey("UserId", "FollowingId");

                    b.ToTable("followings");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.FollowingTask", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("user_id");

                    b.Property<DateTime>("TakeAllAt")
                        .HasColumnName("take_all_at");

                    b.HasKey("UserId");

                    b.ToTable("following_tasks");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.Login", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Browser")
                        .HasColumnName("browser");

                    b.Property<DateTime>("Expires")
                        .HasColumnName("expires");

                    b.Property<DateTime>("LastLoginAt")
                        .HasColumnName("last_login_at");

                    b.Property<string>("Platform")
                        .HasColumnName("platform");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnName("token");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("Expires");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.ToTable("logins");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.ReactiveEvent", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<long>("StatusId")
                        .HasColumnName("status_id");

                    b.Property<DateTime>("ReactiveAt")
                        .HasColumnName("reactive_at");

                    b.Property<string>("EventTypeString")
                        .HasColumnName("event_type");

                    b.Property<long?>("SourceStatusId")
                        .HasColumnName("source_status_id");

                    b.Property<long>("TargetUserId")
                        .HasColumnName("target_user_id");

                    b.HasKey("UserId", "StatusId", "ReactiveAt", "EventTypeString");

                    b.ToTable("reactive_events");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.SpanTime", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("ActiveAt")
                        .HasColumnName("active_at");

                    b.Property<TimeSpan>("Span")
                        .HasColumnName("span");

                    b.HasKey("UserId", "ActiveAt");

                    b.ToTable("span_times");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.Status", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<int>("FavoriteCount")
                        .HasColumnName("favourite_count");

                    b.Property<string>("InReplyToScreenName")
                        .HasColumnName("in_reply_to_screen_name");

                    b.Property<long?>("InReplyToStatusId")
                        .HasColumnName("in_reply_to_status_id");

                    b.Property<long?>("InReplyToUserId")
                        .HasColumnName("in_reply_to_user_id");

                    b.Property<string>("Language")
                        .HasColumnName("language");

                    b.Property<long?>("QuotedStatusId")
                        .HasColumnName("quoted_status_id");

                    b.Property<int>("RetweetCount")
                        .HasColumnName("retweet_count");

                    b.Property<long?>("RetweetedStatusId")
                        .HasColumnName("retweeted_status_id");

                    b.Property<string>("Source")
                        .HasColumnName("source");

                    b.Property<string>("Text")
                        .HasColumnName("text");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("Text");

                    b.ToTable("statuses");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.Token", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnName("access_token");

                    b.Property<string>("AccessTokenSecret")
                        .IsRequired()
                        .HasColumnName("access_token_secret");

                    b.Property<string>("ConsumerKey")
                        .IsRequired()
                        .HasColumnName("consumer_key");

                    b.Property<string>("ConsumerSecret")
                        .IsRequired()
                        .HasColumnName("consumer_secret");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("tokens");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<int>("FavouritesCount")
                        .HasColumnName("favourites_count");

                    b.Property<int>("FollowersCount")
                        .HasColumnName("followers_count");

                    b.Property<int>("FriendsCount")
                        .HasColumnName("friends_count");

                    b.Property<bool>("IsProtected")
                        .HasColumnName("is_protected");

                    b.Property<bool>("IsSuspended")
                        .HasColumnName("is_suspended");

                    b.Property<bool>("IsVerified")
                        .HasColumnName("is_verified");

                    b.Property<int>("ListedCount")
                        .HasColumnName("listed_count");

                    b.Property<string>("Location")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("ProfileBannerUrl")
                        .HasColumnName("profile_banner_url");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnName("profile_image_url");

                    b.Property<string>("ScreenName")
                        .HasColumnName("screen_name");

                    b.Property<int>("StatusesCount")
                        .HasColumnName("statuses_count");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.HasKey("Id");

                    b.HasIndex("ScreenName", "Description");

                    b.ToTable("users");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.UserEvent", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("EventAt")
                        .HasColumnName("event_at");

                    b.Property<string>("EventTypeString")
                        .HasColumnName("event_type");

                    b.Property<long?>("ListId")
                        .HasColumnName("list_id");

                    b.Property<long?>("TargetUserId")
                        .HasColumnName("target_user_id");

                    b.HasKey("UserId", "EventAt", "EventTypeString");

                    b.ToTable("user_events");
                });

            modelBuilder.Entity("TwittSquare.ASP.Model.Token", b =>
                {
                    b.HasOne("TwittSquare.ASP.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
