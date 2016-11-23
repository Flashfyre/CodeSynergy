using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_036 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignerUserID",
                table: "ModerationMailboxItem",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModerationMailboxItem_AssignerUserID",
                table: "ModerationMailboxItem",
                column: "AssignerUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ModerationMailboxItem_AspNetUsers_AssignerUserID",
                table: "ModerationMailboxItem",
                column: "AssignerUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModerationMailboxItem_AspNetUsers_AssignerUserID",
                table: "ModerationMailboxItem");

            migrationBuilder.DropIndex(
                name: "IX_ModerationMailboxItem_AssignerUserID",
                table: "ModerationMailboxItem");

            migrationBuilder.DropColumn(
                name: "AssignerUserID",
                table: "ModerationMailboxItem");
        }
    }
}
