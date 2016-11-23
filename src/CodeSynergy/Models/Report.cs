using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Report : Message
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int ReportID { get; set; }
        [Column(TypeName = "int")]
        public int? ReportedQuestionID { get; set; }
        [ForeignKey("ReportedQuestionID")]
        public Question ReportedQuestion { get; set; }
        [Column(TypeName = "int")]
        public int? ReportedQuestionPostID { get; set; }
        [ForeignKey("ReportedQuestionID, ReportedQuestionPostID")]
        public QAPost ReportedAnswer { get; set; }
        [Column(TypeName = "smallint")]
        public short? ReportedPostCommentID { get; set; }
        [ForeignKey("ReportedQuestionID, ReportedQuestionPostID, ReportedPostCommentID")]
        public Comment ReportedComment { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string ReportedUserID { get; set; }
        [ForeignKey("ReportedUserID")]
        public ApplicationUser ReportedUser { get; set; }
        [Column(TypeName = "bigint")]
        public long? ReportedPrivateMessageID { get; set; }
        [ForeignKey("ReportedPrivateMessageID")]
        public PrivateMessage ReportedPrivateMessage { get; set; }
        [Required]
        [Column(TypeName = "tinyint")]
        public byte ReportTypeID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string ReportReason { get; set; }
        [NotMapped]
        public override string Summary {
            get
            {
                return (ReportTypeID != (byte)EnumReportType.User_Profile ? Enum.GetName(typeof(EnumReportType), (EnumReportType) ReportTypeID).Replace('_', ' ') + " " : "") + ReportedItemID;
            }
        }
        [NotMapped]
        public override string Contents {
            get
            {
                string contents = "<table id=\report-table\">" +
                    "<tbody>" +
                        "<tr>" +
                            "<td><label>Report for:</label></td>" +
                            "<td>{0}</td>" +
                        "</tr>";

                if (ReportTypeID != (byte) EnumReportType.User_Profile)
                {
                    contents +=
                        "<tr>" +
                            "<td><label>Reported User:</label></td>" +
                            "<td><a href=\"/User/" + ReportedItemUser.DisplayName + "\" class=\"blue hover-green\"><strong>" + ReportedItemUser.DisplayName + "</strong></a></td>" +
                        "</tr>";
                }

                contents +=
                        "<tr>" +
                            "<td><label>Report Reason:</label></td>" +
                            "<td>" + ReportReason + "</td>" +
                        "</tr>" +
                    "</tbody>" +
                "</table>";

                return contents;
            }
        }
        [NotMapped]
        public string ReportedItemID
        {
            get {
                return (ReportTypeID != (byte) EnumReportType.User_Profile ? "#" : "") + ReportedItem.GetReportableItemID();
            }
        }
        [NotMapped]
        public IReportable ReportedItem
        {
            get
            {
                switch ((EnumReportType)ReportTypeID)
                {
                    case EnumReportType.Question:
                        return ReportedQuestion;
                    case EnumReportType.Answer:
                        return ReportedAnswer;
                    case EnumReportType.Comment:
                        return ReportedComment;
                    case EnumReportType.User_Profile:
                        return ReportedUser;
                    default:
                        return ReportedPrivateMessage;
                }
            }
        }
        [NotMapped]
        public ApplicationUser ReportedItemUser
        {
            get
            {
                switch ((EnumReportType) ReportTypeID)
                {
                    case EnumReportType.Question:
                        return ReportedQuestion.QuestionPost.User;
                    case EnumReportType.Answer:
                        return ReportedAnswer.User;
                    case EnumReportType.Comment:
                        return ReportedComment.User;
                    case EnumReportType.Private_Message:
                        return ReportedPrivateMessage.SenderUser;
                    default:
                        return ReportedUser;
                }
            }
        }

        public void Send(ReportRepository reports, ModerationMailbox moderationMailbox)
        {
            SentDate = DateTime.Now;

            reports.Add(this);

            ModerationMailboxItem mailboxItem = new ModerationMailboxItem()
            {
                MailboxItemID = (moderationMailbox.GetAll().Any() ? moderationMailbox.GetAll().Max(i => i.MailboxItemID) : 0) + 1,
                ReportID = ReportID
            };

            moderationMailbox.Add(mailboxItem);
        }
    }

    public enum EnumReportType
    {
        Question,
        Answer,
        Comment,
        User_Profile,
        Private_Message
    }

    public static class ReportTypeHelper
    {
        public static string DisplayName(this EnumReportType reportType)
        {
            return Enum.GetName(typeof(EnumReportType), reportType).Replace('_', ' ');
        }
    }
}
