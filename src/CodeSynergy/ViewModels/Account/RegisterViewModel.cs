using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "JobTitle")]
        public string JobTitle { get; set; }

        [Display(Name = "Gender")]
        public Boolean Gender { get; set; }

        public DateTime BirthDate { get; set;  }

        [Display(Name = "Country")]
        public String Country { get; set; }

        [Display(Name = "Region")]
        public String Region { get; set; }

        [Display(Name = "City")]
        public String City { get; set; }

        [Display(Name = "GitHub ID")]
        public string GitHubID { get; set; }

        [Display(Name = "Include GitHub Link in my Profile Page")]
        public Boolean ProfileGitHub { get; set; }
    }
}
