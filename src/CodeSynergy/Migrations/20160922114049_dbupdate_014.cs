using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    QuestionPostID = table.Column<int>(type: "int", nullable: false),
                    PostCommentID = table.Column<short>(type: "smallint", nullable: false),
                    Contents = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    DeletedFlag = table.Column<bool>(type: "bit", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<short>(type: "smallint", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => new { x.QuestionID, x.QuestionPostID, x.PostCommentID });
                    table.ForeignKey(
                        name: "FK_Comment_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Comment_Post_QuestionID_QuestionPostID",
                        columns: x => new { x.QuestionID, x.QuestionPostID },
                        principalTable: "Post",
                        principalColumns: new[] { "QuestionID", "QuestionPostID" },
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CommentVote",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    QuestionPostID = table.Column<int>(type: "int", nullable: false),
                    PostCommentID = table.Column<short>(type: "smallint", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vote = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentVote", x => new { x.QuestionID, x.QuestionPostID, x.PostCommentID, x.UserID });
                    table.ForeignKey(
                        name: "FK_CommentVote_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CommentVote_Comment_QuestionID_QuestionPostID_PostCommentID",
                        columns: x => new { x.QuestionID, x.QuestionPostID, x.PostCommentID },
                        principalTable: "Comment",
                        principalColumns: new[] { "QuestionID", "QuestionPostID", "PostCommentID" },
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserID",
                table: "Comment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_QuestionID_QuestionPostID",
                table: "Comment",
                columns: new[] { "QuestionID", "QuestionPostID" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentVote_UserID",
                table: "CommentVote",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVote_QuestionID_QuestionPostID_PostCommentID",
                table: "CommentVote",
                columns: new[] { "QuestionID", "QuestionPostID", "PostCommentID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentVote");

            migrationBuilder.DropTable(
                name: "Comment");
        }
    }
}
