using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.UserViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel() { }

        public ProfileViewModel(ApplicationUser User)
        {
            this.User = User;
            Contents = User.ProfileMessage;
            ResizeMethod = EnumResizeMethod.Stretch;
        }

        public ApplicationUser User { get; set; }
        [Required, MaxLength(4000), ValidateLinks]
        public string Contents { get; set; }
        [Required]
        public bool IsImage { get; set; }
        public bool? IsBackgroundImage { get; set; }
        public IFormFile ProfileImage { get; set; }
        public EnumResizeMethod ResizeMethod { get; set; }
        public IFormFile BackgroundImage { get; set; }

        public enum EnumResizeMethod
        {
            Stretch,
            Crop
        }
    }
}
