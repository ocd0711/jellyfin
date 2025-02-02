#pragma warning disable CS1591
// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Jellyfin.Server.Implementations.Migrations
{
    public partial class AddCustomDisplayPreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DisplayPreferences_UserId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.AlterColumn<int>(
                name: "MaxActiveSessions",
                // schema: "jellyfin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                type: "VARCHAR(150)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CustomItemDisplayPreferences",
                // schema: "jellyfin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "VARCHAR(150)", nullable: false),
                    ItemId = table.Column<Guid>(type: "VARCHAR(150)", nullable: false),
                    Client = table.Column<string>(type: "VARCHAR(32)", maxLength: 32, nullable: false),
                    Key = table.Column<string>(type: "VARCHAR(150)", nullable: false),
                    Value = table.Column<string>(type: "VARCHAR(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomItemDisplayPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPreferences_UserId_ItemId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                columns: new[] { "UserId", "ItemId", "Client" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomItemDisplayPreferences_UserId",
                // schema: "jellyfin",
                table: "CustomItemDisplayPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomItemDisplayPreferences_UserId_ItemId_Client_Key",
                // schema: "jellyfin",
                table: "CustomItemDisplayPreferences",
                columns: new[] { "UserId", "ItemId", "Client", "Key" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomItemDisplayPreferences",
                schema: "jellyfin");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPreferences_UserId_ItemId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.DropColumn(
                name: "ItemId",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.AlterColumn<int>(
                name: "MaxActiveSessions",
                // schema: "jellyfin",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPreferences_UserId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                columns: new[] { "UserId", "Client" },
                unique: true);
        }
    }
}
