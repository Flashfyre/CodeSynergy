using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class RankingPosRepository : IRepository<RankingPos, short>
    {
        ApplicationDbContext context;

        public RankingPosRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<RankingPos> GetAll()
        {
            return context.RankingPos.Include(p => p.Rankings).ThenInclude(r => r.User).ThenInclude(u => u.Roles).Where(p => p.Rankings.Any()).AsEnumerable();
        }

        public void Add(RankingPos item)
        {
            context.RankingPos.Add(item);
            context.SaveChanges();
        }

        public RankingPos Find(short id)
        {
            return context.RankingPos.SingleOrDefault(t => t.RankingPosID == id);
        }

        public bool Remove(RankingPos RankingPos)
        {
            bool successful = context.RankingPos.Remove(RankingPos) != null;

            if (successful)
            {
                context.SaveChanges();
            }

            return successful;
        }

        public bool Remove(short id)
        {
            return Remove(Find(id));
        }

        public void Update(RankingPos RankingPos)
        {
            context.RankingPos.Update(RankingPos);
            context.SaveChanges();
        }
    }
}