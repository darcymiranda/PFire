using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFire.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberIds = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_Id",
                table: "UserGroup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_Name",
                table: "UserGroup",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroup");
        }
    }
}
