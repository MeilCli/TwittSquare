using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;
using Microsoft.EntityFrameworkCore;
using TwittSquare.Core.Utils.Extensions;
using CStatus = CoreTweet.Status;

namespace TwittSquare.Core.Twitter.Model {

    [Table("statuses")]
    public class Status {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("favourite_count")]
        public int FavoriteCount { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        [Column("in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [Column("in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [Column("language")]
        public string Language { get; set; }

        [Column("quoted_status_id")]
        public long? QuotedStatusId { get; set; }

        [NotMapped]
        public Status QuotedStatus { get; set; }

        [Column("retweet_count")]
        public int RetweetCount { get; set; }

        [Column("retweeted_status_id")]
        public long? RetweetedStatusId { get; set; }

        [NotMapped]
        public Status RetweetedStatus { get; set; }

        [Column("source")]
        public string Source { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [NotMapped]
        public User User { get; set; }

        public Status() { }

        public Status(CStatus status) {
            Update(status);
        }

        public void Update(CStatus status) {
            Id = status.Id;
            CreatedAt = status.CreatedAt.LocalDateTime;
            FavoriteCount = status.FavoriteCount ?? 0;
            var extended = status.GetExtendedTweetElements();
            Text = $"{string.Join(" ",extended.HiddenPrefix.Select(x=>"@"+x.ScreenName))} {extended.TweetText.ToText()} {string.Join(" ",extended.HiddenSuffix.Select(x=>x.ExpandedUrl))}";
            InReplyToScreenName = status.InReplyToScreenName;
            InReplyToStatusId = status.InReplyToStatusId;
            InReplyToUserId = status.InReplyToUserId;
            Language = status.Language;
            if(status.QuotedStatus != null) {
                QuotedStatusId = status.QuotedStatus.Id;
                QuotedStatus = new Status(status.QuotedStatus);
            }
            RetweetCount = status.RetweetCount ?? 0;
            if(status.RetweetedStatus != null) {
                RetweetedStatusId = status.RetweetedStatus.Id;
                RetweetedStatus = new Status(status.RetweetedStatus);
            }
            Source = status.ParseSource().Name;
            UserId = status.User.Id ?? -1;
            User = new User(status.User);
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Status>().HasIndex(x => new { x.Text });
        }

    }
   
}
