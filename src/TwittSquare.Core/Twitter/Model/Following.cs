using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwittSquare.Core.Twitter.Model {

    [Table("followings")]
    public class Following {

        [Column("user_id",Order =0)]
        public long UserId { get; set; }

        [Column("following_id",Order =1)]
        public long FollowingId { get; set; }

        [Column("cash_at")]
        public DateTime CashAt { get; set; }

        public Following() { }

        public Following(long userId,long followingId,DateTime cashAt) {
            UserId = userId;
            FollowingId = followingId;
            CashAt = cashAt;
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Following>().HasKey(x => new { x.UserId,x.FollowingId });
        }
    }

    [Table("following_tasks")]
    public class FollowingTask {

        [Key]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("take_all_at")]
        public DateTime TakeAllAt { get; set; }

        public FollowingTask() { }

        public FollowingTask(long userId,DateTime takeAllAt) {
            UserId = userId;
            TakeAllAt = takeAllAt;
        }
    }
}
