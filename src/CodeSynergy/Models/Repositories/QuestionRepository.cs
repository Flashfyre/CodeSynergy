using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class QuestionRepository : IRepository<Question, int>
    {
        ApplicationDbContext context;

        public QuestionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Question> GetAll()
        {
            return context.Questions.AsEnumerable();
        }

        public void Add(Question item)
        {
            context.Questions.Add(item);
            context.SaveChanges();
        }

        public Question Find(int id)
        {
            return context.Questions.SingleOrDefault(q => q.QuestionID == id);
        }

        public bool Remove(Question questionIn)
        {
            bool successful = context.Questions.Remove(questionIn) != null;

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

        public void Update(Question questionIn)
        {
            context.Questions.Update(questionIn);
            context.SaveChanges();
        }
    }
}