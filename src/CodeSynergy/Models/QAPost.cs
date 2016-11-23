using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CodeSynergy.Models
{
    public class QAPost : Post
    {
        public ICollection<Comment> Comments { get; set; }
        [NotMapped]
        public ICollection<Comment> VisibleComments
        {
            get
            {
                ICollection<Comment> comments = Comments;
                return comments != null && comments.Any() && comments.Any(c => !c.DeletedFlag) ? comments.Where(c => !c.DeletedFlag).ToList() : new List<Comment>();
            }
        }

        public void AddComment(ApplicationDbContext context, ApplicationUser poster, string contents)
        {
            Comment comment = new Comment()
            {
                QuestionID = QuestionID,
                QuestionPostID = QuestionPostID,
                PostCommentID = (short) (Comments.Any() ? Comments.Last().PostCommentID + 1 : 1),
                UserID = poster.Id,
                Contents = contents,
                PostDate = DateTime.Now
            };
            context.Comments.Add(comment);
            poster.CommentsPosted++;
            context.SaveChanges();
        }

        public void RemoveComment(ApplicationDbContext context, Comment comment)
        {
            comment.DeletedFlag = true;
            comment.EditDate = DateTime.Now;
            context.Comments.Update(comment);
            comment.User.CommentsPosted--;
            context.SaveChanges();
        }

        public override PostVote GetVote(ApplicationDbContext context, string username)
        {
            return context.QAPostVotes.SingleOrDefault(p => p.User.Email == username && p.QuestionID == QuestionID && p.QuestionPostID == QuestionPostID);
        }

        public override void AddVote(ApplicationDbContext context, ApplicationUser voter, bool vote)
        {
            QAPostVote postVote = new QAPostVote()
            {
                QuestionID = QuestionID,
                QuestionPostID = QuestionPostID,
                UserID = voter.Id,
                Vote = vote
            };
            int scoreChange = vote ? 1 : -1;
            context.QAPostVotes.Add(postVote);
            Score += (short) scoreChange;
            if (QuestionPostID == 1)
                User.QuestionScore += scoreChange;
            else
                User.AnswerScore += scoreChange;
            if (QuestionPostID != 1 && Question.BestAnswerQuestionPostID != null && ((Question.BestAnswerQuestionPostID != QuestionPostID && vote) || (Question.BestAnswerQuestionPostID == QuestionPostID && !vote)))
                Question.RecalculateBestAnswer();
            context.QAPosts.Update(this);
            context.SaveChanges();
        }

        public override void UpdateVote(ApplicationDbContext context, PostVote postVote)
        {
            int scoreChange = postVote.Vote ? 2 : -2;
            context.QAPostVotes.Update((QAPostVote) postVote);
            Score += (short) scoreChange;
            context.QAPosts.Update(this);
            if (QuestionPostID == 1)
                User.QuestionScore += scoreChange;
            else
                User.AnswerScore += scoreChange;
            if (QuestionPostID != 1 && Question.BestAnswerQuestionPostID != null && ((Question.BestAnswerQuestionPostID != QuestionPostID && postVote.Vote) || (Question.BestAnswerQuestionPostID == QuestionPostID && !postVote.Vote)))
                Question.RecalculateBestAnswer();
            User.AnswerScore++;
            context.SaveChanges();
        }

        public override void RemoveVote(ApplicationDbContext context, PostVote postVote)
        {
            int scoreChange = postVote.Vote ? -1 : 1;
            Score += (short) scoreChange;
            if (QuestionPostID == 1)
                User.QuestionScore += scoreChange;
            else
                User.AnswerScore += scoreChange;
            if (QuestionPostID != 1 && Question.BestAnswerQuestionPostID != null && ((Question.BestAnswerQuestionPostID != QuestionPostID && !postVote.Vote) || (Question.BestAnswerQuestionPostID == QuestionPostID && postVote.Vote)))
                    Question.RecalculateBestAnswer();
            context.QAPostVotes.Remove((QAPostVote) postVote);
            context.QAPosts.Update(this);
            context.SaveChanges();
        }

        public override bool IsComment()
        {
            return false;
        }
    }
}
