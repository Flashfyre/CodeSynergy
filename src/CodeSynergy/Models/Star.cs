using CodeSynergy.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Star
    {
        [Key]
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionID { get; set; }
        [ForeignKey("QuestionID")]
        public Question Question { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime StarredDate { get; set; }

        public string GetTimeSinceStarredDate()
        {
            return FormatHelper.GetTimeSinceDate(StarredDate);
        }
    }
}
