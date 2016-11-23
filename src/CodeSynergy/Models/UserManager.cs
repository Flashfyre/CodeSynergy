using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class UserManager : UserManager<ApplicationUser>
    {
        BanRepository _bans;

        public UserManager(IUserStore<ApplicationUser> users, IRepository<Ban, int> bans, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(users, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _bans = (BanRepository) bans;
        }

        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return (Store as UserRepository).FindByEmailAsync(email);
        }

        public bool HasActiveBan(ApplicationUser user)
        {
            return (Store as UserRepository).HasActiveBan(_bans, user);
        }

        public Ban GetActiveBan(ApplicationUser user)
        {
            return (Store as UserRepository).GetActiveBan(_bans, user);
        }
    }
}
