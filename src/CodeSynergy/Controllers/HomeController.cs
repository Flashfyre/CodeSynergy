using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using CodeSynergy.Models.HomeViewModels;

namespace CodeSynergy.Controllers
{
    public class HomeController : Controller
    {
        QuestionRepository questions;
        TagRepository tags;

        public HomeController() : base()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            questions = new QuestionRepository(context);
            tags = new TagRepository(context);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Timeout()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QuestionGrid(String param)
        {
            return PartialView("MvcGrid/_QuestionGrid", questions.GetAll());
        }

        [HttpGet]
        public IActionResult ViewQuestion()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditQuestion()
        {
            return View();
        }

        //
        // POST: /Home/GetTagForTagName
        [HttpPost]
        public JsonResult GetTagForTagName(String tagName)
        {
            Tag tag = tags.Find(tagName);

            return Json(new object[] { tag != null ? tag.TagID : 0, tag != null ? tag.TagName : tagName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostQuestion(PostQuestionViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                Question question = new Question();
                question.Summary = model.Summary;
                question.PostDate = new DateTime();
                foreach (Tag t in model.Tags)
                {
                    if (tags.Find(t.TagID) != null)
                    {
                        tags.Add(t);
                    }
                }
                questions.Add(question);

                Post questionPost = new Post();
                questionPost.QuestionID = question.QuestionID;
                questionPost.Contents = model.Content;
                questionPost.UserID = null;
                questionPost.PostDate = new DateTime();
                questionPost.QuestionPostID = 1;

                question.Posts.Add(questionPost);

                questions.Update(question);
            }

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
