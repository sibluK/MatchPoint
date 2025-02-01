using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentManager.Migrations
{
    /// <inheritdoc />
    public partial class teamFieldsRemovedFromPlayerGameStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGameStats_Teams_TeamId",
                table: "PlayerGameStats");

            migrationBuilder.DropIndex(
                name: "IX_PlayerGameStats_TeamId",
                table: "PlayerGameStats");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "PlayerGameStats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "PlayerGameStats",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameStats_TeamId",
                table: "PlayerGameStats",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGameStats_Teams_TeamId",
                table: "PlayerGameStats",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
