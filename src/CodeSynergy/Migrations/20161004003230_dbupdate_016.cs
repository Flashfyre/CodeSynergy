using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_016 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnswerScore",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommentScore",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionScore",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Post",
                type: "nvarchar(4000)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Comment",
                type: "nvarchar(4000)",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerScore",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommentScore",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "QuestionScore",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Post",
                type: "nvarchar(MAX)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Comment",
                type: "nvarchar(MAX)",
                nullable: false);
        }
    }
}
