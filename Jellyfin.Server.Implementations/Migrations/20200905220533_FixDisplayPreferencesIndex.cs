#pragma warning disable CS1591
#pragma warning disable SA1601

using Microsoft.EntityFrameworkCore.Migrations;

namespace Jellyfin.Server.Implementations.Migrations
{
    public partial class FixDisplayPreferencesIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayPreferences_Users_UserId",
                table: "DisplayPreferences");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPreferences_UserId",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPreferences_UserId",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPreferences_UserId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                columns: new[] { "UserId", "Client" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_DisplayPreferences_Users_UserId",
            //     table: "DisplayPreferences");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPreferences_UserId_Client",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPreferences_UserId",
                // schema: "jellyfin",
                table: "DisplayPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPreferences_UserId",
                // schema: "jellyfin",
                table: "DisplayPreferences",
                column: "UserId",
                unique: true);
            
            migrationBuilder.AddForeignKey(
                name: "FK_DisplayPreferences_Users_UserId",
                table: "DisplayPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
