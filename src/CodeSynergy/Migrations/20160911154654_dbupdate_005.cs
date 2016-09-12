using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UntrustedURLPattern",
                columns: table => new
                {
                    PatternID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatternText = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    RemovedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UntrustedURLPattern", x => x.PatternID);
                    table.ForeignKey(
                        name: "FK_UntrustedURLPattern_AspNetUsers_AddedByUserID",
                        column: x => x.AddedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UntrustedURLPattern_AspNetUsers_RemovedByUserID",
                        column: x => x.RemovedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UntrustedURLPattern_AddedByUserID",
                table: "UntrustedURLPattern",
                column: "AddedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UntrustedURLPattern_RemovedByUserID",
                table: "UntrustedURLPattern",
                column: "RemovedByUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UntrustedURLPattern");
        }
    }
}
