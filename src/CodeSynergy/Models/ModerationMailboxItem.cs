using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class ModerationMailboxItem : MailboxItem
    {
        [Column(TypeName = "nvarchar(450)")]
        public string AssigneeUserID { get; set; }
        [ForeignKey("AssigneeUserID")]
        public ApplicationUser AssigneeUser { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string AssignerUserID { get; set; }
        [ForeignKey("AssignerUserID")]
        public ApplicationUser AssignerUser { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int ReportID { get; set; }
        [ForeignKey("ReportID")]
        public Report Report { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string ActionTaken { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? AssignedDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? ResolvedDate { get; set; }
        [NotMapped]
        public override byte MailboxTypeID
        {
            get
            {
                return (byte) EnumMailboxType.Moderation;
            }
            set { }
        }
        [NotMapped]
        public override Message Message
        {
            get
            {
                return Report;
            }
        }
        [NotMapped]
        public override string RowClassHtml
        {
            get
            {
                return "mailbox-item-row" + (ReadFlag ? "" : " mailbox-item-row-unread") + (AssigneeUserID == null || ResolvedDate != null ? "" : " mailbox-item-row-assigned")
                    + (ResolvedDate == null ? "" : " mailbox-item-row-resolved");
            }
        }
        [NotMapped]
        public string ReportSummaryPrefix
        {
            get
            {
                return AssignedDate == null ? ResolvedDate == null ? "" : "[Assigned] " : "[Resolved] ";
            }
        }

        public void MarkAsRead(ModerationMailbox mailbox, bool markAsUnread)
        {
            ReadFlag = !markAsUnread;
            mailbox.Update(this);
        }

        public void Delete(ModerationMailbox mailbox)
        {
            DeletedDate = DateTime.Now;
            mailbox.Update(this);
        }

        public void AssignToUser(ModerationMailbox mailbox, ApplicationUser assigneeUser, ApplicationUser assignerUser)
        {
            AssignedDate = DateTime.Now;
            AssigneeUserID = assigneeUser.Id;
            AssignerUserID = assignerUser.Id;
            mailbox.Update(this);
        }

        public void Unassign(ModerationMailbox mailbox)
        {
            AssignedDate = null;
            AssigneeUserID = null;
            AssignerUserID = null;
            mailbox.Update(this);
        }

        public void MarkAsResolved(ModerationMailbox mailbox, string actionTaken)
        {
            ResolvedDate = DateTime.Now;
            ActionTaken = actionTaken;
            mailbox.Update(this);
        }

        public void MarkAsUnresolved(ModerationMailbox mailbox)
        {
            ResolvedDate = null;
            ActionTaken = null;
        }

        public override string[] GetSearchText()
        {
            return new string[] { Report.Summary, Report.Contents, Report.ReportReason, Report.SenderName, AssigneeUserID != null ? AssigneeUser.DisplayName : "", Report.ReportedItemUser.DisplayName };
        }
    }
}
