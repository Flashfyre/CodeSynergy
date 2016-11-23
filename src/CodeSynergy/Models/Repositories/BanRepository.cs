using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class BanRepository : IRepository<Ban, int>
    {
        ApplicationDbContext context;

        public BanRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Ban> GetAll()
        {
            return context.Bans.Include(b => b.BanningUser).Include(b => b.BannedUser).AsEnumerable();
        }

        public IEnumerable<Ban> GetAllForUser(ApplicationUser user, Boolean activeOnly = true)
        {
            return context.Bans.Where(b => b.BannedUserID == user.Id && (!activeOnly || b.Active)).Include(b => b.BanningUser).Include(b => b.BannedUser).ThenInclude(u => u.Roles).AsEnumerable();
        }

        public void Add(Ban item)
        {
            context.Bans.Add(item);
            context.SaveChanges();
        }

        public Ban Find(int id)
        {
            return context.Bans.Include(b => b.BanningUser).Include(b => b.BannedUser).ThenInclude(u => u.Roles).SingleOrDefault(t => t.BanID == id);
        }

        public Ban Find(string userDisplayName)
        {
            return context.Bans.Include(b => b.BanningUser).Include(b => b.BannedUser).ThenInclude(u => u.Roles).SingleOrDefault(t => t.BannedUser.DisplayName.ToLower() == userDisplayName.ToLower());
        }
        

        public bool Remove(Ban BanIn)
        {
            bool successful = context.Bans.Remove(BanIn) != null;

            if (successful)
                context.SaveChanges();

            return successful;
        }

        public bool Remove(int id)
        {
            return Remove(Find(id));
        }

        public void Update(Ban BanIn)
        {
            context.Bans.Update(BanIn);
            context.SaveChanges();
        }
    }
}