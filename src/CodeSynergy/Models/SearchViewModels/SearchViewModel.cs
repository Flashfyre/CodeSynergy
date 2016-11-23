using System;
using System.ComponentModel.DataAnnotations;

namespace CodeSynergy.Models.SearchViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel() { }

        public SearchViewModel(String SearchType, String Query, String RowsPerPage)
        {
            if (SearchType != null)
            {
                switch (SearchType)
                {
                    case "Questions":
                        this.SearchType = EnumSearchType.Question;
                        break;
                    case "Posts":
                        this.SearchType = EnumSearchType.Post;
                        break;
                    case "Answers":
                        this.SearchType = EnumSearchType.Answer;
                        break;
                    case "Tags":
                        this.SearchType = EnumSearchType.Tag;
                        break;
                    case "Users":
                        this.SearchType = EnumSearchType.User;
                        break;
                    default:
                        break;
                }

                if (Query != null)
                    this.Query = Query;

                if (RowsPerPage != null)
                {
                    int rowsPerPage;
                    if (int.TryParse(RowsPerPage, out rowsPerPage))
                        this.RowsPerPage = rowsPerPage;
                }
            }
        }

        public string Query { get; set; }
        [NotNull]
        public EnumSearchType SearchType { get; set; }
        [Required]
        public int RowsPerPage { get; set; } = (int) EnumRowsPerPage._25;
        
        public enum EnumSearchType
        {
            Null = 0,
            Question = 1,
            Post = 2,
            Answer = 3,
            Tag = 4,
            User = 5
        } 

        public enum EnumRowsPerPage
        {
            _10 = 10,
            _25 = 25,
            _50 = 50,
            _75 = 75,
            _100 = 100,
            _150 = 150,
            _200 = 200,
            _250 = 250
        }
    }

    public class NotNullAttribute : ValidationAttribute
    {
        public NotNullAttribute() : base()
        {
            this.ErrorMessage = string.Format("Please select a search type.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        public override bool IsValid(object value)
        {
            return (int) value != (int) SearchViewModel.EnumSearchType.Null;
        }
    }
}
