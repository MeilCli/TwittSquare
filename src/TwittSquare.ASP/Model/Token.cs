using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CToken = CoreTweet.Tokens;

namespace TwittSquare.ASP.Model {

    [Table("tokens")]
    public class Token {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("consumer_key")]
        public string ConsumerKey { get; set; }

        [Required]
        [Column("consumer_secret")]
        public string ConsumerSecret { get; set; }

        [Required]
        [Column("access_token")]
        public string AccessToken { get; set; }

        [Required]
        [Column("access_token_secret")]
        public string AccessTokenSecret { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Token() { }

        public Token(CToken token,User user) {
            Id = user.Id;
            ConsumerKey = token.ConsumerKey;
            ConsumerSecret = token.ConsumerSecret;
            AccessToken = token.AccessToken;
            AccessTokenSecret = token.AccessTokenSecret;
            UserId = user.Id;
            User = user;
        }
    }
}
