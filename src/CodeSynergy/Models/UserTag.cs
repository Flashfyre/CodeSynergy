using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class UserTag
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
        public int TagID { get; set; }
        [ForeignKey("TagID")]
        public Tag Tag { get; set; }
        [Column(TypeName = "int")]
        public int QuestionScore { get; set; }
        [Column(TypeName = "int")]
        public int AnswerScore { get; set; }
        [Column(TypeName = "int")]
        public int CommentScore { get; set; }
        [Column(TypeName = "int")]
        public int BestAnswerCount { get; set; }
        [Column(TypeName = "int")]
        public int QuestionStarCount { get; set; }
        [Column(TypeName = "int")]
        public int QuestionCount { get; set; }
        [Column(TypeName = "int")]
        public int AnswerCount { get; set; }
        [Column(TypeName = "int")]
        public int CommentCount { get; set; }
        [NotMapped]
        public int TotalScore
        {
            get
            {
                return QuestionScore + AnswerScore + CommentScore;
            }
        }
        [NotMapped]
        public int TotalPostCount {
            get
            {
                return QuestionCount + AnswerCount + CommentCount;
            }
        }
    }
}
