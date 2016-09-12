using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_AddedByID",
                table: "UntrustedURLPattern");

            migrationBuilder.DropIndex(
                name: "IX_UntrustedURLPattern_AddedByID",
                table: "UntrustedURLPattern");

            migrationBuilder.DropColumn(
                name: "AddedByID",
                table: "UntrustedURLPattern");

            migrationBuilder.AddForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_AddedByUserID",
                table: "UntrustedURLPattern",
                column: "AddedByUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_AddedByUserID",
                table: "UntrustedURLPattern");

            migrationBuilder.AddColumn<string>(
                name: "AddedByID",
                table: "UntrustedURLPattern",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UntrustedURLPattern_AddedByID",
                table: "UntrustedURLPattern",
                column: "AddedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_UntrustedURLPattern_AspNetUsers_AddedByID",
                table: "UntrustedURLPattern",
                column: "AddedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
