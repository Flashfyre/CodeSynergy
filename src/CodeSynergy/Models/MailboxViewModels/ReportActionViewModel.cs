using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CodeSynergy.Models.MailboxViewModels
{
    public class ReportActionViewModel : MessageViewModel {
        [Required, StringLength(20), Remote("ValidateExistingDisplayName", "Account")]
        [Display(Name = "Assignee")]
        public string AssigneeDisplayName { get; set; }
        [Required, StringLength(255), ValidateLinks("report action")]
        [Display(Name = "Report Action")]
        public string Contents { get; set; }
    }
}
