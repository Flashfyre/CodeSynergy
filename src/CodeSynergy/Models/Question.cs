using CodeSynergy.Data;
using CodeSynergy.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Question : ISearchable, IReportable
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int QuestionID { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string LockedByUserID { get; set; }
        [ForeignKey("LockedByUserID")]
        public ApplicationUser LockedByUser { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? LockedDate { get; set; }
        public ICollection<QAPost> Posts { get; set; }
        [Column(TypeName = "int")]
        public int? BestAnswerQuestionPostID { get; set; }
        [NotMapped]
        public QAPost BestAnswer {
            get {
                if (BestAnswerQuestionPostID != null && BestAnswerQuestionPostID > Posts.Count)
                    RecalculateBestAnswer();
                return BestAnswerQuestionPostID != null ? Posts.SingleOrDefault(p => p.QuestionPostID == BestAnswerQuestionPostID) : null;
            }
        }
        [Column(TypeName = "datetime2")]
        public DateTime? SolvedDate { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int ViewCount { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public string Summary { get; set; }
        [Column(TypeName = "int")]
        public int DupeOriginalID { get; set; }
        public ICollection<QuestionTag> QuestionTags { get; set; }
        public ICollection<Star> Stars { get; set; }
        [NotMapped]
        public Question DupeOriginal {
            get
            {
                if (DupeOriginalID == 0)
                    return null;
                else
                {
                    using (var context = new ApplicationDbContext())
                    {
                        return context.Questions.Single(q => q.QuestionID == DupeOriginalID);
                    }
                }
            }
            set
            {
                DupeOriginalID = value.DupeOriginalID;
            }
        }
        [NotMapped]
        public string SummaryHtml
        {
            get
            {
                return (!QuestionPost.DeletedFlag ? Summary + (BestAnswer != null ? "&nbsp;<i class='fa fa-check'></i>" : "") : "<strong>[DELETED]</strong>") + (LockedDate == null ? "" : "&nbsp;<i class=\"fa fa-lock\"></i>");
            }
        }
        [NotMapped]
        public DateTime LastActivityDate
        {
            get
            {
                return LastActivityPost.LastActivityDate;
            }
        }
        [NotMapped]
        public string LastActivityHtml
        {
            get
            {
                Post lastActivityPost = LastActivityPost;
                string actionString = lastActivityPost.EditDate == null ? !lastActivityPost.IsComment() ? lastActivityPost.QuestionPostID == 1 ? "Question asked" : "Answer posted" : "Comment posted" : (!lastActivityPost.IsComment() ? lastActivityPost.QuestionPostID == 1 ? "Question" : "Answer" : "Comment") + " edited";
                return actionString + " <span class=\"nowrap\" title=\"" + LastActivityDate.ToLocalTime() + "\">" + FormatHelper.GetTimeSinceDate(LastActivityDate).ToLower() + "</span> <span class=\"nowrap\">by " + lastActivityPost.User.GetFullFormattedDisplayName(null, true) + "</span>"; 
            }
        }
        [NotMapped]
        public List<QAPost> VisiblePosts
        {
            get
            {
                return Posts.Where(p => (!p.DeletedFlag || p.QuestionPostID == 1 || p.VisibleComments.Any())).ToList();
            }
        }

        [NotMapped]
        public List<QAPost> UndeletedPosts
        {
            get
            {
                return Posts.Where(p => !p.DeletedFlag).ToList();
            }
        }
        [NotMapped]
        public QAPost QuestionPost
        {
            get
            {
                return Posts.Single(p => p.QuestionPostID == 1);
            }
        }
        [NotMapped]
        public Post LastActivityPost
        {
            get
            {
                List<QAPost> posts = UndeletedPosts;
                Post lastActivityPost = posts[0];
                if (posts.Count == 1)
                {
                    foreach (Comment comment in posts[0].Comments)
                    {
                        DateTime dateToCompare = comment.LastActivityDate;
                        if (dateToCompare > lastActivityPost.LastActivityDate)
                        {
                            lastActivityPost = comment;
                        }
                    }
                }
                else
                {
                    foreach (QAPost post in posts)
                    {
                        DateTime dateToCompare = post.LastActivityDate;
                        if (dateToCompare > lastActivityPost.LastActivityDate)
                        {
                            lastActivityPost = post;
                        }

                        foreach (Comment comment in post.Comments)
                        {
                            dateToCompare = post.LastActivityDate;
                            if (dateToCompare > lastActivityPost.LastActivityDate)
                            {
                                lastActivityPost = comment;
                            }
                        }
                    }
                }

                return lastActivityPost;
            }
        }
        [NotMapped]
        public int AnswerCount
        {
            get
            {
                return Math.Max(UndeletedPosts.Count - 1, 0);
            }
        }
        [NotMapped]
        public List<Tag> Tags
        {
            get
            {
                using (var context = new ApplicationDbContext())
                {
                    IEnumerable<Tag> allTags = context.Tags;
                    return allTags.Where(t => QuestionTags.Any(qt => qt.TagID == t.TagID)).ToList();
                }
            }
        }
        [NotMapped]
        public string TagsString {
            get {
                string ret = "";

                Tags.ForEach(t => ret = ret != "" ? ret + ",[" + t.TagName + "]" : "[" + t.TagName + "]");

                return ret;
            }
        }
        [NotMapped]
        public string TagsHTML
        {
            get
            {
                string ret = "";

                Tags.ForEach(t => ret += "<span class=\"tag\" data-tag-name=\"" + t.TagName + "\" data-tag-id=\"" + t.TagID + "\">" + t.FormattedTagName + "</span>");

                return ret;
            }
        }
        [NotMapped]
        public string TagsHTMLSmall
        {
            get
            {
                string ret = "";

                Tags.ForEach(t => ret += "<span class=\"tag tag-small\" data-tag-name=\"" + t.TagName + "\" data-tag-id=\"" + t.TagID + "\">" + t.FormattedTagName + "</span>");

                return ret;
            }
        }

        public string GetTimeSinceLockedDate()
        {
            return FormatHelper.GetTimeSinceDate(LockedDate);
        }

        public void AddPost(ApplicationDbContext context, ApplicationUser poster, string contents)
        {
            QAPost post = new QAPost()
            {
                QuestionID = QuestionID,
                QuestionPostID = Posts != null && Posts.Any() ? Posts.Last().QuestionPostID + 1 : 1,
                UserID = poster.Id,
                Contents = contents,
                PostDate = DateTime.Now
            };
            context.QAPosts.Add(post);
            if (post.QuestionID > 1)
                poster.AnswersPosted++;
            context.SaveChanges();
        }

        public void RemovePost(ApplicationDbContext context, QAPost post)
        {
            post.DeletedFlag = true;
            post.EditDate = DateTime.Now;
            context.QAPosts.Update(post);
            post.User.AnswersPosted--;
            context.SaveChanges();
        }

        public void AddStar(ApplicationDbContext context, ApplicationUser user)
        {
            Star star = new Star()
            {
                UserID = user.Id,
                QuestionID = QuestionID,
                StarredDate = DateTime.Now
            };
            context.Stars.Add(star);
            QuestionPost.User.StarCount++;
            context.SaveChanges();
        }

        public void RemoveStar(ApplicationDbContext context, Star star)
        {
            context.Stars.Remove(star);
            QuestionPost.User.StarCount--;
            context.SaveChanges();
        }

        public void RecalculateBestAnswer()
        {
            using (var context = new ApplicationDbContext())
            {
                QAPost bestAnswer = null;
                IEnumerable<QAPost> posts = UndeletedPosts;
                if (DateTime.Now.Subtract(posts.ElementAt(0).PostDate).Days > 0)
                {
                    IEnumerable<QAPost> maxScorePosts = posts.Where(p => p.QuestionPostID != 1 && !p.DeletedFlag && p.Score > 0 && p.Score == posts.Max(ps => ps.Score));
                    bestAnswer = maxScorePosts.Count() > 0 ? maxScorePosts.SingleOrDefault(p => p.PostDate == maxScorePosts.Min(pd => pd.PostDate)) : null;
                }

                if (bestAnswer != null)
                    BestAnswerQuestionPostID = bestAnswer.QuestionPostID;
                else if (BestAnswerQuestionPostID != null)
                    BestAnswerQuestionPostID = null;
            }
        }

        public string[] GetSearchText()
        {
            return new string[] { SummaryHtml, QuestionPost.Contents };
        }

        public EnumReportType GetReportType()
        {
            return EnumReportType.Question;
        }

        public string GetReportableItemID()
        {
            return QuestionID.ToString();
        }

        public string GetReportableItemDescription()
        {
            return "<strong>" + QuestionPost.User.FormattedDisplayName + "</strong>'" + (QuestionPost.User.FormattedDisplayName.ToLower().Last() != 's' ? "s" : "") + " question \"<strong>" + SummaryHtml + "</strong>\"";
        }

        public string GetReportableItemLinkHtml(string returnUrl = null)
        {
            return "<a href=\"/Question/" + QuestionID + "\" class=\"blue hover-green\">" + GetReportableItemDescription() + "</a>";
        }
    }
}
