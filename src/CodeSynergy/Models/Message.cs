using CodeSynergy.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Message
    {
        [Column(TypeName = "nvarchar(450)")]
        public string SenderUserID { get; set; }
        [ForeignKey("SenderUserID")]
        public ApplicationUser SenderUser { get; set; }
        public virtual string Summary { get; set; }
        public virtual string Contents { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime SentDate { get; set; }
        [NotMapped]
        public string SenderName
        {
            get
            {
                return SenderUserID != null ? SenderUser.FormattedDisplayName : "Guest";
            }
        }

        public string GetTimeSinceSentDate()
        {
            return FormatHelper.GetTimeSinceDate(SentDate);
        }
    }
}
