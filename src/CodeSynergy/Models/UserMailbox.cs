using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class UserMailbox : Mailbox
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
        public ICollection<UserMailboxItem> UserItems { get; set; }
        [NotMapped]
        public override IEnumerable<MailboxItem> Items {
            get
            {
                return UserItems.AsEnumerable();
            }
        }
    }
}
