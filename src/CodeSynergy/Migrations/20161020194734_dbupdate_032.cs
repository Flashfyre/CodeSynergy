using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_032 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID" },
                principalTable: "Post",
                principalColumns: new[] { "QuestionID", "QuestionPostID" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report");

            migrationBuilder.AddColumn<int>(
                name: "RepotedQuestionPostID",
                table: "Report",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "RepotedQuestionPostID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "RepotedQuestionPostID" },
                principalTable: "Post",
                principalColumns: new[] { "QuestionID", "QuestionPostID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
