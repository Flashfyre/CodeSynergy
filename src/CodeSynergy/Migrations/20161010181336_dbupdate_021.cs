using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedFlag",
                table: "Question");

            migrationBuilder.AddColumn<string>(
                name: "LockedByUserID",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedDate",
                table: "Question",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_LockedByUserID",
                table: "Question",
                column: "LockedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_AspNetUsers_LockedByUserID",
                table: "Question",
                column: "LockedByUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_AspNetUsers_LockedByUserID",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_LockedByUserID",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "LockedByUserID",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "LockedDate",
                table: "Question");

            migrationBuilder.AddColumn<bool>(
                name: "LockedFlag",
                table: "Question",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
