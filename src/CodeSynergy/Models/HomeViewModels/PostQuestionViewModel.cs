using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.HomeViewModels
{
    public class PostQuestionViewModel
    {
        [Required]
        [MinLength(3)]
        public string Summary { get; set; }

        [Required]
        [MinLength(3)]
        public string Content { get; set; }
        
        public Tag[] Tags { get; set; }
    }
}
