using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace CodeSynergy.Models.QuestionViewModels
{
    public class PostAnswerViewModel
    {
        public PostAnswerViewModel() { }

        public PostAnswerViewModel(Question Question)
        {
            this.Question = Question;
        }

        [Required]
        public int QuestionID { get; set; }
        public int? DupeOriginalID { get; set; }
        public string LockedByDisplayName { get; set; }
        public int? QuestionPostID { get; set; }
        public short? PostCommentID { get; set; }
        public bool IsComment { get; set; } 
        public Question Question { get; set; }
        [Required, MinLength(3), MaxLength(4000), ValidateLinks]
        public string Contents { get; set; }
    }

    public class ValidateLinksAttribute : ValidationAttribute
    {
        bool IsTextArea = false;

        public ValidateLinksAttribute(string fieldName = "post") : base()
        {
            ErrorMessage = string.Format("Your {0} contains a link or embedded image that references an untrusted URL. Please remove the suspicious link or image.", fieldName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        public override bool IsValid(object value)
        {
            string content = WebUtility.HtmlDecode(value as string).ToLower();
            List<string> contentATags = new List<string>();
            List<string> contentImgTags = new List<string>();
            bool isValid = true;
            UntrustedURLPatternRepository untrustedURLPatterns = new UntrustedURLPatternRepository(new ApplicationDbContext());

            int strPos = 0;

            if (value == null)
                return true;

            while ((strPos = content.IndexOf("<a", strPos)) > -1 && content.IndexOf("</a>", strPos) > -1)
            {
                int startPos = strPos;
                contentATags.Add(content.Substring(startPos, (strPos = content.IndexOf("</a>", startPos) + 4) - startPos));
            }

            foreach (UntrustedURLPattern pattern in untrustedURLPatterns.GetAll())
            {
                string patternString = pattern.PatternText;
                RegexOptions options = RegexOptions.None;
                if (patternString[patternString.Length - 1] != '/')
                {
                    switch (patternString[patternString.Length - 1])
                    {
                        case 'i':
                            options = RegexOptions.IgnoreCase;
                            break;
                    }
                    patternString = patternString.Substring(1, patternString.Length - 3);
                }
                else
                {
                    patternString = patternString.Substring(1, patternString.Length - 2);
                }

                if (new Regex(patternString, options).IsMatch(content))
                {
                    foreach (string tag in contentATags)
                    {
                        if (new Regex(patternString, options).IsMatch(tag))
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        break;
                    }
                }
            }

            if (isValid)
            {
                strPos = 0;

                while ((strPos = content.IndexOf("<img", strPos)) > -1)
                {
                    int startPos = strPos;
                    contentImgTags.Add(content.Substring(startPos, (strPos = content.IndexOf("/>", startPos) + 2) - startPos));
                }

                foreach (UntrustedURLPattern pattern in untrustedURLPatterns.GetAll())
                {
                    string patternString = pattern.PatternText;
                    RegexOptions options = RegexOptions.None;
                    if (patternString[patternString.Length - 1] != '/')
                    {
                        switch (patternString[patternString.Length - 1])
                        {
                            case 'i':
                                options = RegexOptions.IgnoreCase;
                                break;
                        }
                        patternString = patternString.Substring(1, patternString.Length - 3);
                    }
                    else
                    {
                        patternString = patternString.Substring(1, patternString.Length - 2);
                    }

                    if (new Regex(patternString, options).IsMatch(content))
                    {
                        foreach (string tag in contentImgTags)
                        {
                            if (new Regex(patternString, options).IsMatch(tag))
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if (!isValid)
                        {
                            break;
                        }
                    }
                }
            }

            return isValid;
        }
    }
}
