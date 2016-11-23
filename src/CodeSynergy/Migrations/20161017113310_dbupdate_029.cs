using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_029 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMailboxItem",
                table: "UserMailboxItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMailboxItem",
                table: "UserMailboxItem",
                columns: new[] { "UserID", "MailboxTypeID", "MailboxItemID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMailboxItem",
                table: "UserMailboxItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMailboxItem",
                table: "UserMailboxItem",
                columns: new[] { "UserID", "MailboxItemID" });
        }
    }
}
