using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatroomEnt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatroomEnt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cid = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultPerms = table.Column<int>(type: "INTEGER", nullable: false),
                    Users = table.Column<string>(type: "TEXT", nullable: true),
                    PowerUsers = table.Column<string>(type: "TEXT", nullable: true),
                    Moderators = table.Column<string>(type: "TEXT", nullable: true),
                    Administrators = table.Column<string>(type: "TEXT", nullable: true),
                    SilencedUsers = table.Column<string>(type: "TEXT", nullable: true),
                    ShowJoinLeaveMessages = table.Column<byte>(type: "INTEGER", nullable: false),
                    SavedRoom = table.Column<byte>(type: "INTEGER", nullable: false),
                    Silenced = table.Column<byte>(type: "INTEGER", nullable: false),
                    MOTD = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    GameLobbyHost = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyIP = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyPort = table.Column<int>(type: "INTEGER", nullable: false),
                    GameLobbyPlayers = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomEnt", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomEnt_Id",
                table: "ChatroomEnt",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatroomEnt");
        }
    }
}
