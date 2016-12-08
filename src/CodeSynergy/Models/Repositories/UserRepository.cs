using CodeSynergy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class UserRepository : UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string>
    {
        public UserRepository(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }

        public bool HasActiveBan(BanRepository bans, ApplicationUser user)
        {
            return bans.GetAllForUser(user).Any();
        }

        public Ban GetActiveBan(BanRepository bans, ApplicationUser user)
        {
            return bans.GetAllForUser(user).SingleOrDefault();
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts)
                .ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag)
                .AsEnumerable();
        }

        public ApplicationUser FindByEmail(string email)
        {
            ThrowIfDisposed();
            return Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts)
                .ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag)
                .FirstOrDefault(u => u.Email == email);
        }

        public ApplicationUser FindByDisplayName(string displayName)
        {
            ThrowIfDisposed();
            return Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts)
                .ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag)
                .FirstOrDefault(u => u.DisplayName.Replace(" ", "_") == displayName.Replace(" ", "_"));
        }

        public async Task<ApplicationUser> FindByDisplayNameAsync(string displayName)
        {
            ThrowIfDisposed();
            return await Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts)
                .ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag)
                .FirstOrDefaultAsync(u => u.DisplayName.Replace(" ", "_") == displayName.Replace(" ", "_"));
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return await Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts).ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();
            return await Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts).ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return await Users.Include(u => u.Roles).Include(u => u.Country).Include(u => u.Region).Include(u => u.QAPosts).ThenInclude(p => p.Question).ThenInclude(q => q.QuestionTags).Include(u => u.QAPosts).ThenInclude(p => p.Comments).Include(u => u.Comments).ThenInclude(c => c.Question).ThenInclude(q => q.Posts).Include(u => u.Stars).Include(u => u.UserTags).ThenInclude(ut => ut.Tag).FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}