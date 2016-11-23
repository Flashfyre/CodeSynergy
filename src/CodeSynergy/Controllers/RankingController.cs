using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using CodeSynergy.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CodeSynergy.Controllers
{
    public class RankingController : Controller
    {
        private readonly RankingPosRepository _rankingPos;

        public RankingController(IRepository<RankingPos, short> rankingPos) : base()
        {
            _rankingPos = (RankingPosRepository) rankingPos;
        }

        // GET: /Ranking/Page/
        [HttpGet]
        public IActionResult Index(int Id = 1, string Modal = null)
        {
            ViewData["Modal"] = Modal;
            return View(Id);
        }

        // GET: /Ranking/RankingGrid
        [HttpGet]
        public IActionResult RankingGrid(string ColumnIndex = "-1", string SortAsc = "false", string Page = "1")
        {
            int columnIndex = -1;
            bool sortAsc = false;
            int page = 1;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            int.TryParse(Page, out page);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;
            ViewData["page"] = page;
            IEnumerable<RankingPos> rankingPos = _rankingPos.GetAll().ToList();

            return PartialView("MvcGrid/_RankingGrid", rankingPos);
        }
    }
}
