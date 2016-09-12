using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ban",
                columns: table => new
                {
                    BanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanLiftDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanReason = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    BannedUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BanningUserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ban", x => x.BanID);
                    table.ForeignKey(
                        name: "FK_Ban_AspNetUsers_BannedUserID",
                        column: x => x.BannedUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ban_AspNetUsers_BanningUserID",
                        column: x => x.BanningUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ban_BannedUserID",
                table: "Ban",
                column: "BannedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Ban_BanningUserID",
                table: "Ban",
                column: "BanningUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ban");
        }
    }
}
