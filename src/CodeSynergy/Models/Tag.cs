using CodeSynergy.Data;
using CodeSynergy.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Tag : ISearchable
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int TagID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string TagName { get; set; }
        [NotMapped]
        public string FormattedTagName {
            get
            {
                return TagName.Replace('_', ' ');
            }
        }
        [NotMapped]
        List<Question> TaggedQuestions
        {
            get
            {
                using (var context = new ApplicationDbContext())
                {
                    return context.QuestionTags.Where(qt => qt.TagID == TagID).Select(qt => qt.Question).ToList();
                }
            }
        }
        [NotMapped]
        public int TaggedQuestionsCount
        {
            get
            {
                using (var context = new ApplicationDbContext())
                {
                    return context.QuestionTags.Where(qt => qt.TagID == TagID).Count();
                }
            }
        }

        public string[] GetSearchText()
        {
            return new string[] { FormattedTagName };
        }
    }
}
