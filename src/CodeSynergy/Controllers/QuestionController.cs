using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using CodeSynergy.Models.QuestionViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeSynergy.Controllers
{
    public class QuestionController : Controller
    {
        private ApplicationDbContext _context; // Database context
        private readonly UserRepository _users; // Repository for users
        private readonly QuestionRepository _questions; // Repository for questions
        private readonly TagRepository _tags; // Repository for tags
        private readonly QuestionTagRepository _questionTags; // Repository for question tags

        public QuestionController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IRepository<Question, int> questions, IRepository<Tag, int> tags, IJoinTableRepository<QuestionTag, int, int> questionTags) : base()
        {
            _context = new ApplicationDbContext();
            _users = (UserRepository) users;
            _questions = (QuestionRepository) questions;
            _tags = (TagRepository) tags;
            _questionTags = (QuestionTagRepository) questionTags;
        }

        // Question page loaded
        // GET: /Question/
        [HttpGet]
        public IActionResult Index(int Id, string Modal)
        {
            Question question = _questions.Find(Id); // Question for a given ID

            if (question != null) // Question is valid
            {
                // Add votes for all the question's posts
                ViewBag.PostVotes = _context.QAPostVotes.Include(v => v.User).Where(v => v.User.Email == Request.HttpContext.User.Identity.Name && v.QuestionID == Id).ToDictionary(v => v.QuestionPostID);
                Dictionary<int, Dictionary<short, CommentVote>> commentVotes = new Dictionary<int, Dictionary<short, CommentVote>>();
                Dictionary<string, RepVote> repVotes = _context.RepVotes.Include(v => v.VoterUser)
                    .Where(v => v.VoterUser.Email == Request.HttpContext.User.Identity.Name && question.VisiblePosts.Any(p => p.UserID == v.VoteeUserID)).ToDictionary(v => v.VoteeUserID);
                if (_context.Comments.Any()) // If there are any comments, add comment votes for each comment on each post
                {
                    foreach (QAPost post in question.VisiblePosts)
                    {
                        Dictionary<short, CommentVote> postCommentVotes = new Dictionary<short, CommentVote>();
                        if (post.VisibleComments.Any())
                        {
                            postCommentVotes = _context.CommentVotes.Where(v => v.User.Email == Request.HttpContext.User.Identity.Name && v.QuestionID == Id && v.QuestionPostID == post.QuestionPostID).ToDictionary(v => v.PostCommentID);
                        }
                        commentVotes.Add(post.QuestionPostID, postCommentVotes);
                    }
                }
                ViewBag.CommentVotes = commentVotes;
                ViewBag.RepVotes = repVotes;
                IEnumerable<Question> similarQuestions = _questions.GetAll().Where(q => q.DupeOriginalID == Id && !q.QuestionPost.DeletedFlag).OrderBy(q => q.LastActivityDate);
                ViewBag.SimilarQuestionsTotal = similarQuestions.Count();
                ViewBag.SimilarQuestions = similarQuestions.Take(10).ToList();
                IEnumerable<Question> relatedQuestions = _questions.GetAll().Where(q => q.QuestionID != Id && q.DupeOriginalID != Id && !q.QuestionPost.DeletedFlag && q.TagsString.Split(',').Intersect(question.TagsString.Split(',')).Count() >= 1).OrderBy(q => q.LastActivityDate).OrderBy(q => q.TagsString.Split(',').Intersect(question.TagsString.Split(',')).Count());
                ViewBag.RelatedQuestionsTotal = relatedQuestions.Count();
                ViewBag.RelatedQuestions = relatedQuestions.Take(10).ToList();

                ViewData["Modal"] = Modal;

                // If the user is not the question poster, increment the question view count
                if (question.QuestionPost.User.Email != Request.HttpContext.User.Identity.Name)
                {
                    question.ViewCount++;
                    _questions.Update(question);
                }

                // If the recently viewed list doesn't yet exists in cookies, add it with this question
                if (!Request.Cookies.Any(c => c.Key == "recently_viewed"))
                {
                    Response.Cookies.Append("recently_viewed", JsonConvert.SerializeObject(new int[] { question.QuestionID }));
                }
                else // If the recently viewed list exists in cookies, add this question to the list
                {
                    int[] questionIds = JsonConvert.DeserializeObject<int[]>(Request.Cookies["recently_viewed"]);
                    if (questionIds.Contains(question.QuestionID))
                    {
                        List<int> reorderedIds = new List<int>() { question.QuestionID };
                        foreach (int id in questionIds)
                        {
                            if (id != question.QuestionID)
                                reorderedIds.Add(id);
                        }
                        questionIds = reorderedIds.Take(10).ToArray();
                    }
                    else
                        questionIds = questionIds.Prepend(question.QuestionID).Take(10).ToArray();

                    Response.Cookies.Append("recently_viewed", JsonConvert.SerializeObject(questionIds));
                }

                return View(new PostAnswerViewModel(question));
            } else
            {
                return Redirect("/Home/Error");
            }
        }

        // Action performed on view question page
        // POST: /Question/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PostAnswerViewModel model, string returnUrl = null)
        {
            string username = Request.HttpContext.User.Identity.Name;
            Question question = _questions.Find(model.QuestionID);

            if (ModelState.IsValid) // Action is valid
            {
                ApplicationUser user = await _users.FindByEmailAsync(username);

                user.LastActivityDate = DateTime.Now;

                if (model.LockedByDisplayName == null)
                { // Form post is not locking the question
                    if (model.DupeOriginalID == null)
                    { // Form post is not a marking or unmarking as a duplicate 
                        QAPost post = question.VisiblePosts.SingleOrDefault(p => p.QuestionPostID == model.QuestionPostID);
                        model.Contents = new Regex("<\\s*script.*?>.*?(<\\s*\\/script.*?>|$)", RegexOptions.IgnoreCase).Replace(model.Contents, "");
                        if (!model.IsComment) // Form post is an answer
                        {
                            QAPost userPost = question.VisiblePosts.SingleOrDefault(p => p.User.Email == username);
                            if (model.QuestionPostID == null || userPost == post || ((user.Role == "Moderator" && (post.User.Role == "Member")) || (user.Role == "Administrator" && post.User.Email != "admin@codesynergy.net") || user.Email == "admin@codesynergy.net"))
                            {
                                if (model.QuestionPostID == null && post == null) // Form post is a new answer
                                {
                                    question.AddPost(_context, user, model.Contents);
                                    user.QuestionsPosted++;
                                    await _users.UpdateAsync(user);
                                }
                                else if (model.Contents != post.Contents) // Form post is an edit to an existing answer
                                {
                                    post.Contents = model.Contents;
                                    post.EditDate = DateTime.Now;
                                    _context.QAPosts.Update(post);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        else // Form post is a comment
                        {
                            if (model.PostCommentID == null) // Form post is a new comment
                            {
                                if (post != null)
                                {
                                    post.AddComment(_context, user, model.Contents);
                                    user.CommentsPosted++;
                                    await _users.UpdateAsync(user);
                                }
                            }
                            else // Form post is an edit to an existing comment
                            {
                                Comment comment = post.VisibleComments.SingleOrDefault(c => c.PostCommentID == model.PostCommentID);

                                if (comment != null && model.Contents != comment.Contents)
                                {
                                    comment.Contents = model.Contents;
                                    comment.EditDate = DateTime.Now;
                                    _context.Comments.Update(comment);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else // Form post is a mark or unmarking as a duplicate
                    {
                        Question duplicateQuestion = null;
                        Question originalQuestion = _questions.Find((int)model.DupeOriginalID);

                        if (originalQuestion != null)
                        {
                            if (question.QuestionID < originalQuestion.QuestionID)
                            {
                                duplicateQuestion = originalQuestion;
                                originalQuestion = question;
                            }
                            else
                                duplicateQuestion = question;

                            duplicateQuestion.DupeOriginalID = (int)model.DupeOriginalID;
                            _questions.Update(duplicateQuestion);
                        }

                        model.DupeOriginalID = 0;
                    }
                }
                else if (user.Role == "Moderator" || user.Role == "Administrator") // Form post is a locking of the question
                {
                    question.LockedDate = DateTime.Now;
                    question.LockedByUserID = user.Id;
                    _questions.Update(question);
                }

                model.Question = question;
                model.QuestionPostID = null;
                model.PostCommentID = null;
                model.Contents = "";
            }

            ViewBag.PostVotes = _context.QAPostVotes.Include(v => v.User).Where(v => v.User.Email == username && v.QuestionID == model.QuestionID).ToDictionary(v => v.QuestionPostID);
            Dictionary<int, Dictionary<short, CommentVote>> commentVotes = new Dictionary<int, Dictionary<short, CommentVote>>();
            Dictionary<string, RepVote> repVotes = _context.RepVotes.Include(v => v.VoterUser)
                .Where(v => v.VoterUser.Email == Request.HttpContext.User.Identity.Name && question.VisiblePosts.Any(p => p.UserID == v.VoteeUserID)).ToDictionary(v => v.VoteeUserID);
            if (_context.Comments.Any()) // Comments exist in the database
            {
                foreach (QAPost post in question.VisiblePosts) // Go through all the question's (visible) posts
                {
                    Dictionary<short, CommentVote> postCommentVotes = new Dictionary<short, CommentVote>();
                    if (post.VisibleComments.Any()) // If the post has any visible posts, add the comment votes for the post's comments
                        postCommentVotes = _context.CommentVotes.Where(v => v.User.Email == Request.HttpContext.User.Identity.Name && v.QuestionID == model.QuestionID && v.QuestionPostID == post.QuestionPostID).ToDictionary(v => v.PostCommentID);
                    commentVotes.Add(post.QuestionPostID, postCommentVotes);
                }
            }
            ViewBag.CommentVotes = commentVotes;
            ViewBag.RepVotes = repVotes;
            IEnumerable<Question> similarQuestions = _questions.GetAll().Where(q => q.DupeOriginalID == model.QuestionID && !q.QuestionPost.DeletedFlag).OrderBy(q => q.LastActivityDate);
            ViewBag.SimilarQuestionsTotal = similarQuestions.Count();
            ViewBag.SimilarQuestions = similarQuestions.Take(10).ToList();
            IEnumerable<Question> relatedQuestions = _questions.GetAll().Where(q => q.QuestionID != model.QuestionID && q.DupeOriginalID != model.QuestionID && !q.QuestionPost.DeletedFlag && q.TagsString.Split(',').Intersect(question.TagsString.Split(',')).Count() >= 1).OrderBy(q => q.LastActivityDate).OrderBy(q => q.TagsString.Split(',').Intersect(question.TagsString.Split(',')).Count());
            ViewBag.RelatedQuestionsTotal = relatedQuestions.Count();
            ViewBag.RelatedQuestions = relatedQuestions.Take(10).ToList();

            if (!ModelState.IsValid)
                model.Question = question;

            return View(model);
        }

        // Question grid loaded
        // GET: /Question/QuestionGrid
        [HttpGet]
        public IActionResult QuestionGrid(string ColumnIndex = "-1", string SortAsc = "false", string UseSearchGrid = "false")
        {
            int columnIndex = -1;
            bool sortAsc = false;
            bool useSearchGrid = false;
            bool.TryParse(UseSearchGrid, out useSearchGrid);
            if (useSearchGrid)
            {
                int.TryParse(ColumnIndex, out columnIndex);
                bool.TryParse(SortAsc, out sortAsc);
            }
            IEnumerable<Question> questionList = _questions.GetAll().Where(q => !q.QuestionPost.DeletedFlag);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;
            ViewData["useSearchGrid"] = useSearchGrid;

            if (!useSearchGrid && columnIndex == -1)
                questionList = questionList.OrderByDescending(q => q.LastActivityDate);

            return PartialView("MvcGrid/_QuestionGrid", questionList);
        }

        // Related questions grid loaded
        // GET: /Question/RelatedQuestionGrid
        [HttpGet]
        public IActionResult RelatedQuestionGrid(string ColumnIndex = "-1", string SortAsc = "false", string UseSearchGrid = "false", string QuestionID = "1")
        {
            Question sourceQuestion;
            IEnumerable<Question> relatedQuestions;
            int columnIndex = -1;
            bool sortAsc = false;
            bool useSearchGrid = false;
            int questionID = 1;
            bool.TryParse(UseSearchGrid, out useSearchGrid);
            if (useSearchGrid)
            {
                int.TryParse(ColumnIndex, out columnIndex);
                bool.TryParse(SortAsc, out sortAsc);
            }
            int.TryParse(QuestionID, out questionID);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;
            ViewData["useSearchGrid"] = useSearchGrid;
            sourceQuestion = _questions.Find(questionID);
            ViewBag.SourceQuestion = sourceQuestion;
            relatedQuestions = _questions.GetAll().Where(q => q.QuestionID != questionID && q.DupeOriginalID != questionID && !q.QuestionPost.DeletedFlag && q.TagsString.Split(',').Intersect(sourceQuestion.TagsString.Split(',')).Count() >= 1)
                .OrderBy(q => q.LastActivityDate).OrderBy(q => q.TagsString.Split(',').Intersect(sourceQuestion.TagsString.Split(',')).Count());

            return PartialView("MvcGrid/_RelatedQuestionGrid", relatedQuestions);
        }

        // Similar questions grid loaded
        // GET: /Question/SimilarQuestionGrid
        [HttpGet]
        public IActionResult SimilarQuestionGrid(string ColumnIndex = "-1", string SortAsc = "false", string UseSearchGrid = "false", string QuestionID = "1")
        {
            Question sourceQuestion;
            IEnumerable<Question> similarQuestions;
            int columnIndex = -1;
            bool sortAsc = false;
            bool useSearchGrid = false;
            int questionID = 1;
            bool.TryParse(UseSearchGrid, out useSearchGrid);
            if (useSearchGrid)
            {
                int.TryParse(ColumnIndex, out columnIndex);
                bool.TryParse(SortAsc, out sortAsc);
            }
            int.TryParse(QuestionID, out questionID);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;
            ViewData["useSearchGrid"] = useSearchGrid;
            sourceQuestion = _questions.Find(questionID);
            ViewBag.SourceQuestion = sourceQuestion;
            similarQuestions = _questions.GetAll().Where(q => q.DupeOriginalID == questionID && !q.QuestionPost.DeletedFlag).OrderBy(q => q.LastActivityDate);

            return PartialView("MvcGrid/_SimilarQuestionGrid", similarQuestions);
        }

        // Delete post POST request sent
        // POST: /Question/DeletePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeletePost(int questionID, int questionPostID)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            Question question = _questions.Find(questionID);
            QAPost post = question.VisiblePosts.SingleOrDefault(p => p.QuestionPostID == questionPostID);
            bool success = false;
            string errorMessage = "";

            if (post.UserID == user.Id || user.Role == "Moderator" || user.Role == "Administrator")
            {
                user.LastActivityDate = DateTime.Now;
                if (post.QuestionPostID == 1)
                {
                    question.LockedDate = DateTime.Now;
                    question.LockedByUserID = user.Id;
                    List<QuestionTag> questionTagList = _questionTags.GetAll().Where(qt => qt.QuestionID == question.QuestionID).ToList();
                    for (int qt = 0; qt < questionTagList.Count; qt++)
                        _questionTags.Remove(questionTagList.ElementAt(qt));
                    List<Star> stars = question.Stars.ToList();
                    for (int s = 0; s < stars.Count; s++)
                        question.RemoveStar(_context, stars.ElementAt(s));
                }
                question.RemovePost(_context, post);
                success = true;
            }
            else
                errorMessage = "You do not have the rights to delete someone else's post!";

            return Json(new object[] { success, success ? question.AnswerCount.ToString() : errorMessage });
        }

        // Delete comment POST request sent
        // POST: /Question/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteComment(int questionID, int questionPostID, short postCommentID)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            QAPost post = _questions.Find(questionID).VisiblePosts.SingleOrDefault(p => p.QuestionPostID == questionPostID);
            Comment comment = post.VisibleComments.SingleOrDefault(c => c.PostCommentID == postCommentID);
            bool success = false;
            string errorMessage = "";

            if (comment.UserID == user.Id || user.Role == "Moderator" || user.Role == "Administrator")
            {
                user.LastActivityDate = DateTime.Now;
                post.RemoveComment(_context, comment);
                success = true;
            }
            else
                errorMessage = "You do not have the rights to delete someone else's comment!";

            return Json(new object[] { success, success ? post.VisibleComments.Count().ToString() : errorMessage });
        }

        // Add star POST request sent
        // POST: /Question/AddStar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddStar(int questionID)
        {
            Question question = _questions.Find(questionID);
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            bool success = false;
            string errorMessage = "";

            if (question.QuestionPost.UserID != user.Id)
            {
                user.LastActivityDate = DateTime.Now;
                if (!user.Stars.Any(u => u.QuestionID == question.QuestionID))
                    question.AddStar(_context, user);
                success = true;
            }
            else
                errorMessage = "You cannot star your own question!";

            return Json(new object[] { success, errorMessage });
        }

        // Remove star POST request sent
        // POST: /Question/RemoveStar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveStar(int questionID)
        {
            Question question = _questions.Find(questionID);
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            Star star = user.Stars.SingleOrDefault(u => u.QuestionID == question.QuestionID);
            bool success = false;
            
            user.LastActivityDate = DateTime.Now;

            if (star != null)
                question.RemoveStar(_context, star);

            success = true;

            return Json(new object[] { success });
        }

        // Post vote POST request sent
        // POST: /Question/PostVote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostVote(int questionID, int questionPostID, bool vote)
        {
            QAPost post = _questions.Find(questionID).VisiblePosts.SingleOrDefault(p => p.QuestionPostID == questionPostID);
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            QAPostVote postVote = (QAPostVote) post.GetVote(_context, user.Email);
            bool success = false;
            string errorMessage = "";

            if (post.UserID != user.Id)
            {
                user.LastActivityDate = DateTime.Now;
                if (postVote != null)
                {
                    if (postVote.Vote != vote)
                    {
                        postVote.Vote = vote;
                        post.UpdateVote(_context, postVote);
                    }
                    else
                        post.RemoveVote(_context, postVote);
                }
                else
                    post.AddVote(_context, user, vote);
                success = true;
            } else
                errorMessage = "You cannot vote on your own post!";

            return Json(new object[] { success, success ? post.Score.ToString() : errorMessage });
        }

        // Comment vote POST request sent
        // POST: /Question/CommentVote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CommentVote(int questionID, int questionPostID, short postCommentID, bool vote)
        {
            Comment comment = _questions.Find(questionID).VisiblePosts.SingleOrDefault(p => p.QuestionPostID == questionPostID).VisibleComments.SingleOrDefault(c => c.PostCommentID == postCommentID);
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            CommentVote commentVote = (CommentVote) comment.GetVote(_context, user.Email);
            bool success = false;
            string errorMessage = "";

            if (comment.UserID != user.Id)
            {
                user.LastActivityDate = DateTime.Now;
                if (commentVote != null)
                {
                    if (commentVote.Vote != vote)
                    {
                        commentVote.Vote = vote;
                        comment.UpdateVote(_context, commentVote);
                    }
                    else
                        comment.RemoveVote(_context, commentVote);
                }
                else
                    comment.AddVote(_context, user, vote);
                success = true;
            }
            else
                errorMessage = "You cannot vote on your own comment!";

            return Json(new object[] { success, success ? comment.Score.ToString() : errorMessage });
        }

        // Create question page loaded
        // GET: /Question/Create
        [HttpGet]
        public IActionResult Create()
        {
            // If the user is not logged in, redirect them to the homepage with a login modal
            if (HttpContext.User.Identity.Name == null)
                return Redirect("/?modal=Account/Login");
            return View();
        }

        // Get tag for tag name POST request sent
        // POST: /Question/GetTagForTagName
        [HttpPost]
        public JsonResult GetTagForTagName(string tagName)
        {
            Tag tag = _tags.Find(tagName); // Get the tag (if any) that matches the provided name

            return Json(new object[] { tag != null ? tag.TagID : 0, tag != null ? tag.TagName : tagName });
        }

        // Create question POST request sent
        // POST: /Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostQuestionViewModel model, string Modal)
        {
            if (ModelState.IsValid) // Question is valid
            {
                ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
                Regex scriptPattern = new Regex("<\\s*script.*?>.*?(<\\s*\\/script.*?>|$)", RegexOptions.IgnoreCase); // Regex pattern for script tags
                Question question = new Question() // Create a new question
                {
                    Summary = model.Summary = scriptPattern.Replace(model.Summary, "")
                };

                user.LastActivityDate = DateTime.Now;

                _questions.Add(question); // Add the question to the database

                question.AddPost(_context, user, scriptPattern.Replace(model.Contents, "")); // Add the question post

                // Go through each tag
                for (int t = 0; t < model.Tags.Length; t++)
                {
                    Tag tag = model.Tags[t];
                    // Add the tag to the database if it is a new tag
                    if (tag.TagID == 0)
                    {
                        Tag tagByName = _tags.Find(tag.TagName);
                        if (tagByName == null)
                            _tags.Add(tag);
                        else
                            tag = tagByName;
                    }

                    // Add a question tag for the tag
                    _questionTags.Add(new QuestionTag()
                    {
                        QuestionID = question.QuestionID,
                        TagID = tag.TagID
                    });
                }

                user.QuestionsPosted++;
                await _users.UpdateAsync(user);

                return Redirect("/Question/" + question.QuestionID);
            }

            return View(model);
        }

        // Post question tags POST request sent
        // POST: /Question/PostQuestionTags
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostQuestionTags(int QuestionID, Tag[] Tags)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            Question question = _questions.Find(QuestionID);
            IEnumerable<QuestionTag> allQuestionTags = _questionTags.GetAll();
            List<QuestionTag> questionTags = allQuestionTags.Where(qt => qt.QuestionID == QuestionID).ToList();

            user.LastActivityDate = DateTime.Now;

            for (int t = 0; t < Tags.Length; t++) // For each tag in the request's tag array
            {
                Tag tag = Tags[t];
                bool isNewTag = false;
                if (tag.TagID == 0) // If the tag doesn't exist in the database yet, add it
                {
                    Tag tagByName = _tags.Find(tag.TagName);
                    if (tagByName == null)
                    {
                        _tags.Add(tag);
                        isNewTag = true;
                    }
                    else
                        tag = tagByName;
                }

                if (isNewTag || !questionTags.Any(qt => qt.TagID == tag.TagID)) // If the tag is a new tag to the question, add a question tag for it
                {
                    _questionTags.Add(new QuestionTag()
                    {
                        QuestionID = question.QuestionID,
                        TagID = tag.TagID
                    });
                }
            }

            for (int qt = 0; qt < questionTags.Count(); qt++) // For each question tag for the question
            {
                QuestionTag questionTag = questionTags[qt];
                if (!Tags.Any(t => t.TagID == questionTag.TagID)) // If the question tag is for a tag that is no longer used on this question, remove its question tag
                    _questionTags.Remove(questionTag);
            }

            return Json(new object[] { true, Tags.ToArray() });
        }

        // Related questions modal loaded
        // GET: /Question/RelatedQuestions/Id
        [HttpGet]
        public IActionResult RelatedQuestions(int Id)
        {
            Question question = _questions.Find(Id);
            return View(question);
        }

        // Similar questions page loaded
        // GET: /Question/SimilarQuestions/Id
        [HttpGet]
        public IActionResult SimilarQuestions(int Id)
        {
            Question question = _questions.Find(Id);
            return View(question);
        }
    }
}
