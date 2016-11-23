using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class PrivateMessage : Message, IReportable
    {
        [Key]
        [Required]
        [Column(TypeName = "bigint")]
        public long PrivateMessageID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string RecipientUserID { get; set; }
        [ForeignKey("RecipientUserID")]
        public ApplicationUser RecipientUser { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public override string Summary { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public override string Contents { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }

        public void Send(PrivateMessageRepository privateMessages, MailboxRepository mailboxes)
        {
            UserMailbox senderMailbox = mailboxes.Find(SenderUserID, (byte) EnumMailboxType.Sent);
            UserMailbox recipientMailbox = mailboxes.Find(RecipientUserID, (byte) EnumMailboxType.Inbox);

            SentDate = DateTime.Now;

            privateMessages.Add(this);

            UserMailboxItem senderItem = new UserMailboxItem()
            {
                UserID = SenderUserID,
                MailboxTypeID = (byte) EnumMailboxType.Sent,
                PrivateMessageID = PrivateMessageID,
                ReadFlag = RecipientUserID != SenderUserID
            };
            UserMailboxItem recipientItem = new UserMailboxItem()
            {
                UserID = RecipientUserID,
                MailboxTypeID = (byte) EnumMailboxType.Inbox,
                PrivateMessageID = PrivateMessageID,
                ReadFlag = false
            };

            senderItem.MailboxItemID = (senderMailbox.Items.Any() ? senderMailbox.Items.Max(i => i.MailboxItemID) : 0) + 1;
            senderMailbox.UserItems.Add(senderItem);
            recipientItem.MailboxItemID = (recipientMailbox.Items.Any() ? recipientMailbox.Items.Max(i => i.MailboxItemID) : 0) + 1;
            recipientMailbox.UserItems.Add(recipientItem);

            mailboxes.Update(senderMailbox);
            mailboxes.Update(recipientMailbox);
        }

        public EnumReportType GetReportType()
        {
            return EnumReportType.Private_Message;
        }

        public string GetReportableItemID()
        {
            return PrivateMessageID.ToString();
        }

        public string GetReportableItemDescription()
        {
            return "<strong>" + SenderName + "</strong>'" + (SenderName.ToLower().Last() != 's' ? "s" : "") + " private message to <strong>" + RecipientUser.FormattedDisplayName + "</strong> \"<strong>" + Summary + "</strong>\"";
        }

        public string GetReportableItemLinkHtml(string returnUrl = "/")
        {
            return "<a href=\"javascript:void(0);\" class=\"private-message-link blue hover-green\" data-private-message-id=\"" + PrivateMessageID + "\">" + GetReportableItemDescription() + "</a>";
        }
    }
}
