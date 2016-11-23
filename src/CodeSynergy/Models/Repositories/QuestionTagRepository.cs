using CodeSynergy.Data;
using System.Collections.Generic;
using System.Linq;

namespace CodeSynergy.Models.Repositories
{
    public class QuestionTagRepository : IJoinTableRepository<QuestionTag, int, int>
    {
        ApplicationDbContext context;

        public QuestionTagRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<QuestionTag> GetAll()
        {
            return context.QuestionTags.AsEnumerable();
        }

        public void Add(QuestionTag item)
        {
            context.QuestionTags.Add(item);
            context.SaveChanges();
        }

        public QuestionTag Find(int idA, int idB)
        {
            return context.QuestionTags.SingleOrDefault(t => t.QuestionID == idA && t.TagID == idB);
        }

        public bool Remove(QuestionTag qtIn)
        {
            bool successful = context.QuestionTags.Remove(qtIn) != null;

            if (successful)
            {
                context.SaveChanges();
            }

            return successful;
        }

        public bool Remove(int idA, int idB)
        {
            return Remove(Find(idA, idB));
        }

        public void Update(QuestionTag qtIn)
        {
            context.QuestionTags.Update(qtIn);
            context.SaveChanges();
        }
    }
}