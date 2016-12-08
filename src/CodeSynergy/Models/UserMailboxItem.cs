using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class UserMailboxItem : MailboxItem
    {
        [Key]
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "tinyint")]
        public override byte MailboxTypeID { get; set; }
        [ForeignKey("UserID, MailboxTypeID")]
        public UserMailbox Mailbox { get; set; }
        [Required]
        [Column(TypeName = "bigint")]
        public long PrivateMessageID { get; set; }
        [ForeignKey("PrivateMessageID")]
        public PrivateMessage PrivateMessage { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? StarredDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? MarkedAsSpamDate { get; set; }
        [NotMapped]
        public override Message Message { get
            {
                return PrivateMessage;
            }
        }
        [NotMapped]
        public override string RowClassHtml
        {
            get
            {
                return "mailbox-item-row" + (ReadFlag ? "" : " mailbox-item-row-unread") + (StarredDate == null ? "" : " mailbox-item-row-starred")
                    + ((MarkedAsSpamDate == null && DeletedDate == null) || MailboxTypeID == (byte) EnumMailboxType.Spam || MailboxTypeID == (byte) EnumMailboxType.Deleted ? "" : " hidden");
            }
        }

        public int Star(MailboxRepository mailboxes)
        {
            if (MailboxTypeID == (byte) EnumMailboxType.Inbox || MailboxTypeID == (byte) EnumMailboxType.Sent)
            {
                UserMailbox inbox = mailboxes.Find(UserID, (byte) EnumMailboxType.Inbox);
                UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
                UserMailbox starred = mailboxes.Find(UserID, (byte) EnumMailboxType.Starred);

                UserMailboxItem inboxItem = PrivateMessage.RecipientUserID == UserID ? inbox.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID) : null;
                UserMailboxItem sentItem = PrivateMessage.SenderUserID == UserID ? sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID) : null;

                DateTime starredDate = DateTime.Now;

                UserMailboxItem starredItem = new UserMailboxItem()
                {
                    MailboxItemID = (starred.Items.Any() ? starred.Items.Max(i => i.MailboxItemID) : 0) + 1,
                    UserID = UserID,
                    MailboxTypeID = (byte) EnumMailboxType.Starred,
                    PrivateMessageID = PrivateMessageID,
                    ReadFlag = ReadFlag,
                    StarredDate = starredDate
                };

                starred.UserItems.Add(starredItem);

                if (inboxItem != null)
                {
                    inboxItem.StarredDate = starredDate;
                    inboxItem.MarkedAsSpamDate = null;
                    mailboxes.Update(inbox);
                }
                if (sentItem != null)
                {
                    sentItem.StarredDate = starredDate;
                    sentItem.MarkedAsSpamDate = null;
                    mailboxes.Update(sent);
                }

                mailboxes.Update(starred);

                return starredItem.MailboxItemID; // Return the MailboxItemID of the new starred item
            }

            return 0;
        }

        public void Unstar(MailboxRepository mailboxes)
        {
            UserMailbox inbox = mailboxes.Find(UserID, (byte) EnumMailboxType.Inbox);
            UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
            UserMailbox starred = mailboxes.Find(UserID, (byte) EnumMailboxType.Starred);

            starred.UserItems.Remove(starred.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID));

            if (PrivateMessage.RecipientUserID == UserID)
            {
                inbox.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).StarredDate = null;
                mailboxes.Update(inbox);
            }
            if (PrivateMessage.SenderUserID == UserID)
            {
                sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).StarredDate = null;
                mailboxes.Update(sent);
            }

            mailboxes.Update(starred);
        }

        public void MarkAsRead(MailboxRepository mailboxes, bool markAsUnread)
        {
            UserMailbox[] matchingMailboxes = mailboxes.GetAllForUser(UserID).Where(m => m.UserItems.Any(i => i.PrivateMessageID == PrivateMessageID)).ToArray();

            for (int m = 0; m < matchingMailboxes.Length; m++)
            {
                matchingMailboxes[m].UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).ReadFlag = !markAsUnread;
                mailboxes.Update(matchingMailboxes[m]);
            }
        }

        public int MarkAsSpam(MailboxRepository mailboxes)
        {
            if (MailboxTypeID == (byte) EnumMailboxType.Inbox)
            {
                UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
                UserMailbox spam = mailboxes.Find(UserID, (byte) EnumMailboxType.Spam);

                if (StarredDate != null)
                    Unstar(mailboxes);

                MarkedAsSpamDate = DateTime.Now;

                UserMailboxItem spamItem = new UserMailboxItem()
                {
                    MailboxItemID = (spam.Items.Any() ? spam.Items.Max(i => i.MailboxItemID) : 0) + 1,
                    UserID = UserID,
                    MailboxTypeID = (byte) EnumMailboxType.Spam,
                    PrivateMessageID = PrivateMessageID,
                    ReadFlag = ReadFlag,
                    MarkedAsSpamDate = MarkedAsSpamDate
                };

                spam.UserItems.Add(spamItem);

                if (PrivateMessage.SenderUserID == UserID)
                {
                    sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).MarkedAsSpamDate = MarkedAsSpamDate;
                    mailboxes.Update(sent);
                }

                mailboxes.Update(Mailbox);
                mailboxes.Update(spam);

                return spamItem.MailboxItemID; // Return the MailboxItemID of the new spam item
            }

            return 0;
        }

        public void RemoveFromSpam(MailboxRepository mailboxes)
        {
            if (MailboxTypeID == (byte) EnumMailboxType.Spam)
            {
                UserMailbox inbox = mailboxes.Find(UserID, (byte) EnumMailboxType.Inbox);
                UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
                UserMailbox spam = Mailbox;

                spam.UserItems.Remove(this);
                
                inbox.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).MarkedAsSpamDate = null;

                if (PrivateMessage.SenderUserID == UserID)
                {
                    sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).MarkedAsSpamDate = null;
                    mailboxes.Update(sent);
                }

                mailboxes.Update(inbox);
                mailboxes.Update(spam);
            }
        }

        public int Delete(MailboxRepository mailboxes)
        {
            if (MailboxTypeID == (byte) EnumMailboxType.Inbox || MailboxTypeID == (byte) EnumMailboxType.Sent || MailboxTypeID == (byte) EnumMailboxType.Spam)
            {
                UserMailbox inbox = mailboxes.Find(UserID, (byte) EnumMailboxType.Inbox);
                UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
                UserMailbox spam = mailboxes.Find(UserID, (byte) EnumMailboxType.Spam);
                UserMailbox deleted = mailboxes.Find(UserID, (byte) EnumMailboxType.Deleted);

                UserMailboxItem inboxItem = PrivateMessage.RecipientUserID == UserID ? inbox.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID) : null;
                UserMailboxItem sentItem = PrivateMessage.SenderUserID == UserID ? sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID) : null;
                UserMailboxItem spamItem = MarkedAsSpamDate != null ? spam.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID) : null;

                DateTime deletedDate = DateTime.Now;

                if (StarredDate != null)
                    Unstar(mailboxes);

                UserMailboxItem deletedItem = new UserMailboxItem()
                {
                    MailboxItemID = (deleted.Items.Any() ? deleted.Items.Max(i => i.MailboxItemID) : 0) + 1,
                    UserID = UserID,
                    MailboxTypeID = (byte) EnumMailboxType.Deleted,
                    PrivateMessageID = PrivateMessageID,
                    ReadFlag = ReadFlag,
                    DeletedDate = DeletedDate
                };

                deleted.UserItems.Add(deletedItem);

                if (inboxItem != null)
                {
                    inboxItem.DeletedDate = deletedDate;
                    inboxItem.MarkedAsSpamDate = null;
                    mailboxes.Update(inbox);
                }
                if (sentItem != null)
                {
                    sentItem.DeletedDate = deletedDate;
                    sentItem.MarkedAsSpamDate = null;
                    mailboxes.Update(sent);
                }
                if (spamItem != null)
                {
                    spam.UserItems.Remove(this);
                    mailboxes.Update(spam);
                }

                mailboxes.Update(deleted);

                return deletedItem.MailboxItemID; // Return the MailboxItemID of the new deleted item
            }

            return 0;
        }

        public void Undelete(MailboxRepository mailboxes)
        {
            if (MailboxTypeID == (byte) EnumMailboxType.Deleted)
            {
                UserMailbox inbox = mailboxes.Find(UserID, (byte) EnumMailboxType.Inbox);
                UserMailbox sent = mailboxes.Find(UserID, (byte) EnumMailboxType.Sent);
                UserMailbox deleted = Mailbox;

                deleted.UserItems.Remove(this);

                if (PrivateMessage.RecipientUserID == UserID)
                {
                    inbox.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).DeletedDate = null;
                    mailboxes.Update(inbox);
                }
                if (PrivateMessage.SenderUserID == UserID)
                {
                    sent.UserItems.SingleOrDefault(i => i.PrivateMessageID == PrivateMessageID).DeletedDate = null;
                    mailboxes.Update(sent);
                }

                mailboxes.Update(deleted);
            }
        }
    }
}
