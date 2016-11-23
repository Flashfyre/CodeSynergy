using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_037 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessage_AspNetUsers_SenderUserID",
                table: "PrivateMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_SenderUserID",
                table: "Report");

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserID",
                table: "Report",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserID",
                table: "PrivateMessage",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessage_AspNetUsers_SenderUserID",
                table: "PrivateMessage",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_SenderUserID",
                table: "Report",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessage_AspNetUsers_SenderUserID",
                table: "PrivateMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_SenderUserID",
                table: "Report");

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserID",
                table: "Report",
                type: "nvarchar(450)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "SenderUserID",
                table: "PrivateMessage",
                type: "nvarchar(450)",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessage_AspNetUsers_SenderUserID",
                table: "PrivateMessage",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_SenderUserID",
                table: "Report",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
