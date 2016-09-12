using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedByUserID",
                table: "UntrustedURLPattern",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UntrustedURLPattern_LastUpdatedByUserID",
                table: "UntrustedURLPattern",
                column: "LastUpdatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_LastUpdatedByUserID",
                table: "UntrustedURLPattern",
                column: "LastUpdatedByUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_LastUpdatedByUserID",
                table: "UntrustedURLPattern");

            migrationBuilder.DropIndex(
                name: "IX_UntrustedURLPattern_LastUpdatedByUserID",
                table: "UntrustedURLPattern");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserID",
                table: "UntrustedURLPattern");
        }
    }
}
