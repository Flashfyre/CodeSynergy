using CodeSynergy.Data;
using CodeSynergy.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public abstract class Post : ISearchable, IReportable
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionID { get; set; }
        [ForeignKey("QuestionID")]
        public Question Question { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionPostID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        [Required]
        [Column(TypeName = "smallint")]
        public short Score { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public string Contents { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime PostDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? EditDate { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool DeletedFlag { get; set; }
        [NotMapped]
        public DateTime LastActivityDate
        {
            get
            {
                return EditDate == null ? PostDate : (DateTime) EditDate;
            }
        }
        [NotMapped]
        public string LastActivityHtml
        {
            get
            {
                string actionString = EditDate == null ? !IsComment() ? QuestionPostID == 1 ? "asked" : "posted" : "posted" : "edited";
                return actionString + " <span class=\"nowrap\" title=\"" + LastActivityDate.ToLocalTime() + "\">" + GetTimeSinceLastActivityDate() + "</span>";
            }
        }
        [NotMapped]
        public string PostType
        {
            get
            {
                return !IsComment() ? QuestionPostID == 1 ? "Question" : "Answer" : "Comment";
            }
        }

        public string GetTimeSincePostDate()
        {
            return FormatHelper.GetTimeSinceDate(PostDate);
        }

        public string GetTimeSinceEditDate()
        {
            return FormatHelper.GetTimeSinceDate(EditDate);
        }

        public string GetTimeSinceLastActivityDate()
        {
            return FormatHelper.GetTimeSinceDate(LastActivityDate);
        }

        public abstract PostVote GetVote(ApplicationDbContext context, string username);

        public abstract void AddVote(ApplicationDbContext context, ApplicationUser voter, bool vote);

        public abstract void UpdateVote(ApplicationDbContext context, PostVote postVote);

        public abstract void RemoveVote(ApplicationDbContext context, PostVote postVote);

        public abstract bool IsComment();

        public string[] GetSearchText()
        {
            return new string[] { Contents };
        }

        public virtual EnumReportType GetReportType()
        {
            return EnumReportType.Answer;
        }

        public virtual string GetReportableItemID()
        {
            return QuestionID + "-" + QuestionPostID;
        }

        public virtual string GetReportableItemDescription()
        {
            return "<strong>" + User.FormattedDisplayName + "</strong>'" + (User.FormattedDisplayName.ToLower().Last() != 's' ? "s" : "") + " answer on " + Question.GetReportableItemDescription();
        }

        public string GetReportableItemLinkHtml(string returnUrl = null)
        {
            return "<a href=\"" + GetReportableItemURL() + "\" class=\"blue hover-green\">" + GetReportableItemDescription() + "</a>";
        }

        public virtual string GetReportableItemURL()
        {
            return "/Question/" + QuestionID + "#p" + QuestionPostID;
        }
    }
}