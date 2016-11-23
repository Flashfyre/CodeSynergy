using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTag",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagID = table.Column<int>(type: "int", nullable: false),
                    AnswerCount = table.Column<int>(type: "int", nullable: false),
                    AnswerScore = table.Column<int>(type: "int", nullable: false),
                    BestAnswerCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    CommentScore = table.Column<int>(type: "int", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    QuestionScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTag", x => new { x.UserID, x.TagID });
                    table.ForeignKey(
                        name: "FK_UserTag_Tag_TagID",
                        column: x => x.TagID,
                        principalTable: "Tag",
                        principalColumn: "TagID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTag_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AlterColumn<string>(
                name: "ProfileMessage",
                table: "AspNetUsers",
                type: "nvarchar(4000)",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserTag_TagID",
                table: "UserTag",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTag_UserID",
                table: "UserTag",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTag");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileMessage",
                table: "AspNetUsers",
                type: "nvarchar(MAX)",
                nullable: true);
        }
    }
}
