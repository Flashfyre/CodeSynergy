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
    public class Comment : Post
    {
        [ForeignKey("QuestionID,QuestionPostID")]
        public QAPost Post { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "smallint")]
        public short PostCommentID { get; set; }
        
        public override PostVote GetVote(ApplicationDbContext context, string username)
        {
            return context.CommentVotes.SingleOrDefault(c => c.User.Email == username && c.QuestionID == QuestionID && c.QuestionPostID == QuestionPostID && c.PostCommentID == PostCommentID);
        }

        public override void AddVote(ApplicationDbContext context, ApplicationUser voter, bool vote)
        {
            CommentVote CommentVote = new CommentVote()
            {
                QuestionID = QuestionID,
                QuestionPostID = QuestionPostID,
                PostCommentID = PostCommentID,
                UserID = voter.Id,
                Vote = vote
            };
            int scoreChange = vote ? 1 : -1;
            context.CommentVotes.Add(CommentVote);
            Score += (short) scoreChange;
            User.CommentScore += scoreChange;
            context.Comments.Update(this);
            context.SaveChanges();
        }

        public override void UpdateVote(ApplicationDbContext context, PostVote CommentVote)
        {
            int scoreChange = CommentVote.Vote ? 2 : -2;
            context.CommentVotes.Update((CommentVote) CommentVote);
            Score += (short) scoreChange;
            User.CommentScore += scoreChange;
            context.Comments.Update(this);
            context.SaveChanges();
        }

        public override void RemoveVote(ApplicationDbContext context, PostVote CommentVote)
        {
            int scoreChange = CommentVote.Vote ? -1 : 1;
            Score += (short) scoreChange;
            User.CommentScore += scoreChange;
            context.CommentVotes.Remove((CommentVote) CommentVote);
            context.Comments.Update(this);
            context.SaveChanges();
        }

        public override bool IsComment()
        {
            return true;
        }

        public override EnumReportType GetReportType()
        {
            return EnumReportType.Comment;
        }

        public override string GetReportableItemID()
        {
            return QuestionID + "-" + QuestionPostID + "-" + PostCommentID;
        }

        public override string GetReportableItemDescription()
        {
            return "<strong>" + User.FormattedDisplayName + "</strong>'" + (User.FormattedDisplayName.ToLower().Last() != 's' ? "s" : "") + " comment on " + base.GetReportableItemDescription();
        }

        public override string GetReportableItemURL()
        {
            return base.GetReportableItemURL() + "c" + PostCommentID;
        }
    }
}
