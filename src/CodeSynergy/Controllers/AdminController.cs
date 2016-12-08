using CodeSynergy.Data;
using CodeSynergy.Models;
using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private readonly UserRepository _users; // Repository for users
        private readonly BanRepository _bans; // Repository for bans
        private readonly UntrustedURLPatternRepository _untrustedURLPatterns; // Repository for regular expressions that validate posts

        public AdminController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IRepository<Ban, int> bans, IRepository<UntrustedURLPattern, int> untrustedURLPatterns) : base()
        {
            _users = (UserRepository) users;
            _bans = (BanRepository) bans;
            _untrustedURLPatterns = (UntrustedURLPatternRepository) untrustedURLPatterns;
        }

        // Moderation modal loaded
        // GET: /Admin/Moderation
        [HttpGet]
        public IActionResult Moderation(string DisplayName = null)
        {
            if (HttpContext.User.Identity.Name == null) // If the user is not logged in, redirect them to the homepage with a login modal
                return Redirect("/?modal=Account/Login");
            else
            {
                ApplicationUser user = _users.FindByEmail(HttpContext.User.Identity.Name);
                if (user.Role != "Moderator" && user.Role != "Administrator") // If the user is not a moderator or administrator, redirect them to the homepage
                    return Redirect("/");
            }
            ViewData["DisplayName"] = DisplayName;
            ViewBag.BanTerms = BanTermHelper.Values();

            return View();
        }

        // User ban grid loaded
        // GET: /Admin/UserBanGrid
        [HttpGet]
        public IActionResult UserBanGrid(string ColumnIndex = "-1", string SortAsc = "false")
        {
            int columnIndex = -1;
            bool sortAsc = false;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;

            return PartialView("MvcGrid/_UserBanGrid", _bans.GetAll().Where(b => b.Active));
        }

        // Ban user POST request sent
        // POST: /Admin/ChangeBanTerm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> BanUser(string userDisplayName, int banTerm, string banReason)
        {
            int errorCount = 0;
            string errorMessage = "";
            ApplicationUser user = null;

            // Check for null request parameters
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
            if (errorCount == 0) // No errors: get the user object for the display name and check for errors
            {
                user = await _users.FindByDisplayNameAsync(userDisplayName);
                if (user == null)
                {
                    errorCount++;
                    errorMessage = "You may not ban a user that does not exist!";
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
                } else if (_bans.GetAllForUser(user, true).Count() > 0)
                {
                    errorCount++;
                    errorMessage = "There is already an active ban on that user!";
                }
            }

            if (errorCount == 0) // No errors: ban the user
            {
                EnumBanTerm enumBanTerm = (EnumBanTerm) banTerm;
                DateTime banDate = DateTime.Now;
                _bans.Add(new Ban()
                {
                    BannedUserID = user.Id,
                    BanningUserID = (await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name)).Id,
                    BanTerm = (byte) banTerm,
                    BanReason = banReason,
                    BanDate = banDate,
                    BanLiftDate = enumBanTerm.DateTimeOffset(banDate)
                });
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        // Change ban term POST request sent
        // POST: /Admin/ChangeBanTerm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangeBanTerm(int banID, int banTerm)
        {
            Ban ban = _bans.Find(banID); // Existing ban

            int errorCount = 0;
            string errorMessage = null;

            if (banTerm == ban.BanTerm) // Ban already has the specified term: cancel and add error
                errorCount++;
            else if (Request.HttpContext.User.Identity.Name == ban.BannedUser.Email) // User is attempting to change their own ban term: cancel, add error, and reset their ban term
            {
                errorCount++;
                errorMessage = "You may not change your own ban term! Your ban term has been reset for attempting to alter the term of your own active ban.";
                EnumBanTerm enumBanTerm = (EnumBanTerm) ban.BanTerm;
                ban.BanDate = DateTime.Now;
                ban.BanLiftDate = enumBanTerm.DateTimeOffset(DateTime.Now);
                _bans.Update(ban);
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && ban.BannedUser.Role != "Member") // Privileges not sufficient: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to change the ban term of that user!";
            }
            if (errorCount == 0) // No errors: change the ban term of the ban
            {
                ban.BanTerm = (byte) banTerm;
                ban.BanLiftDate = ((EnumBanTerm) banTerm).DateTimeOffset(ban.BanDate);
                _bans.Update(ban);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        // Lift ban POST request sent
        // POST: /Admin/LiftBan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LiftBan(int banID)
        {
            Ban ban = _bans.Find(banID); // Existing ban

            int errorCount = 0;
            string errorMessage = null;

            if (Request.HttpContext.User.Identity.Name == ban.BannedUser.Email) // User is attempting to unban themself: cancel, add error, and reset their ban term
            {
                errorCount++;
                errorMessage = "You may not lift your own ban term! Your ban term has been reset for attempting to lift your own active ban.";
                EnumBanTerm enumBanTerm = (EnumBanTerm) ban.BanTerm;
                ban.BanDate = DateTime.Now;
                ban.BanLiftDate = enumBanTerm.DateTimeOffset(DateTime.Now);
                _bans.Update(ban);
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && ban.BannedUser.Role != "Member") // User has insufficient privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to lift the ban of that user!";
            }
            if (errorCount == 0) // No errors: lift the ban
            {
                ban.BanLiftDate = DateTime.Now;
                _bans.Update(ban);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        //
        // GET: /Admin/UntrustedURLPatternGrid
        [HttpGet]
        public IActionResult UntrustedURLPatternGrid(string ColumnIndex = "-1", string SortAsc = "false")
        {
            int columnIndex = -1;
            bool sortAsc = false;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;

            return PartialView("MvcGrid/_UntrustedURLPatternGrid", _untrustedURLPatterns.GetAll().Where(u => u.RemovedByUserID == null));
        }

        // Add untrusted URL POST request sent
        // POST: /Admin/AddUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddUntrustedURLPattern(string patternText)
        {
            int errorCount = 0;
            string errorMessage = null;

            if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator")) // User has insufficient privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to add an untrusted URL pattern!";
            } else if (patternText == "") // Empty URL pattern: cancel and add error
                errorCount++;
            else { // No errors: add the untrusted URL pattern
                if (!new Regex(@"^/.*/[g|i|m|u|y]?$", RegexOptions.IgnoreCase).IsMatch(patternText)) // If slashes are not present, add them
                    patternText = "/" + patternText + "/";

                ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
                _untrustedURLPatterns.Add(new UntrustedURLPattern()
                {
                    AddedByUser = user,
                    PatternText = patternText
                });
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        // Update untrusted URL pattern POST request sent
        // POST: /Admin/UpdateUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateUntrustedURLPattern(int patternID, string patternText)
        {
            UntrustedURLPattern pattern = _untrustedURLPatterns.Find(patternID); // Specified pattern
            int errorCount = 0;
            string errorMessage = null;

            if (pattern == null) // Pattern doesn't exist: cancel and show error
            {
                errorCount++;
                errorMessage = "No untrusted URL pattern exists with the ID '" + patternID.ToString() + "'!";
            } else if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator")) // User has insufficient privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to modify an untrusted URL pattern!";
            }
            else if (Request.HttpContext.User.IsInRole("Administrator") &&
              (pattern.AddedByUser.Role == "Administrator" && Request.HttpContext.User.Identity.Name != "admin@codesynergy.com")) // User is attempting to change a pattern added by the main administrator: cancel and add error
            {
                errorCount++;
                errorMessage = "You may not modify an untrusted URL pattern added by the main administrator!";
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && pattern.AddedByUser.Role == "Administrator") // Pattern was added by a user with higher privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to modify that untrusted URL pattern!";
            }
            else if (patternText == "" || pattern.PatternText == patternText) // Pattern is empty or the same: cancel and add error
                errorCount++;
            else // No errors: update the untrusted URL pattern
            {
                if (!new Regex(@"^/.*/[g|i|m|u|y]?$", RegexOptions.IgnoreCase).IsMatch(patternText)) // If slashes are not present, add them
                {
                    patternText = "/" + patternText + "/";
                }
                ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
                pattern.PatternText = patternText;
                pattern.LastUpdatedByUserID = user.Id;
                _untrustedURLPatterns.Update(pattern);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        // Delete untrusted URL pattern POST request sent
        // POST: /Admin/DeleteUntrustedURLPattern
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteUntrustedURLPattern(int patternID)
        {
            UntrustedURLPattern pattern = _untrustedURLPatterns.Find(patternID);
            int errorCount = 0;
            string errorMessage = null;

            if (pattern == null) // Pattern doesn't exist: cancel and add error
            {
                errorCount++;
                errorMessage = "No untrusted URL pattern exists with the ID '" + patternID.ToString() + "'!";
            }
            else if (!Request.HttpContext.User.IsInRole("Administrator") && !Request.HttpContext.User.IsInRole("Moderator")) // User has insufficient privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to delete an untrusted URL pattern!";
            } else if (Request.HttpContext.User.IsInRole("Administrator") &&
                (pattern.AddedByUser.Role == "Administrator" && Request.HttpContext.User.Identity.Name != "admin@codesynergy.com")) // User is attempting to delete a pattern added by the main administrator: cancel and add error
            {
                errorCount++;
                errorMessage = "You may not delete an untrusted URL pattern added by the main administrator!";
            }
            else if (Request.HttpContext.User.IsInRole("Moderator") && pattern.AddedByUser.Role == "Administrator") // User is attempting to delete a pattern created by a user with higher privileges: cancel and add error
            {
                errorCount++;
                errorMessage = "You do not have the privileges to delete that untrusted URL pattern!";
            }
            else // No errors: delete the untrusted URL pattern
            {
                ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
                pattern.RemovedByUserID = user.Id;
                _untrustedURLPatterns.Update(pattern);
            }

            return Json(new object[] { errorCount, errorMessage });
        }

        // User roles modal loaded
        // GET: /Admin/UserRoles
        [HttpGet]
        public IActionResult UserRoles()
        {
            if (HttpContext.User.Identity.Name == null) // If the user is not logged in, redirect them to the homepage with a login modal
                return Redirect("/?modal=Account/Login");
            else
            {
                ApplicationUser user = _users.FindByEmail(HttpContext.User.Identity.Name);
                if (user.Role != "Administrator") // If the user is not an administrator, redirect them to the homepage
                    return Redirect("/");
            }

            return View();
        }

        // User role grid loaded
        // GET: /Admin/UserRoleGrid
        [HttpGet]
        public IActionResult UserRoleGrid(string ColumnIndex = "-1", string SortAsc = "false")
        {
            int columnIndex = -1;
            bool sortAsc = false;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;

            return PartialView("MvcGrid/_UserRoleGrid", _users.GetAll());
        }

        // Change user role POST request sent
        // POST: /Admin/ChangeUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeUserRole(string roleId, string userId)
        {
            ApplicationUser user = await _users.FindByIdAsync(userId);
            int errorCount = 0;
            string errorMessage = null;

            // User is not an administrator: cancel, add error, and ban the user for 1 week for using client-side page alteration to attempt to go beyond their privileges
            if (!Request.HttpContext.User.IsInRole("Administrator"))
            {
                errorCount++;
                errorMessage = "You are you not an administrator! You have received a 1 week ban for suspicious activity.";
                user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
                EnumBanTerm enumBanTerm = EnumBanTerm.OneWeek;
                DateTime banDate = DateTime.Now;
                _bans.Add(new Ban()
                {
                    BannedUserID = (await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name)).Id,
                    BanningUserID = (await _users.FindByEmailAsync("admin@codesynergy.com")).Id,
                    BanTerm = (byte)enumBanTerm,
                    BanReason = "Suspicious activity - Attempting to change a user role in admin-only user role controls",
                    BanDate = banDate,
                    BanLiftDate = enumBanTerm.DateTimeOffset(banDate)
                });
            }
            else if (roleId == user.Role) // New role is the same as the old role: cancel and add error
                errorCount++;
            else if (user.UserName == Request.HttpContext.User.Identity.Name) // User is attempting to change their own role: cancel and add error
            {
                errorCount++;
                errorMessage = "You may not change your own role!";
            }
            else if (Request.HttpContext.User.Identity.Name != "admin@codesynergy.com") // User is attempting to change the main administrator's role: cancel and add error
            {
                errorCount++;
                errorMessage = "You may not change the role of the main administrator!";
            } else // No errors: change the user role
            {
                user.Role = roleId;
                await _users.UpdateAsync(user);
            }

            return Json(new object[] { errorCount, errorMessage });
        }
    }
}
