using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_017 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BestAnswerQuestionPostID",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "StarCount",
                table: "Question",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "BestAnswerCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StarCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestAnswerQuestionPostID",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "StarCount",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "BestAnswerCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StarCount",
                table: "AspNetUsers");
        }
    }
}
