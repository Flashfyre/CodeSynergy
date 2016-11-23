using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class SignInManager : SignInManager<ApplicationUser>
    {
        UserManager<ApplicationUser> _userManager;

        public SignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            _userManager = userManager;
        }

        public override async Task SignInAsync(ApplicationUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            user.Online = true;
            await _userManager.UpdateAsync(user);

            await base.SignInAsync(user, authenticationProperties, authenticationMethod);
        }
        
        public async Task SignOutAsync(ApplicationUser user)
        {
            user.Online = false;
            await _userManager.UpdateAsync(user);

            await base.SignOutAsync();
        }

        public async new Task<bool> IsSignedIn(ClaimsPrincipal principal)
        {
            bool isSignedIn = base.IsSignedIn(principal);

            if (isSignedIn)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(principal.Identity.Name);
                if (!user.Online)
                {
                    user.Online = true;
                    await _userManager.UpdateAsync(user);
                }
            }

            return isSignedIn;
        }
    }
}
