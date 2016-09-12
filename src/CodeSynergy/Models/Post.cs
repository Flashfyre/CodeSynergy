using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Post
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionID { get; set; }
        [ForeignKey("QuestionID")]
        public Question Question { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionPostID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        [Required]
        [Column(TypeName = "smallint")]
        public short Score { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Contents { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime PostDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? EditDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? SolvedDate { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool DeletedFlag { get; set; }
    }
}
