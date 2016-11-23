using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankingCategory",
                columns: table => new
                {
                    RankingCategoryID = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RankingName = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingCategory", x => x.RankingCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "RankingPos",
                columns: table => new
                {
                    RankingPosID = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingPos", x => x.RankingPosID);
                });

            migrationBuilder.CreateTable(
                name: "Ranking",
                columns: table => new
                {
                    RankingCategoryID = table.Column<byte>(type: "tinyint", nullable: false),
                    RankingPosID = table.Column<short>(type: "smallint", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranking", x => new { x.RankingCategoryID, x.RankingPosID });
                    table.ForeignKey(
                        name: "FK_Ranking_RankingCategory_RankingCategoryID",
                        column: x => x.RankingCategoryID,
                        principalTable: "RankingCategory",
                        principalColumn: "RankingCategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ranking_RankingPos_RankingPosID",
                        column: x => x.RankingPosID,
                        principalTable: "RankingPos",
                        principalColumn: "RankingPosID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ranking_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ranking_RankingCategoryID",
                table: "Ranking",
                column: "RankingCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Ranking_RankingPosID",
                table: "Ranking",
                column: "RankingPosID");

            migrationBuilder.CreateIndex(
                name: "IX_Ranking_UserID",
                table: "Ranking",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ranking");

            migrationBuilder.DropTable(
                name: "RankingCategory");

            migrationBuilder.DropTable(
                name: "RankingPos");
        }
    }
}
