using System.Collections.Generic;

namespace CodeSynergy.Models.QuestionViewModels
{
    public class TagInputViewModel
    {
        public TagInputViewModel(List<Tag> Tags = null, bool IsGridFilter = false)
        {
            this.Tags = Tags != null ? Tags : new List<Tag>();
            this.IsGridFilter = IsGridFilter;
        }

        public List<Tag> Tags { get; set; }
        public bool IsGridFilter { get; set; }
    }
}
