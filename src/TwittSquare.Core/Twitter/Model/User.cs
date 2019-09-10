using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;
using Microsoft.EntityFrameworkCore;
using TwittSquare.Core.Utils.Extensions;
using CUser = CoreTweet.User;

namespace TwittSquare.Core.Twitter.Model {

    [Table("users")]
    public class User {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("screen_name")]
        public string ScreenName { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("favourites_count")]
        public int FavouritesCount { get; set; }

        [Column("followers_count")]
        public int FollowersCount { get; set; }

        [Column("friends_count")]
        public int FriendsCount { get; set; }

        [Column("listed_count")]
        public int ListedCount { get; set; }

        [Column("location")]
        public string Location { get; set; }

        [Column("profile_banner_url")]
        public string ProfileBannerUrl { get; set; }

        [Column("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [Column("is_protected")]
        public bool IsProtected { get; set; }

        [Column("statuses_count")]
        public int StatusesCount { get; set; }

        [Column("is_suspended")]
        public bool IsSuspended { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("is_verified")]
        public bool IsVerified { get; set; }

        public User() { }

        public User(CUser user) {
            Update(user);
        }

        public void Update(CUser user) {
            Id = user.Id ?? -1;
            ScreenName = user.ScreenName;
            Name = user.Name;
            CreatedAt = user.CreatedAt.LocalDateTime;
            if(user.Description != null) {
                Description = CoreTweetSupplement.EnumerateTextParts(user.Description,user.Entities?.Description).ToText();
            }else {
                Description = string.Empty;
            }
            FavouritesCount = user.FavouritesCount;
            FollowersCount = user.FollowersCount;
            FriendsCount = user.FriendsCount;
            ListedCount = user.ListedCount ?? 0;
            Location = user.Location ?? string.Empty;
            ProfileBannerUrl = user.ProfileBannerUrl != null ? $"{user.ProfileBannerUrl}/web" : null;
            ProfileImageUrl = user.ProfileImageUrl != null ? user.GetProfileImageUrl("bigger").AbsoluteUri : null;
            IsProtected = user.IsProtected;
            StatusesCount = user.StatusesCount;
            IsSuspended = user.IsSuspended ?? false;
            if(user.Url != null) {
                Url = CoreTweetSupplement.EnumerateTextParts(user.Url,user.Entities?.Url).ToText();
            } else {
                Url = string.Empty;
            }
            IsVerified = user.IsVerified;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasIndex(x => new { x.ScreenName,x.Description });
        }
    }
}
