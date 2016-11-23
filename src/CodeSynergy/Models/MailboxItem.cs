using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeSynergy.Models
{
    public abstract class MailboxItem : ISearchable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "int")]
        public int MailboxItemID { get; set; }
        public abstract byte MailboxTypeID { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool ReadFlag { get; set; }
        public abstract Message Message { get; }
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }
        [NotMapped]
        public abstract string RowClassHtml { get; }

        public virtual string[] GetSearchText()
        {
            return new string[] { Message.Summary, Message.Contents, Message.SenderUser.DisplayName };
        }
    }
}
