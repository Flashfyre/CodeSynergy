using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class MailboxRepository : IJoinTableRepository<UserMailbox, string, byte>
    {
        ApplicationDbContext context;

        public MailboxRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<UserMailbox> GetAll()
        {
            return context.UserMailboxes.Include(m => m.UserItems).ThenInclude(i => i.PrivateMessage).ThenInclude(pm => pm.SenderUser).ThenInclude(u => u.Roles).Include(m => m.UserItems).ThenInclude(i => i.User).AsEnumerable();
        }

        public IEnumerable<UserMailbox> GetAllForUser(ApplicationUser user)
        {
            return GetAllForUser(user.Id);
        }

        public IEnumerable<UserMailbox> GetAllForUser(string userID)
        {
            return context.UserMailboxes.Where(b => b.UserID == userID).Include(m => m.UserItems).ThenInclude(i => i.PrivateMessage).ThenInclude(pm => pm.SenderUser).ThenInclude(u => u.Roles).Include(m => m.UserItems).ThenInclude(i => i.User).AsEnumerable();
        }

        public void Add(UserMailbox item)
        {
            throw new NotImplementedException();
        }

        public UserMailbox Find(string idA, byte idB)
        {
            return context.UserMailboxes.Include(m => m.UserItems).ThenInclude(i => i.PrivateMessage).ThenInclude(pm => pm.SenderUser).ThenInclude(u => u.Roles).Include(m => m.UserItems).ThenInclude(i => i.User).SingleOrDefault(t => t.UserID == idA && t.MailboxTypeID == idB);
        }
        
        public bool Remove(UserMailbox MailboxIn)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string idA, byte idB)
        {
            return Remove(Find(idA, idB));
        }

        public void Update(UserMailbox MailboxIn)
        {
            context.UserMailboxes.Update(MailboxIn);
            context.SaveChanges();
        }
    }
}