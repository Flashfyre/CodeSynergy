using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostVote",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    QuestionPostID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vote = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVote", x => new { x.QuestionID, x.QuestionPostID });
                    table.ForeignKey(
                        name: "FK_PostVote_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PostVote_Post_QuestionID_QuestionPostID",
                        columns: x => new { x.QuestionID, x.QuestionPostID },
                        principalTable: "Post",
                        principalColumns: new[] { "QuestionID", "QuestionPostID" },
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostVote_UserID",
                table: "PostVote",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostVote_QuestionID_QuestionPostID",
                table: "PostVote",
                columns: new[] { "QuestionID", "QuestionPostID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostVote");
        }
    }
}
