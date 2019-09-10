using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage;
using Constant = TwittSquare.Core.Twitter.Constant;

namespace TwittSquare.ASP.Model {
    public class TwitterContext : DbContext {

        public DbSet<Login> Logins { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<FollowingTask> FollowingTasks { get; set; }
        public DbSet<ActiveTime> ActiveTimes { get; set; }
        public DbSet<SpanTime> SpanTimes { get; set; }
        public DbSet<DeleteEvent> DeleteEvents { get; set; }
        public DbSet<ReactiveEvent> ReactiveEvents { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }

        public TwitterContext() : base() {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(Constant.DatabaseConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("public");
            User.OnModelCreating(modelBuilder);
            Login.OnModelCreating(modelBuilder);
            Status.OnModelCreating(modelBuilder);
            Following.OnModelCreating(modelBuilder);
            ActiveTime.OnModelCreating(modelBuilder);
            SpanTime.OnModelCreating(modelBuilder);
            DeleteEvent.OnModelCreating(modelBuilder);
            ReactiveEvent.OnModelCreating(modelBuilder);
            UserEvent.OnModelCreating(modelBuilder);
        }
    }
}
