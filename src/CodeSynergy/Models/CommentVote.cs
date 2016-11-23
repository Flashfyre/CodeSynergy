using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class CommentVote : PostVote
    {
        [Key]
        [Required]
        [Column(TypeName = "smallint")]
        public short PostCommentID { get; set; }
        [ForeignKey("QuestionID,QuestionPostID,PostCommentID")]
        public Comment Comment { get; set; }
    }
}
