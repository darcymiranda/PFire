using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserPrefs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundInChatrooms",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundOnNewMessages",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundOnScreenshots",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundOnVoicecalls",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundsOnLogOn",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlaySoundsOnNewMessagesInGame",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowFriendsOfFriends",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGameDataOnProfile",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGameServerData",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGameStatusToFriends",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowNicknames",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOfflineFriends",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTimeStampInChat",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTooltipOnDownload",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTooltipOnLogOn",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTyping",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowVoiceChatServer",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaySoundInChatrooms",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PlaySoundOnNewMessages",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PlaySoundOnScreenshots",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PlaySoundOnVoicecalls",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PlaySoundsOnLogOn",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PlaySoundsOnNewMessagesInGame",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowFriendsOfFriends",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowGameDataOnProfile",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowGameServerData",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowGameStatusToFriends",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowNicknames",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowOfflineFriends",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowTimeStampInChat",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowTooltipOnDownload",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowTooltipOnLogOn",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowTyping",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ShowVoiceChatServer",
                table: "User");
        }
    }
}
