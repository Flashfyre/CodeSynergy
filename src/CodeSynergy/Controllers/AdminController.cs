using CodeSynergy.Data;
using CodeSynergy.Models;
using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeSynergy.Controllers
{
    public class AdminController : Controller
    {
        UserRepository users;
        BanRepository bans;
        UntrustedURLPatternRepository untrustedURLPatterns;

        public AdminController() : base()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            users = new UserRepository(context);
            bans = new BanRepository(context);
            untrustedURLPatterns = new UntrustedURLPatternRepository(context);
        }

        //
        // GET: /Admin/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/Moderation
        [HttpGet]
        public IActionResult Moderation()
        {
            IEnumerable<EnumBanTerm> banTerms = BanTermHelper.Values();
            ViewBag.BanTerms = banTerms;
            return View();
        }

        //
        // GET: /Admin/UserBanGrid
        [HttpGet]
        public IActionResult UserBanGrid(String param)
        {
            return PartialView("MvcGrid/_UserBanGrid", bans.GetAll().Where(b => b.Active));
        }

        //
        // POST: /Admin/ChangeBanTerm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult BanUser(String userDisplayName, int banTerm, String banReason)
        {
            int errorCount = 0;
            String errorMessage = "";
            ApplicationUser user = null;

            if (userDisplayName == null)
            {
                errorCount++;
                errorMessage = "You must enter a user display name!";
            }
            if (banReason == null)
            {
                errorCount++;
                errorMessage += (errorCount == 1 ? "" : "\r\n") + "You must enter a ban reason!";
            }
            if (errorCount == 0)
            {
                user = users.FindByDisplayName(userDisplayName);
                if (user == null)
                {
                    errorCount++;
                    errorMessage = "No user with the display name '" + userDisplayName + "' exists!";
                }
                else if (Request.HttpContext.User.Identity.Name == user.UserName)
                {
                    errorCount++;
                    errorMessage = "You may not ban yourself!";
                }
                else if (Request.HttpContext.User.IsInRole("Administrator") &&
                  (user.Role == "Administrator" && Request.HttpContext.User.Identity.Name != "admin@codesynergy.com"))
                {
                    errorCount++;
                    errorMessage = "You may not ban the main administrator!";
                }
                else if (Request.HttpContext.User.IsInRole("Moderator") && user.Role != "Member")
                {
                    errorCount++;
                    errorMessage = "You do not have the privileges to ban that user!";
                } else if (bans.GetAllForUser(user, true).Count() > 0)
                {
                    errorCount++;
                    errorMessage = "There is already an active ban on that user!";
                }
            }

            if (errorCount == 0)
            {
                EnumBanTerm enumBanTerm = (EnumBanTerm) banTerm;
                DateTime banDate = DateTime.Now;
                bans.Add(new Ban()
                {
                    BannedUserID = user.Id,
                    BanningUserID = users.FindByUserName(Request.HttpContext.User.Identity.Name).Id,
                    BanTerm = (byte) banTerm,
                    BanReason = banReason,
                    BanDate = banDate,
                    BanLiftDate = enumBanTerm.DateTimeOffset(banDate)
                });
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // POST: /Admin/ChangeBanTerm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangeBanTerm(int banID, int banTerm)
        {
            Ban ban = bans.Find(banID);

            int errorCount = 0;
            String errorMessage = null;

            if (banTerm == ban.BanTerm)
            {
                errorCount++;
            } else if (Request.HttpContext.User.Identity.Name == ban.BannedUser.UserName)
            {
                errorCount++;
                errorMessage = "You may not change your own ban term! Your ban term has been reset for attempting to alter the term of your own active ban.";
                EnumBanTerm enumBanTerm = (EnumBanTerm) ban.BanTerm;
                ban.BanDate = DateTime.Now;
                ban.BanLiftDate = enumBanTerm.DateTimeOffset(DateTime.Now);
                bans.Update(ban);
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && ban.BannedUser.Role != "Member")
            {
                errorCount++;
                errorMessage = "You do not have the privileges to change the ban term of that user!";
            }
            {
                ban.BanTerm = (byte) banTerm;
                ban.BanLiftDate = ((EnumBanTerm) banTerm).DateTimeOffset(ban.BanDate);
                bans.Update(ban);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // POST: /Admin/LiftBan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LiftBan(int banID)
        {
            Ban ban = bans.Find(banID);

            int errorCount = 0;
            String errorMessage = null;

            if (Request.HttpContext.User.Identity.Name == ban.BannedUser.UserName)
            {
                errorCount++;
                errorMessage = "You may not life your own ban term! Your ban term has been reset for attempting to lift your own active ban.";
                EnumBanTerm enumBanTerm = (EnumBanTerm) ban.BanTerm;
                ban.BanDate = DateTime.Now;
                ban.BanLiftDate = enumBanTerm.DateTimeOffset(DateTime.Now);
                bans.Update(ban);
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && ban.BannedUser.Role != "Member")
            {
                errorCount++;
                errorMessage = "You do not have the privileges to lift the ban of that user!";
            }
            {
                ban.BanLiftDate = DateTime.Now;
                bans.Update(ban);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // GET: /Admin/UntrustedURLPatternGrid
        [HttpGet]
        public IActionResult UntrustedURLPatternGrid(String param)
        {
            return PartialView("MvcGrid/_UntrustedURLPatternGrid", untrustedURLPatterns.GetAll().Where(u => u.RemovedByUserID == null));
        }

        //
        // POST: /Admin/AddUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddUntrustedURLPattern(string patternText)
        {
            int errorCount = 0;
            String errorMessage = null;

            if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator"))
            {
                errorCount++;
                errorMessage = "You do not have the privileges to add an untrusted URL pattern!";
            } else if (patternText == "") {
                errorCount++;
            } else {
                if (!new Regex(@"^/.*/[g|i|m|u|y]?$", RegexOptions.IgnoreCase).IsMatch(patternText))
                {
                    patternText = "/" + patternText + "/";
                }
                ApplicationUser user = users.FindByUserName(Request.HttpContext.User.Identity.Name);
                untrustedURLPatterns.Add(new UntrustedURLPattern()
                {
                    AddedByUser = user,
                    PatternText = patternText
                });
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // POST: /Admin/UpdateUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUntrustedURLPattern(int patternID, string patternText)
        {
            UntrustedURLPattern pattern = untrustedURLPatterns.Find(patternID);
            int errorCount = 0;
            String errorMessage = null;

            if (pattern == null)
            {
                errorCount++;
                errorMessage = "No untrusted URL pattern exists with the ID '" + patternID.ToString() + "'!";
            } else if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator"))
            {
                errorCount++;
                errorMessage = "You do not have the privileges to modify an untrusted URL pattern!";
            }
            else if (Request.HttpContext.User.IsInRole("Administrator") &&
              (pattern.AddedByUser.Role == "Administrator" && Request.HttpContext.User.Identity.Name != "admin@codesynergy.com"))
            {
                errorCount++;
                errorMessage = "You may not modify an untrusted URL pattern added by the main administrator!";
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && pattern.AddedByUser.Role == "Administrator")
            {
                errorCount++;
                errorMessage = "You do not have the privileges to modify that untrusted URL pattern!";
            }
            else if (patternText == "" || pattern.PatternText == patternText)
            {
                errorCount++;
            }
            else
            {
                if (!new Regex(@"^/.*/[g|i|m|u|y]?$", RegexOptions.IgnoreCase).IsMatch(patternText))
                {
                    patternText = "/" + patternText + "/";
                }
                ApplicationUser user = users.FindByUserName(Request.HttpContext.User.Identity.Name);
                pattern.PatternText = patternText;
                pattern.LastUpdatedByUserID = user.Id;
                untrustedURLPatterns.Update(pattern);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // POST: /Admin/DeleteUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteUntrustedURLPattern(int patternID)
        {
            UntrustedURLPattern pattern = untrustedURLPatterns.Find(patternID);
            int errorCount = 0;
            String errorMessage = null;

            if (pattern == null)
            {
                errorCount++;
                errorMessage = "No untrusted URL pattern exists with the ID '" + patternID.ToString() + "'!";
            }
            else if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator"))
            {
                errorCount++;
                errorMessage = "You do not have the privileges to delete an untrusted URL pattern!";
            } else if (Request.HttpContext.User.IsInRole("Administrator") &&
                (pattern.AddedByUser.Role == "Administrator" && Request.HttpContext.User.Identity.Name != "admin@codesynergy.com"))
            {
                errorCount++;
                errorMessage = "You may not delete an untrusted URL pattern added by the main administrator!";
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && pattern.AddedByUser.Role == "Administrator")
            {
                errorCount++;
                errorMessage = "You do not have the privileges to delete that untrusted URL pattern!";
            }
            else
            {
                ApplicationUser user = users.FindByUserName(Request.HttpContext.User.Identity.Name);
                pattern.RemovedByUserID = user.Id;
                untrustedURLPatterns.Update(pattern);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // GET: /Admin/UserRoles
        [HttpGet]
        public IActionResult UserRoles()
        {
            return View();
        }

        //
        // GET: /Admin/UserRoleGrid
        [HttpGet]
        public IActionResult UserRoleGrid(String param)
        {
            return PartialView("MvcGrid/_UserRoleGrid", users.GetAll());
        }

        //
        // POST: /Admin/ChangeUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangeUserRole(String roleId, String userId)
        {
            ApplicationUser user = users.Find(userId);
            int errorCount = 0;
            String errorMessage = null;

            if (!Request.HttpContext.User.IsInRole("Administrator"))
            {
                errorCount++;
                errorMessage = "You are you not an administrator! You received a 1 day ban for suspicious activity.";
                user = users.FindByUserName(Request.HttpContext.User.Identity.Name);
                EnumBanTerm enumBanTerm = EnumBanTerm.OneDay;
                DateTime banDate = DateTime.Now;
                bans.Add(new Ban()
                {
                    BannedUserID = users.FindByUserName(Request.HttpContext.User.Identity.Name).Id,
                    BanningUserID = users.FindByUserName("admin@codesynergy.com").Id,
                    BanTerm = (byte)enumBanTerm,
                    BanReason = "Suspicious activity - Attempting to change a user role in admin-only user role controls",
                    BanDate = banDate,
                    BanLiftDate = enumBanTerm.DateTimeOffset(banDate)
                });
            }
            else if (roleId == user.Role)
            {
                errorCount++;
            }
            else if (user.UserName == Request.HttpContext.User.Identity.Name)
            {
                errorCount++;
                errorMessage = "You may not change your own role!";
            }
            else if (Request.HttpContext.User.Identity.Name != "admin@codesynergy.com")
            {
                errorCount++;
                errorMessage = "You may not change the role of the main administrator!";
            } else
            {
                user.Role = roleId;
                users.Update(user);
            }

            return Json(new object[] { errorCount, errorMessage });
        }
    }
}
