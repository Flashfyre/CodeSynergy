using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class TagRepository : IRepository<Tag, int>
    {
        ApplicationDbContext context;

        public TagRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Tag> GetAll()
        {
            return context.Tags.AsEnumerable();
        }

        public void Add(Tag item)
        {
            context.Add(item);
            context.SaveChanges();
        }

        public Tag Find(int id)
        {
            return context.Tags.SingleOrDefault(t => t.TagID == id);
        }

        public Tag Find(string tagName)
        {
            return context.Tags.SingleOrDefault(t => t.TagName.ToLower() == tagName);
        }

        public bool Remove(Tag tagIn)
        {
            bool successful = context.Tags.Remove(tagIn) != null;

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

        public void Update(Tag tagIn)
        {
            context.Tags.Update(tagIn);
            context.SaveChanges();
        }
    }
}