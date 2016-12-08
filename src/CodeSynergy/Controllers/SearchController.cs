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
        private readonly UserRepository _users; // Repository for users
        private readonly QuestionRepository _questions; // Repository for questions
        private readonly TagRepository _tags; // Repository for tags
        private readonly QuestionTagRepository _questionTags; // Repository for question tags

        public SearchController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IRepository<Question, int> questions, IRepository<Tag, int> tags, IJoinTableRepository<QuestionTag, int, int> questionTags) : base()
        {
            _users = (UserRepository) users;
            _questions = (QuestionRepository) questions;
            _tags = (TagRepository) tags;
            _questionTags = (QuestionTagRepository) questionTags;
        }

        // Search page loaded
        // GET: /Search/SearchType/Query
        public IActionResult Index(string SearchType, string Query, string RowsPerPage, string Modal)
        {
            ViewData["Modal"] = Modal;
            return View(new SearchViewModel(SearchType, Query, RowsPerPage));
        }

        // Search page POST request sent
        // POST: /Search/
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(SearchViewModel SearchModel)
        {
            if (ModelState.IsValid) // Search is valid: redirect to the search page using the posted values for parameters
                return Redirect("/Search/" + Enum.GetName(typeof(SearchViewModel.EnumSearchType), SearchModel.SearchType) + "s/" + SearchModel.Query + "?rowsperpage=" + SearchModel.RowsPerPage);

            return View(SearchModel); // Search was invalid: refresh and show errors
        }

        // Search questions grid loaded
        // GET: /Search/SearchQuestionGrid
        [HttpGet]
        public IActionResult SearchQuestionGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchQuestionGrid", SearchResult<Question>.GetSearchResults(_questions.GetAll().Where(q => !q.QuestionPost.DeletedFlag), Query));
        }

        // Search posts grid loaded
        // GET: /Search/SearchPostGrid
        [HttpGet]
        public IActionResult SearchPostGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            IEnumerable<Post> posts = _questions.GetAll().Where(q => !q.QuestionPost.DeletedFlag).SelectMany(q => q.Posts).Where(p => !p.DeletedFlag).AsEnumerable();
            IEnumerable<Post> comments = (posts as IEnumerable<QAPost>).SelectMany(p => p.Comments).Where(c => !c.DeletedFlag).Where(c => !c.Question.QuestionPost.DeletedFlag);
            return PartialView("MvcGrid/_SearchPostGrid", SearchResult<Post>.GetSearchResults(posts, Query).Concat(SearchResult<Post>.GetSearchResults(comments, Query)));
        }

        // Search answers grid loaded
        // GET: /Search/SearchAnswerGrid
        [HttpGet]
        public IActionResult SearchAnswerGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchAnswerGrid", SearchResult<QAPost>.GetSearchResults(_questions.GetAll().Where(q => !q.QuestionPost.DeletedFlag).SelectMany(q => q.Posts)
                .Where(p => !p.DeletedFlag).AsEnumerable(), Query));
        }

        // Search tagss grid loaded
        // GET: /Search/SearchTagGrid
        [HttpGet]
        public IActionResult SearchTagGrid(string Query, string RowsPerPage = "25")
        {
            int rowsPerPage = 25;
            int.TryParse(RowsPerPage, out rowsPerPage);
            ViewBag.RowsPerPage = rowsPerPage;
            return PartialView("MvcGrid/_SearchTagGrid", SearchResult<Tag>.GetSearchResults(_tags.GetAll(), Query));
        }

        // Search users grid loaded
        // GET: /Search/SearchUserGrid
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
