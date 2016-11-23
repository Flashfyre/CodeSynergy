using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.AccountViewModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel() { }

        public SettingsViewModel(ApplicationUser user)
        {
            Email = user.Email;
            DisplayName = user.FormattedDisplayName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            JobTitle = user.JobTitle;
            Gender = user.Gender;
            BirthDate = user.BirthDate;
            CountryID = user.CountryID;
            RegionID = user.RegionID;
            City = user.City;
            GitHubID = user.GitHubID;
            ProfileGitHub = user.ProfileGitHub;
            ExcludeFromRanking = user.ExcludeFromRanking;
            UseProfileBackground = user.UseProfileBackground;
            UseSearchGrid = user.UseSearchGrid;
        }
        
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required, Remote("ValidateUniqueDisplayName", "Account"), UniqueDisplayName, StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Required, StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Gender")]
        public bool? Gender { get; set; }
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Country")]
        public string CountryID { get; set; }
        [Display(Name = "Region")]
        public string RegionID { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "GitHub ID")]
        public string GitHubID { get; set; }
        [Display(Name = "Include GitHub link on my profile page")]
        public bool ProfileGitHub { get; set; }
        [Display(Name = "Exclude me from the user ranking (changes may be delayed)")]
        public bool ExcludeFromRanking { get; set; }
        [Display(Name = "Use my profile background throughout the site")]
        public bool UseProfileBackground { get; set; }
        [Display(Name = "Use filterable/sortable question grids (not recommended for mobile devices)")]
        public bool UseSearchGrid { get; set; }
    }
}
