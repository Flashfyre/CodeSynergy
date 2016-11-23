using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepVote",
                columns: table => new
                {
                    VoteeUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoterUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vote = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepVote", x => new { x.VoteeUserID, x.VoterUserID });
                    table.ForeignKey(
                        name: "FK_RepVote_AspNetUsers_VoteeUserID",
                        column: x => x.VoteeUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_RepVote_AspNetUsers_VoterUserID",
                        column: x => x.VoterUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepVote_VoteeUserID",
                table: "RepVote",
                column: "VoteeUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RepVote_VoterUserID",
                table: "RepVote",
                column: "VoterUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepVote");
        }
    }
}
