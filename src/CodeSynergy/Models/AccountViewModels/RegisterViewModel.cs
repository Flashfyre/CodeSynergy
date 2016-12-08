using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required, Remote("ValidateUniqueEmail", "Account"), UniqueEmail, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required, Remote("ValidateUniqueDisplayName", "Account"), UniqueDisplayName, StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Required, StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
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
        public String CountryID { get; set; }
        [Display(Name = "Region")]
        public String RegionID { get; set; }
        [Display(Name = "City")]
        public String City { get; set; }
        [Display(Name = "GitHub ID")]
        public string GitHubID { get; set; }
        [Display(Name = "Include GitHub Link in my Profile Page")]
        public Boolean ProfileGitHub { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueEmailAttribute : ValidationAttribute, IClientModelValidator
    {
        UserRepository users;
        public UniqueEmailAttribute() : base()
        {
            this.ErrorMessage = string.Format("That email is already in use. Please enter an email address that isn't already connected with a CodeSynergy account.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            users = new UserRepository(new ApplicationDbContext(), null);
            return IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        public override bool IsValid(object value)
        {
            string email = value as string;
            bool isValid = true;

            isValid = users.FindByEmail(email) == null;

            return isValid;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-uniqueemail", ErrorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueDisplayNameAttribute : ValidationAttribute, IClientModelValidator
    {
        UserRepository users;
        public UniqueDisplayNameAttribute() : base()
        {
            this.ErrorMessage = string.Format("That display name is already in use. Please enter a unique display name.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            users = new UserRepository(new ApplicationDbContext(), null);
            bool isNewDisplayName;
            if (validationContext.ObjectInstance is SettingsViewModel)
                isNewDisplayName = users.FindByEmail((validationContext.ObjectInstance as SettingsViewModel).Email).DisplayName != ((string)value).Replace(' ', '_');
            else
                isNewDisplayName = true;
            return !isNewDisplayName || IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        public override bool IsValid(object value)
        {
            string displayName = (value as string).Replace(' ', '_');
            bool isValid = true;

            isValid = users.FindByDisplayName(displayName) == null;

            return isValid;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-uniquedisplayname", ErrorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
