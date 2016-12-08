using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeSynergy.Models;
using CodeSynergy.Services;
using CodeSynergy.Models.AccountViewModels;
using CodeSynergy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Http;

namespace CodeSynergy.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager _userManager; // Manages ApplicationUser objects
        private readonly SignInManager _signInManager; // Manages authentication
        private readonly BanRepository _bans; // Repository for user bans
        private readonly IEmailSender _emailSender; // Sends emails (unused)
        private readonly ISmsSender _smsSender; // Sends SMS messages (unused)
        private readonly ILogger _logger; // Logs authentication-related actions

        // Constructor
        public AccountController(
            UserManager userManager,
            SignInManager signInManager,
            IRepository<Ban, int> bans,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _bans = (BanRepository) bans;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // Login page loaded
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Add the return URL to ViewData

            // Add any login errors found in cookies to ViewData and delete those cookies
            if (Request.Cookies.Any(c => c.Key == "LoginError"))
            {
                ViewData["LoginError"] = Request.Cookies["LoginError"];
                Response.Cookies.Delete("LoginError");
            }

            return View();
        }

        // Login form submitted
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Add the return URL to ViewData
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email.ToLower(), model.Password, model.KeepLoggedIn, lockoutOnFailure: false);
                Ban activeBan = null;

                if (result.Succeeded)
                {
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                        activeBan = _bans.GetAllForUser(user).SingleOrDefault(b => b.Active);
                        if (activeBan != null)
                        {
                            Response.Cookies.Append("LoginError", "You may not sign into this account as it is " + (activeBan.BanTerm != (int) EnumBanTerm.Perm ? "banned until " + string.Format("{0:f}", activeBan.BanLiftDate) :
                                "permanently banned") + ".<br /><br />Reason for ban: " + activeBan.BanReason, new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now).AddMinutes(5d) });
                            await _signInManager.SignOutAsync(activeBan.BannedUser);
                        } else
                        {
                            _logger.LogInformation(1, "User logged in.");
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }

                if (returnUrl != null && returnUrl.IndexOf("modal=") == -1)
                    returnUrl += (returnUrl.LastIndexOf("?") == -1 ? "?" : "&") + "modal=Account/Login?returnUrl=" + returnUrl;

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.KeepLoggedIn });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else if (activeBan == null)
                {
                    Response.Cookies.Append("LoginError", "Invalid login attempt.", new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now).AddMinutes(5d) });
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToLocal(returnUrl);
        }

        // Register page loaded
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Countries = GetCountriesList(); // Add a list of countries to the ViewBag
            return View();
        }

        // Get a list of Countries from the database
        public List<Country> GetCountriesList()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Countries.ToList();
            }
        }

        // Get a list of Regions from the database
        // POST: /Account/GetRegions
        [HttpPost]
        [AllowAnonymous]
        public List<Region> GetRegionsList(string CountryID)
        {
            using (var context = new ApplicationDbContext())
            {
                // Get regions that belong to the Country with the provided Country ID
                return context.Regions.Where(r => r.ISO == CountryID).ToList();
            }
        }

        // Get Region data from the database as a JSON array
        // POST: /Account/GetRegions
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetRegions(string CountryID)
        {
            using (var context = new ApplicationDbContext())
            {
                // Get region data for regions that belong to the Country with the provided Country ID
                string[][] regionData = context.Regions.Where(x => x.ISO == CountryID).Select(x => new string[] { x.RegionID, x.RegionName }).ToArray();
                return Json(regionData);
            }
        }

        // Register form submitted
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) // If there are no form errors
            {
                // Create a new ApplicationUser
                var user = new ApplicationUser() {
                    UserName = model.Email.ToLower(), Email = model.Email.ToLower(), DisplayName = model.DisplayName.Replace(' ', '_'), FirstName = model.FirstName, LastName = model.LastName, JobTitle = model.JobTitle, BirthDate = model.BirthDate, Gender = model.Gender,
                    CountryID = model.CountryID, RegionID = model.RegionID, City = model.City, GitHubID = model.GitHubID, ProfileGitHub = model.ProfileGitHub, ProfileMessage = "<p>Welcome to my CodeSynergy profile!</p>", ExcludeFromRanking = false, UseProfileBackground = false,
                    UseSearchGrid = false, RegistrationDate = DateTime.Now, LastActivityDate = DateTime.Now, QuestionsPosted = 0, AnswersPosted = 0, CommentsPosted = 0, ProfileViewCount = 0, Reputation = 0, Online = true, Role = "Member"
                };
                // Create a new user with the provided ApplicationUser object and password
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) // If the user was created successfully
                {
                    if (user.Email == "admin@codesynergy.com") // Make the user an administrator if their email is admin@codesynergy.com (email uniqueness is checked so this is safe)
                        await _userManager.AddToRoleAsync(user, "Administrator");
                    else // Add the member role if the user email does not match the admin email
                        await _userManager.AddToRoleAsync(user, "Member");
                    await _signInManager.SignInAsync(user, isPersistent: false); // Sign the user in
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home"); // Redirect to the home page
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // Logout button clicked
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync(await _userManager.FindByEmailAsync(Request.HttpContext.User.Identity.Name)); // Sign the user out
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // External login (unused)
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // External login callback function (unused)
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        // External login confirmation (unused)
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl; // Add the return URL to ViewData
            return View(model);
        }

        // Settings page loaded
        // GET: /Account/Settings
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            ViewBag.Countries = GetCountriesList(); // Add a list of Countries to the ViewBag
            ViewBag.Regions = user.CountryID == null ? new List<Region>() : GetRegionsList(user.CountryID); // Add a list of Regions or an empty list (depending on whether the user already has a Country) to the ViewBag
            return View(new SettingsViewModel(user));
        }

        // Settings form submitted
        // POST: /Account/UpdateSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(SettingsViewModel model)
        {
            if (ModelState.IsValid) // If there are no form errors
            {
                ApplicationUser user = null;
                IdentityResult result = null;
                
                user = await GetCurrentUserAsync();
                // Update the user with new data provided in the form
                user.DisplayName = model.DisplayName.Replace(' ', '_');
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.JobTitle = model.JobTitle;
                user.BirthDate = model.BirthDate;
                user.Gender = model.Gender;
                user.CountryID = model.CountryID;
                user.RegionID = model.RegionID;
                user.City = model.City;
                user.GitHubID = model.GitHubID;
                user.ProfileGitHub = model.ProfileGitHub;
                user.ExcludeFromRanking = model.ExcludeFromRanking;
                user.UseProfileBackground = model.UseProfileBackground;
                user.UseSearchGrid = model.UseSearchGrid;

                if (model.NewPassword != null) // Change the user's password if necessary
                {
                    result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                }

                if (result == null || result.Succeeded) // If succeeded, save the changes to the database and redirect to the home page
                {
                    result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                AddErrors(result);
            }

            // If we got this far, something failed, load the home page with the settings modal
            return Redirect("/?modal=Account/Settings");
        }

        // Return a JSON array for the result of whether an email is taken by another user
        // GET: /Account/ValidateUniqueEmail
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateUniqueEmail(string Email)
        {
            return Json(!_userManager.Users.Any(u => u.Email == Email) ?
                    "true" : string.Format("That email '{0}' is already in use. Please enter an email address that isn't already connected with a CodeSynergy account.", Email));
        }

        // Return a JSON array for the result of whether a display name is taken by another user
        // GET: /Account/ValidateUniqueDisplayName
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateUniqueDisplayName(string DisplayName)
        {
            if (DisplayName != null)
                DisplayName = DisplayName.Replace(' ', '_');
            return Json(!_userManager.Users.Any(u => u.DisplayName == DisplayName && u.Email != Request.HttpContext.User.Identity.Name) ?
                    "true" : string.Format("The display name '{0}' is already in use. Please enter a unique display name.", DisplayName));
        }

        // Return a JSON array for the result of whether a display name is used by an existing user
        // GET: /Account/ValidateExistingDisplayName
        [HttpGet]
        public IActionResult ValidateExistingDisplayName(string DisplayName)
        {
            if (DisplayName != null)
                DisplayName = DisplayName.Replace(' ', '_');
            return Json(_userManager.Users.Any(u => u.DisplayName == DisplayName) ?
                    "true" : string.Format("There are no users with the display name '{0}'. Please enter a valid display name.", DisplayName));
        }

        // Confirm a code from an email link (unused)
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // Forgot password page loaded (unused)
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot password form submitted (unused)
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // Forgot password confirmeation (unused)
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Reset password page loaded (unused)
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        // Reset password form submitted (unused)
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        // Reset password confirmation (unused)
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // Send code page loaded
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // Send code form submitted (unused)
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        // Verify code page loaded (unused)
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // Verify code form submitted (unused)
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        #region Helpers

        // Add errors to a viewmodel to display on the form after refresh
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Get the ApplicationUser object for the logged in user
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // Validate that a URL is part of the site and redirect to home if it isn't
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
