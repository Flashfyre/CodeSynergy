using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_034 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedDate",
                table: "ModerationMailboxItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedDate",
                table: "ModerationMailboxItem",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolvedDate",
                table: "ModerationMailboxItem",
                type: "datetime2",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedDate",
                table: "ModerationMailboxItem",
                type: "datetime2",
                nullable: false);
        }
    }
}
