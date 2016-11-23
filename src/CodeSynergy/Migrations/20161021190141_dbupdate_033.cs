using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_033 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_PrivateMessage_ReportedPrivateMessageID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Question_ReportedQuestionID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Comment_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report");

            migrationBuilder.AlterColumn<int>(
                name: "ReportedQuestionPostID",
                table: "Report",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReportedQuestionID",
                table: "Report",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ReportedPrivateMessageID",
                table: "Report",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "ReportedPostCommentID",
                table: "Report",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_PrivateMessage_ReportedPrivateMessageID",
                table: "Report",
                column: "ReportedPrivateMessageID",
                principalTable: "PrivateMessage",
                principalColumn: "PrivateMessageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Question_ReportedQuestionID",
                table: "Report",
                column: "ReportedQuestionID",
                principalTable: "Question",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID" },
                principalTable: "Post",
                principalColumns: new[] { "QuestionID", "QuestionPostID" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Comment_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID", "ReportedPostCommentID" },
                principalTable: "Comment",
                principalColumns: new[] { "QuestionID", "QuestionPostID", "PostCommentID" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_PrivateMessage_ReportedPrivateMessageID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Question_ReportedQuestionID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Comment_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report");

            migrationBuilder.AlterColumn<int>(
                name: "ReportedQuestionPostID",
                table: "Report",
                type: "int",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "ReportedQuestionID",
                table: "Report",
                type: "int",
                nullable: false);

            migrationBuilder.AlterColumn<long>(
                name: "ReportedPrivateMessageID",
                table: "Report",
                type: "bigint",
                nullable: false);

            migrationBuilder.AlterColumn<short>(
                name: "ReportedPostCommentID",
                table: "Report",
                type: "smallint",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_PrivateMessage_ReportedPrivateMessageID",
                table: "Report",
                column: "ReportedPrivateMessageID",
                principalTable: "PrivateMessage",
                principalColumn: "PrivateMessageID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Question_ReportedQuestionID",
                table: "Report",
                column: "ReportedQuestionID",
                principalTable: "Question",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_ReportedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID" },
                principalTable: "Post",
                principalColumns: new[] { "QuestionID", "QuestionPostID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Comment_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID", "ReportedPostCommentID" },
                principalTable: "Comment",
                principalColumns: new[] { "QuestionID", "QuestionPostID", "PostCommentID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
