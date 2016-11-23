using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CodeSynergy.Models.MailboxViewModels
{
    public class ReportViewModel
    {
        public IReportable ReportedItem { get; set; }
        // This implementation of the Range attribute is taken from http://jasonwatmore.com/post/2013/10/16/aspnet-mvc-required-checkbox-with-data-annotations
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the rules in order to send a report!")]
        [Display(Name = "Rules")]
        public bool AcceptRules { get; set; }
        public string ReporterDisplayName { get; set; }
        [Required]
        public byte ReportTypeID { get; set; }
        [Required]
        public string ReportedItemID { get; set; }
        [Required, StringLength(255), ValidateLinks("report reason")]
        [Display(Name = "Report Reason")]
        public string ReportReason { get; set; }

        public string[] ReportRules
        {
            get
            {
                return new string[] { "Discrimination or hate speech against an individual or group",
                    "Threats or personal attacks on an individual or group",
                    "A malicious link",
                    "Any attempt, failed or successful, at an injection attack",
                    "A link to or an endorsement of piracy/illegal downloads",
                    "Links posted solely for advertising (spam)",
                    "A link or image containing excessively violent content",
                    "A link or image containing explicit sexual content of any kind"
                };
            }
        }
    }
}
