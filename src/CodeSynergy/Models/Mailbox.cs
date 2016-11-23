using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public abstract class Mailbox
    {
        public virtual byte MailboxTypeID { get; set; }
        public virtual IEnumerable<MailboxItem> Items { get; set; }
    }

    public enum EnumMailboxType : byte
    {
        Inbox,
        Starred,
        Sent,
        Spam,
        Deleted,
        Moderation
    }

    public enum EnumUserMailboxType : byte
    {
        Inbox,
        Starred,
        Sent,
        Spam,
        Deleted
    }

    public static class MailboxTypeHelper
    {
        public static string DisplayName(this EnumMailboxType mailboxType)
        {
            return Enum.GetName(typeof(EnumMailboxType), mailboxType);
        }
    }
}
