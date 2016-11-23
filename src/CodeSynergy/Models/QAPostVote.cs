using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class QAPostVote : PostVote
    {
        [ForeignKey("QuestionID,QuestionPostID")]
        public QAPost Post { get; set; }
    }
}
