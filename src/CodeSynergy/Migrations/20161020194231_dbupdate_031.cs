using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_031 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_ReportedUserID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedItemID",
                table: "Report");

            migrationBuilder.AddColumn<short>(
                name: "ReportedPostCommentID",
                table: "Report",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "ReportedPrivateMessageID",
                table: "Report",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ReportedQuestionID",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportedQuestionPostID",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RepotedQuestionPostID",
                table: "Report",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReportedUserID",
                table: "Report",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedPrivateMessageID",
                table: "Report",
                column: "ReportedPrivateMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedQuestionID",
                table: "Report",
                column: "ReportedQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "RepotedQuestionPostID" });

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "ReportedQuestionPostID", "ReportedPostCommentID" });

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
                name: "FK_Report_AspNetUsers_ReportedUserID",
                table: "Report",
                column: "ReportedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report",
                columns: new[] { "ReportedQuestionID", "RepotedQuestionPostID" },
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
                name: "FK_Report_AspNetUsers_ReportedUserID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Post_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Comment_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedPrivateMessageID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedQuestionID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedQuestionID_RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedQuestionID_ReportedQuestionPostID_ReportedPostCommentID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedPostCommentID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedPrivateMessageID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedQuestionID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedQuestionPostID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "RepotedQuestionPostID",
                table: "Report");

            migrationBuilder.AddColumn<long>(
                name: "ReportedItemID",
                table: "Report",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "ReportedUserID",
                table: "Report",
                type: "nvarchar(450)",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_ReportedUserID",
                table: "Report",
                column: "ReportedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
