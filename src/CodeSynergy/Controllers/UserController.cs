using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using CodeSynergy.Models.UserViewModels;
using System.IO;
using ImageProcessorCore;
using System.Net;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CodeSynergy.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserRepository _users;

        public UserController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users) : base() {
            _context = new ApplicationDbContext();
            _users = (UserRepository) users;
        }

        // GET: /User/DisplayName
        [HttpGet]
        public async Task<IActionResult> Index(string DisplayName, string Modal)
        {
            ApplicationUser user;
            string loggedInEmail = Request.HttpContext.User.Identity.Name;
            if (DisplayName != null)
            {
                user = await _users.FindByDisplayNameAsync(DisplayName.Replace(' ', '_'));
                if (user == null)
                    return Redirect("/Home/Error");
                else if (loggedInEmail != user.Email)
                {
                    user.ProfileViewCount++;
                    await _users.UpdateAsync(user);
                }
                _context.Countries.SingleOrDefault(c => c.ISO == user.CountryID);
                _context.Regions.SingleOrDefault(r => r.RegionID == user.RegionID);
                ViewBag.RepVote = user.GetVote(_context, loggedInEmail);
                ViewBag.Rankings = _context.Rankings.Where(r => r.UserID == user.Id);
                ViewData["Modal"] = Modal;
            }
            else if (loggedInEmail != null)
                return Redirect("/User/" + WebUtility.UrlEncode((await _users.FindByEmailAsync(loggedInEmail)).DisplayName) + Request.QueryString.Value);
            else
                return Redirect("/Home/Error");

            return View(new ProfileViewModel(user));
        }

        // POST: /User/DisplayName
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            if (ModelState.IsValid)
            {
                user.LastActivityDate = DateTime.Now;
                if (!model.IsImage)
                {
                    user.ProfileMessage = model.Contents;
                    await _users.UpdateAsync(user);
                } else
                {
                    bool isBackground = model.IsBackgroundImage != null && (bool) model.IsBackgroundImage;
                    if ((!isBackground && model.ProfileImage == null) || (isBackground && model.BackgroundImage == null))
                    {
                        if (System.IO.File.Exists("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png"))
                        {
                            System.IO.File.Delete("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png");
                        }
                    }
                    else if ((!isBackground && model.ProfileImage.Length > 0) || (isBackground && model.BackgroundImage.Length > 0))
                    {
                        string imageName = !isBackground ? model.ProfileImage.FileName : model.BackgroundImage.FileName;
                        if (imageName.LastIndexOf(".") > -1)
                        {
                            string extension = imageName.Substring(imageName.LastIndexOf("."));
                            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif" || extension == ".bmp")
                            {
                                Image image = new Image((!isBackground ? model.ProfileImage : model.BackgroundImage).OpenReadStream());
                                using (FileStream fileStream = new FileStream("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png", FileMode.Create))
                                {
                                    if (!isBackground && image.Width != image.Height)
                                    {
                                        if (model.ResizeMethod == ProfileViewModel.EnumResizeMethod.Stretch)
                                        {
                                            int smallerDim = image.Width < image.Height ? image.Width : image.Height;
                                            image.Resize(smallerDim, smallerDim).SaveAsPng(fileStream);
                                        }
                                        else
                                        {
                                            if (image.Width < image.Height)
                                                image.Crop(image.Width, image.Width, new Rectangle(image.Bounds.Left, image.Bounds.Top + ((image.Height - image.Width) / 2), image.Width, image.Width)).SaveAsPng(fileStream);
                                            else
                                                image.Crop(image.Height, image.Height, new Rectangle(image.Bounds.Left + ((image.Width - image.Height) / 2), image.Bounds.Top, image.Height, image.Height)).SaveAsPng(fileStream);
                                        }
                                    }
                                    else
                                        image.SaveAsPng(fileStream);
                                    await _users.UpdateAsync(user);
                                }
                            }
                        }
                    }
                }

                return Redirect("/User/" + WebUtility.UrlEncode((await _users.FindByEmailAsync(user.Email)).DisplayName) + Request.QueryString.Value);
            }

            model.User = user;
            _context.Countries.SingleOrDefault(c => c.ISO == user.CountryID);
            _context.Regions.SingleOrDefault(r => r.RegionID == user.RegionID);
            ViewBag.RepVote = null;
            ViewBag.Rankings = _context.Rankings.Where(r => r.UserID == user.Id);

            return View(model);
        }

        //
        // POST: /User/RepVote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RepVote(string displayName, bool vote)
        {
            ApplicationUser voteeUser = await _users.FindByDisplayNameAsync(displayName);
            ApplicationUser voterUser = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            RepVote repVote = voteeUser.GetVote(_context, voterUser.Email);
            bool success = false;
            string errorMessage = "";

            if (voteeUser.Id != voterUser.Id)
            {
                voterUser.LastActivityDate = DateTime.Now;
                if (repVote != null)
                {
                    if (repVote.Vote != vote)
                    {
                        repVote.Vote = vote;
                        voteeUser.UpdateVote(_context, _users, repVote);
                    }
                    else
                    {
                        voteeUser.RemoveVote(_context, _users, repVote);
                    }
                }
                else
                {
                    voteeUser.AddVote(_context, _users, voterUser, vote);
                }
                success = true;
            }
            else
                errorMessage = "You cannot vote on your own reputation!";

            return Json(new object[] { success, success ? voteeUser.Reputation.ToString() : errorMessage });
        }

        // GET: /User/UserPostGrid
        [HttpGet]
        public async Task<IActionResult> UserPostGrid(string UserDisplayName, string ColumnIndex = "-1", string SortAsc = "false")
        {
            ApplicationUser user = await _users.FindByDisplayNameAsync(UserDisplayName);
            int columnIndex = -1;
            bool sortAsc = false;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;

            return PartialView("MvcGrid/_UserPostGrid", user.Posts);
        }

        // GET: /User/UserTagGrid
        [HttpGet]
        public async Task<IActionResult> UserTagGrid(string UserDisplayName, string ColumnIndex = "-1", string SortAsc = "false")
        {
            ApplicationUser user = await _users.FindByDisplayNameAsync(UserDisplayName);
            int columnIndex = -1;
            bool sortAsc = false;
            int.TryParse(ColumnIndex, out columnIndex);
            bool.TryParse(SortAsc, out sortAsc);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;

            return PartialView("MvcGrid/_UserTagGrid", user.UserTags);
        }
    }
}
