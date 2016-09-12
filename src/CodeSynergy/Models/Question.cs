using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Question
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionID { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime PostDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? EditDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? SolvedDate { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int ViewCount { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public string Summary { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool LockedFlag { get; set; }
        [Column(TypeName = "int")]
        public int? DupeOriginalID { get; set; }
        [NotMapped]
        public List<Post> Posts
        {
            get
            {
                using (var context = new ApplicationDbContext())
                {
                    return context.Posts.Where(p => p.QuestionID == QuestionID).ToList();
                }
            }
        }

        [NotMapped]
        public Post QuestionPost {
            get
            {
                return Posts[0];
            }
        }
    }
}
