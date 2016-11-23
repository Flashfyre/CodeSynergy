using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSynergy.Models.Repositories
{
    public class ReportRepository : IRepository<Report, int>
    {
        ApplicationDbContext context;

        public ReportRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Report> GetAll()
        {
            return context.Reports.AsEnumerable();
        }

        public void Add(Report item)
        {
            context.Reports.Add(item);
            context.SaveChanges();
        }

        public Report Find(int id)
        {
            return context.Reports.SingleOrDefault(t => t.ReportID == id);
        }

        public bool Remove(Report ReportIn)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            return Remove(Find(id));
        }

        public void Update(Report ReportIn)
        {
            context.Reports.Update(ReportIn);
            context.SaveChanges();
        }
    }
}