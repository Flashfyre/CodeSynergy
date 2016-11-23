using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.MailboxViewModels
{
    public class PrivateMessageViewModel : MessageViewModel
    {
        [Required, StringLength(20), Remote("ValidateExistingDisplayName", "Account")]
        [Display(Name = "To")]
        public string DisplayName { get; set; }
        [StringLength(64), ValidateLinks("subject")]
        [Display(Name = "Subject")]
        public string Summary { get; set; }
        [Required, StringLength(4000), ValidateLinks("message body")]
        [Display(Name = "Body")]
        public string Contents { get; set; }
    }
}
