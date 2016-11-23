using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeSynergy.Models.SearchViewModels;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CodeSynergy.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserRepository _users;
        private readonly QuestionRepository _questions;
        private readonly TagRepository _tags;
        private readonly QuestionTagRepository _questionTags;

        public SearchController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IRepository<Question, int> questions, IRepository<Tag, int> tags, IJoinTableRepository<QuestionTag, int, int> questionTags) : base()
        {
            _context = new ApplicationDbContext();
            _users = (UserRepository) users;
            _questions = (QuestionRepository) questions;
            _tags = (TagRepository) tags;
            _questionTags = (QuestionTagRepository) questionTags;
        }

        // GET: /Search/SearchType/Query
        public IActionResult Index(string SearchType, string Query, string RowsPerPage, string Modal)
        {
            ViewData["Modal"] = Modal;
            return View(new SearchViewModel(SearchType, Query, RowsPerPage));
        }

        // POST: /Search/
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(SearchViewModel SearchModel)
        {
            if (ModelState.IsValid)
            {
                return Redirect("/Search/" + Enum.GetName(typeof(SearchViewModel.EnumSearchType), SearchModel.SearchType) + "s/" + SearchModel.Query + "?rowsperpage=" + SearchModel.RowsPerPage);
            }

            return View(SearchModel);
        }

        [HttpGet]
        public IActionResult SearchQuestionGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchQuestionGrid", SearchResult<Question>.GetSearchResults(_questions.GetAll().Where(q => !q.QuestionPost.DeletedFlag), Query));
        }

        [HttpGet]
        public IActionResult SearchPostGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            IEnumerable<Post> posts = _context.QAPosts.Where(p => !p.DeletedFlag).Include(p => p.Question).Where(p => !p.Question.QuestionPost.DeletedFlag).Include(p => p.User).AsEnumerable();
            IEnumerable<Post> comments = _context.Comments.Where(c => !c.DeletedFlag).Include(c => c.Question).Where(c => !c.Question.QuestionPost.DeletedFlag).Include(c => c.User);
            return PartialView("MvcGrid/_SearchPostGrid", SearchResult<Post>.GetSearchResults(posts, Query).Concat(SearchResult<Post>.GetSearchResults(comments, Query)));
        }

        [HttpGet]
        public IActionResult SearchAnswerGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchAnswerGrid", SearchResult<QAPost>.GetSearchResults(_context.QAPosts.Where(a => !a.DeletedFlag).Include(p => p.Question).Where(a => !a.Question.QuestionPost.DeletedFlag).Include(p => p.User).AsEnumerable(), Query));
        }

        [HttpGet]
        public IActionResult SearchTagGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchTagGrid", SearchResult<Tag>.GetSearchResults(_tags.GetAll(), Query));
        }

        [HttpGet]
        public IActionResult SearchUserGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            ViewBag.Questions = _questions;
            return PartialView("MvcGrid/_SearchUserGrid", SearchResult<ApplicationUser>.GetSearchResults(_users.GetAll(), Query));
        }
    }
}
