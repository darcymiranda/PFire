using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClientPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameStatusShowMyFriends = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameStatusShowMyGameServer = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameStatusShowMyProfile = table.Column<bool>(type: "INTEGER", nullable: false),
                    ChatShowTimestamps = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowVoiceChatServerToFriends = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowWhenTyping = table.Column<bool>(type: "INTEGER", nullable: false),
                    GameStatusShowFriendOfFriends = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundSendOrReceiveMessage = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundReceiveMessageWhileGaming = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundFriendLogsOnOff = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowOfflineFriends = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowNicknames = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotificationFriendLogsOnOff = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotificationDownloadStartsFinishes = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundSomeoneJoinsLeaveChatroom = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundSendReceiveVoiceChatRequest = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaySoundScreenshotWhileGaming = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotificationConnectionStateChanges = table.Column<bool>(type: "INTEGER", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPreferences_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPreferences_UserId",
                table: "ClientPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPreferences");
        }
    }
}
