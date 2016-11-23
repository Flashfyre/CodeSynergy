using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CodeSynergy.Models.MailboxViewModels
{
    public class MailboxListViewModel
    {
        [Required]
        public Mailbox Mailbox;
        public bool IsActive;
        public string ReturnUrl = null;
    }
}
