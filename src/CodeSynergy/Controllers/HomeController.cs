using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Newtonsoft.Json;

namespace CodeSynergy.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestionRepository _questions; // Repository for questions

        public HomeController(IRepository<Question, int> questions) : base()
        {
            _questions = (QuestionRepository) questions;
        }

        // Home page loaded
        // GET: [/Home]/Index
        [HttpGet]
        public IActionResult Index(string Modal)
        {
            ViewData["Modal"] = Modal;
            List<Question> recentQuestions = new List<Question>(); // List of recent questions to add to the homepage
            // Get recent questions (if any) from cookies and add them to the list
            if (Request.Cookies.Any(c => c.Key == "recently_viewed"))
            {
                foreach (int questionID in JsonConvert.DeserializeObject<int[]>(Request.Cookies["recently_viewed"]))
                {
                    recentQuestions.Add(_questions.Find(questionID));
                }
            }
            ViewBag.RecentQuestions = recentQuestions;
            return View();
        }

        // Question grid loaded
        // GET: [/Home]/QuestionGrid
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

        // Timeout modal loaded
        // GET: [/Home]/Timeout
        [HttpGet]
        public IActionResult Timeout()
        {
            return View();
        }

        // Error page loaded
        // GET: [/Home]/Error
        public IActionResult Error()
        {
            return View();
        }
    }
}
