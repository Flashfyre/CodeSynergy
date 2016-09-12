﻿using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class UntrustedURLPatternRepository : IRepository<UntrustedURLPattern, int>
    {
        ApplicationDbContext context;

        public UntrustedURLPatternRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<UntrustedURLPattern> GetAll()
        {
            return context.UntrustedURLPatterns.AsEnumerable();
        }

        public void Add(UntrustedURLPattern item)
        {
            context.UntrustedURLPatterns.Add(item);
            context.SaveChanges();
        }

        public UntrustedURLPattern Find(int id)
        {
            return context.UntrustedURLPatterns.SingleOrDefault(t => t.PatternID == id);
        }

        public bool Remove(UntrustedURLPattern UntrustedURLPatternIn)
        {
            bool successful = context.UntrustedURLPatterns.Remove(UntrustedURLPatternIn) != null;

            if (successful)
            {
                context.SaveChanges();
            }

            return successful;
        }

        public bool Remove(int id)
        {
            return Remove(Find(id));
        }

        public void Update(UntrustedURLPattern UntrustedURLPatternIn)
        {
            context.UntrustedURLPatterns.Update(UntrustedURLPatternIn);
            context.SaveChanges();
        }
    }
}