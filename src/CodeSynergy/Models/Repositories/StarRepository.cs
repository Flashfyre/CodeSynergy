using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class StarRepository : IJoinTableRepository<Star, string, int>
    {
        ApplicationDbContext context;

        public StarRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Star> GetAll()
        {
            return context.Stars.AsEnumerable();
        }

        public IEnumerable<Star> GetAllForUser(ApplicationUser user)
        {
            return context.Stars.Where(s => s.UserID == user.Id).Include(s => s.Question).ThenInclude(q => q.Posts).ThenInclude(p => p.Comments).ThenInclude(c => c.User).ThenInclude(u => u.Roles)
                .Include(s => s.Question).ThenInclude(q => q.Posts).ThenInclude(p => p.User).AsEnumerable();
        }

        public void Add(Star item)
        {
            context.Stars.Add(item);
            context.SaveChanges();
        }

        public Star Find(string idA, int idB)
        {
            return context.Stars.SingleOrDefault(s => s.UserID == idA && s.QuestionID == idB);
        }

        public bool Remove(Star starIn)
        {
            bool successful = context.Stars.Remove(starIn) != null;

            if (successful)
            {
                context.SaveChanges();
            }

            return successful;
        }

        public bool Remove(string idA, int idB)
        {
            return Remove(Find(idA, idB));
        }

        public void Update(Star starIn)
        {
            context.Stars.Update(starIn);
            context.SaveChanges();
        }
    }
}