using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwittSquare.ASP.Model {

    [Table("logins")]
    public class Login {
        
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // id+(random token)
        [Required]
        [Column("token")]
        public string Token { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Column("last_login_at")]
        public DateTime LastLoginAt { get; set; }

        [Column("browser")]
        public string Browser { get; set; }

        [Column("platform")]
        public string Platform { get; set; }

        internal static void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Login>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Login>().HasIndex(x => x.Token).IsUnique();
            modelBuilder.Entity<Login>().HasIndex(x => x.Expires);
        }
    }
}
