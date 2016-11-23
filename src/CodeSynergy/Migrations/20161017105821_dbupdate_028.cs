using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeSynergy.Migrations
{
    public partial class dbupdate_028 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivateMessage",
                columns: table => new
                {
                    PrivateMessageID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contents = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipientUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessage", x => x.PrivateMessageID);
                    table.ForeignKey(
                        name: "FK_PrivateMessage_AspNetUsers_RecipientUserID",
                        column: x => x.RecipientUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateMessage_AspNetUsers_SenderUserID",
                        column: x => x.SenderUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReportReason = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    ReportTypeID = table.Column<byte>(type: "tinyint", nullable: false),
                    ReportedItemID = table.Column<long>(type: "bigint", nullable: false),
                    ReportedUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_ReportedUserID",
                        column: x => x.ReportedUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_SenderUserID",
                        column: x => x.SenderUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMailbox",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MailboxTypeID = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMailbox", x => new { x.UserID, x.MailboxTypeID });
                    table.ForeignKey(
                        name: "FK_UserMailbox_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ModerationMailboxItem",
                columns: table => new
                {
                    MailboxItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionTaken = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssigneeUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadFlag = table.Column<bool>(type: "bit", nullable: false),
                    ReportID = table.Column<int>(type: "int", nullable: false),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationMailboxItem", x => x.MailboxItemID);
                    table.ForeignKey(
                        name: "FK_ModerationMailboxItem_AspNetUsers_AssigneeUserID",
                        column: x => x.AssigneeUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModerationMailboxItem_Report_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Report",
                        principalColumn: "ReportID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMailboxItem",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MailboxItemID = table.Column<int>(type: "int", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MailboxTypeID = table.Column<byte>(type: "tinyint", nullable: false),
                    PrivateMessageID = table.Column<long>(type: "bigint", nullable: false),
                    ReadFlag = table.Column<bool>(type: "bit", nullable: false),
                    StarredDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMailboxItem", x => new { x.UserID, x.MailboxItemID });
                    table.ForeignKey(
                        name: "FK_UserMailboxItem_PrivateMessage_PrivateMessageID",
                        column: x => x.PrivateMessageID,
                        principalTable: "PrivateMessage",
                        principalColumn: "PrivateMessageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMailboxItem_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMailboxItem_UserMailbox_UserID_MailboxTypeID",
                        columns: x => new { x.UserID, x.MailboxTypeID },
                        principalTable: "UserMailbox",
                        principalColumns: new[] { "UserID", "MailboxTypeID" },
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationMailboxItem_AssigneeUserID",
                table: "ModerationMailboxItem",
                column: "AssigneeUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationMailboxItem_ReportID",
                table: "ModerationMailboxItem",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessage_RecipientUserID",
                table: "PrivateMessage",
                column: "RecipientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessage_SenderUserID",
                table: "PrivateMessage",
                column: "SenderUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedUserID",
                table: "Report",
                column: "ReportedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_SenderUserID",
                table: "Report",
                column: "SenderUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMailbox_UserID",
                table: "UserMailbox",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMailboxItem_PrivateMessageID",
                table: "UserMailboxItem",
                column: "PrivateMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMailboxItem_UserID",
                table: "UserMailboxItem",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMailboxItem_UserID_MailboxTypeID",
                table: "UserMailboxItem",
                columns: new[] { "UserID", "MailboxTypeID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModerationMailboxItem");

            migrationBuilder.DropTable(
                name: "UserMailboxItem");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "PrivateMessage");

            migrationBuilder.DropTable(
                name: "UserMailbox");
        }
    }
}
