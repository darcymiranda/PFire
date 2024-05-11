using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserFavoriteServers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserServerList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameIp = table.Column<int>(type: "INTEGER", nullable: false),
                    GamePort = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServerList", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserServerList_Id",
                table: "UserServerList",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServerList");
        }
    }
}
