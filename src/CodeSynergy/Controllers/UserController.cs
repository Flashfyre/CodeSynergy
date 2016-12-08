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
        private ApplicationDbContext _context; // Database context
        private readonly UserRepository _users; // Repository for users

        public UserController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users) : base() {
            _context = new ApplicationDbContext();
            _users = (UserRepository) users;
        }

        // User profile loaded
        // GET: /User/DisplayName
        [HttpGet]
        public async Task<IActionResult> Index(string DisplayName, string Modal)
        {
            ApplicationUser user;
            string loggedInEmail = Request.HttpContext.User.Identity.Name;
            if (DisplayName != null) // If the display name is included in the URL
            {
                user = await _users.FindByDisplayNameAsync(DisplayName.Replace(' ', '_'));
                if (user == null) // No user exists with that display name: display error
                    return Redirect("/Home/Error");
                else if (loggedInEmail != user.Email) // User exists: add a profile view
                {
                    user.ProfileViewCount++;
                    await _users.UpdateAsync(user);
                }
                // Ensure the user's Country and Region objects have been loaded (could be null otherwise)
                _context.Countries.SingleOrDefault(c => c.ISO == user.CountryID);
                _context.Regions.SingleOrDefault(r => r.RegionID == user.RegionID);
                ViewBag.RepVote = user.GetVote(_context, loggedInEmail);
                ViewBag.Rankings = _context.Rankings.Where(r => r.UserID == user.Id);
                ViewData["Modal"] = Modal;
            }
            else if (loggedInEmail != null) // Display name is not included in the URL and user is logged in: display their own profile
                return Redirect("/User/" + WebUtility.UrlEncode((await _users.FindByEmailAsync(loggedInEmail)).DisplayName) + Request.QueryString.Value);
            else // Display name is not included in the URL and no user is logged in: display error page
                return Redirect("/Home/Error");

            return View(new ProfileViewModel(user));
        }

        // User profile POST request sent
        // POST: /User/DisplayName
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            if (ModelState.IsValid) // Request is valid
            {
                user.LastActivityDate = DateTime.Now;
                if (!model.IsImage) // Request is not an image upload (and therefore is a profile message edit): update user's profile message
                {
                    user.ProfileMessage = model.Contents;
                    await _users.UpdateAsync(user);
                } else // Request is an image uploaded
                {
                    bool isBackground = model.IsBackgroundImage != null && (bool) model.IsBackgroundImage; // Whether the upload is for a background
                    if ((!isBackground && model.ProfileImage == null) || (isBackground && model.BackgroundImage == null)) // No file was provided
                    {
                        if (System.IO.File.Exists("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png")) // Delete the existing image if one exists
                        {
                            System.IO.File.Delete("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png");
                        }
                    }
                    else if ((!isBackground && model.ProfileImage.Length > 0) || (isBackground && model.BackgroundImage.Length > 0)) // A valid image file was provided
                    {
                        string imageName = !isBackground ? model.ProfileImage.FileName : model.BackgroundImage.FileName;
                        if (imageName.LastIndexOf(".") > -1) // The file has an extension
                        {
                            string extension = imageName.Substring(imageName.LastIndexOf(".")); // The file's extension
                            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif" || extension == ".bmp") // The extension is a supported image extension
                            {
                                Image image = new Image((!isBackground ? model.ProfileImage : model.BackgroundImage).OpenReadStream()); // Create a new image instance
                                // Create a file for the user's profile picture or background
                                using (FileStream fileStream = new FileStream("wwwroot/images/user/" + (isBackground ? "background" : "profile") + "/" + user.Id + ".png", FileMode.Create))
                                {
                                    if (!isBackground && image.Width != image.Height) // If image is a profile picture that is not square
                                    {
                                        if (model.ResizeMethod == ProfileViewModel.EnumResizeMethod.Stretch) // Resize method is stretch: stretch the image to a square and save it
                                        {
                                            int smallerDim = image.Width < image.Height ? image.Width : image.Height;
                                            image.Resize(smallerDim, smallerDim).SaveAsPng(fileStream);
                                        }
                                        else // Resize method is crop: crop the image to a square and save it
                                        {
                                            if (image.Width < image.Height)
                                                image.Crop(image.Width, image.Width, new Rectangle(image.Bounds.Left, image.Bounds.Top + ((image.Height - image.Width) / 2), image.Width, image.Width)).SaveAsPng(fileStream);
                                            else
                                                image.Crop(image.Height, image.Height, new Rectangle(image.Bounds.Left + ((image.Width - image.Height) / 2), image.Bounds.Top, image.Height, image.Height)).SaveAsPng(fileStream);
                                        }
                                    }
                                    else // The image is already square: just save it
                                        image.SaveAsPng(fileStream);
                                    await _users.UpdateAsync(user);
                                }
                            }
                        }
                    }
                }

                return Redirect("/User/" + WebUtility.UrlEncode((await _users.FindByEmailAsync(user.Email)).DisplayName) + Request.QueryString.Value); // Reload the page
            }

            model.User = user;
            _context.Countries.SingleOrDefault(c => c.ISO == user.CountryID);
            _context.Regions.SingleOrDefault(r => r.RegionID == user.RegionID);
            ViewBag.RepVote = null;
            ViewBag.Rankings = _context.Rankings.Where(r => r.UserID == user.Id);

            return View(model);
        }

        // Rep vote POST request sent
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

            if (voteeUser.Id != voterUser.Id) // User is not voting on their own reputation
            {
                voterUser.LastActivityDate = DateTime.Now;
                if (repVote != null) // A vote already exists for this voter and votee
                {
                    if (repVote.Vote != vote) // The vote is in the opposite direction: change the vote and update it
                    {
                        repVote.Vote = vote;
                        await voteeUser.UpdateVote(_context, _users, repVote);
                    }
                    else // The vote is the same vote: remove the vote
                        await voteeUser.RemoveVote(_context, _users, repVote);
                }
                else // No vote exists for this voter and votee: add the vote
                    await voteeUser.AddVote(_context, _users, voterUser, vote);
                success = true;
            }
            else // User is attempting to vote on their own reputation: set error message
                errorMessage = "You cannot vote on your own reputation!";

            return Json(new object[] { success, success ? voteeUser.Reputation.ToString() : errorMessage });
        }

        // User posts grid loaded
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

        // User tags grid loaded
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
