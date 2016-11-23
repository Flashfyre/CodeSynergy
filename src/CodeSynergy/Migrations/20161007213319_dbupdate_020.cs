using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comment_QuestionID",
                table: "Comment",
                column: "QuestionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Question_QuestionID",
                table: "Comment",
                column: "QuestionID",
                principalTable: "Question",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Question_QuestionID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_QuestionID",
                table: "Comment");
        }
    }
}
