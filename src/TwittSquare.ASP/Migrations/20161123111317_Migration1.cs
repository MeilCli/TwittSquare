using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwittSquare.ASP.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "active_times",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    active_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_active_times", x => new { x.user_id, x.active_at });
                });

            migrationBuilder.CreateTable(
                name: "delete_events",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    status_id = table.Column<long>(nullable: false),
                    delete_at = table.Column<DateTime>(nullable: false),
                    event_type = table.Column<string>(nullable: false),
                    target_user_id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delete_events", x => new { x.user_id, x.status_id, x.delete_at, x.event_type });
                });

            migrationBuilder.CreateTable(
                name: "followings",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    following_id = table.Column<long>(nullable: false),
                    cash_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_followings", x => new { x.user_id, x.following_id });
                });

            migrationBuilder.CreateTable(
                name: "following_tasks",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    take_all_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_following_tasks", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "logins",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    browser = table.Column<string>(nullable: true),
                    expires = table.Column<DateTime>(nullable: false),
                    last_login_at = table.Column<DateTime>(nullable: false),
                    platform = table.Column<string>(nullable: true),
                    token = table.Column<string>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reactive_events",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    status_id = table.Column<long>(nullable: false),
                    reactive_at = table.Column<DateTime>(nullable: false),
                    event_type = table.Column<string>(nullable: false),
                    source_status_id = table.Column<long>(nullable: true),
                    target_user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reactive_events", x => new { x.user_id, x.status_id, x.reactive_at, x.event_type });
                });

            migrationBuilder.CreateTable(
                name: "span_times",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    active_at = table.Column<DateTime>(nullable: false),
                    span = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_span_times", x => new { x.user_id, x.active_at });
                });

            migrationBuilder.CreateTable(
                name: "statuses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    favourite_count = table.Column<int>(nullable: false),
                    in_reply_to_screen_name = table.Column<string>(nullable: true),
                    in_reply_to_status_id = table.Column<long>(nullable: true),
                    in_reply_to_user_id = table.Column<long>(nullable: true),
                    language = table.Column<string>(nullable: true),
                    quoted_status_id = table.Column<long>(nullable: true),
                    retweet_count = table.Column<int>(nullable: false),
                    retweeted_status_id = table.Column<long>(nullable: true),
                    source = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: true),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    favourites_count = table.Column<int>(nullable: false),
                    followers_count = table.Column<int>(nullable: false),
                    friends_count = table.Column<int>(nullable: false),
                    is_protected = table.Column<bool>(nullable: false),
                    is_suspended = table.Column<bool>(nullable: false),
                    is_verified = table.Column<bool>(nullable: false),
                    listed_count = table.Column<int>(nullable: false),
                    location = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    profile_banner_url = table.Column<string>(nullable: true),
                    profile_image_url = table.Column<string>(nullable: true),
                    screen_name = table.Column<string>(nullable: true),
                    statuses_count = table.Column<int>(nullable: false),
                    url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_events",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    event_at = table.Column<DateTime>(nullable: false),
                    event_type = table.Column<string>(nullable: false),
                    list_id = table.Column<long>(nullable: true),
                    target_user_id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_events", x => new { x.user_id, x.event_at, x.event_type });
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    access_token = table.Column<string>(nullable: false),
                    access_token_secret = table.Column<string>(nullable: false),
                    consumer_key = table.Column<string>(nullable: false),
                    consumer_secret = table.Column<string>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_logins_expires",
                schema: "public",
                table: "logins",
                column: "expires");

            migrationBuilder.CreateIndex(
                name: "IX_logins_token",
                schema: "public",
                table: "logins",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_statuses_text",
                schema: "public",
                table: "statuses",
                column: "text");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_user_id",
                schema: "public",
                table: "tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_screen_name_description",
                schema: "public",
                table: "users",
                columns: new[] { "screen_name", "description" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "active_times",
                schema: "public");

            migrationBuilder.DropTable(
                name: "delete_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "followings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "following_tasks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "logins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reactive_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "span_times",
                schema: "public");

            migrationBuilder.DropTable(
                name: "statuses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");
        }
    }
}
