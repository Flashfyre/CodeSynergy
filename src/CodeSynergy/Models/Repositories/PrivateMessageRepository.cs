using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class PrivateMessageRepository : IRepository<PrivateMessage, long>
    {
        ApplicationDbContext context;

        public PrivateMessageRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<PrivateMessage> GetAll()
        {
            return context.PrivateMessages.Include(pm => pm.SenderUser).ThenInclude(u => u.Roles).Include(pm => pm.RecipientUser).AsEnumerable();
        }

        public void Add(PrivateMessage item)
        {
            context.PrivateMessages.Add(item);
            context.SaveChanges();
        }

        public PrivateMessage Find(long id)
        {
            return context.PrivateMessages.Include(pm => pm.SenderUser).ThenInclude(u => u.Roles).Include(pm => pm.RecipientUser).SingleOrDefault(t => t.PrivateMessageID == id);
        }

        public bool Remove(PrivateMessage PrivateMessageIn)
        {
            PrivateMessageIn.Contents = "[DELETED]";
            PrivateMessageIn.Summary = "[DELETED]";
            PrivateMessageIn.DeletedDate = DateTime.Now;

            context.SaveChanges();

            return true;
        }

        public bool Remove(long id)
        {
            return Remove(Find(id));
        }

        public void Update(PrivateMessage PrivateMessageIn)
        {
            context.PrivateMessages.Update(PrivateMessageIn);
            context.SaveChanges();
        }
    }
}