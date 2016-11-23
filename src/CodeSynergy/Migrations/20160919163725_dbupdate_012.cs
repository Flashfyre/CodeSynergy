using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostVote",
                table: "PostVote");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostVote",
                table: "PostVote",
                columns: new[] { "QuestionID", "QuestionPostID", "UserID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostVote",
                table: "PostVote");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostVote",
                table: "PostVote",
                columns: new[] { "QuestionID", "QuestionPostID" });
        }
    }
}
