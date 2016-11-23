using System.ComponentModel.DataAnnotations;

namespace CodeSynergy.Models.QuestionViewModels
{
    public class PostQuestionViewModel
    {
        [Required, MinLength(3), MaxLength(64), ValidateLinks("summary")]
        public string Summary { get; set; }
        [Required, MinLength(3), MaxLength(4000), ValidateLinks]
        public string Contents { get; set; }
        public Tag[] Tags { get; set; }
    }
}
