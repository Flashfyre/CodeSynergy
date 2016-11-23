using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class ModerationMailbox : Mailbox, IRepository<ModerationMailboxItem, int>
    {
        ApplicationDbContext context;
        public override IEnumerable<MailboxItem> Items {
            get
            {
                return GetAll().Where(i => i.DeletedDate == null);
            }
        }

        public ModerationMailbox(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<ModerationMailboxItem> GetAll()
        {
            return context.ModerationMailboxItems.Include(m => m.AssigneeUser).ThenInclude(u => u.Roles).Include(m => m.Report).ThenInclude(m => m.SenderUser).ThenInclude(u => u.Roles).Include(m => m.Report)
                .ThenInclude(r => r.ReportedQuestion).ThenInclude(q => q.Posts).ThenInclude(p => p.User).Include(m => m.Report).ThenInclude(r => r.ReportedAnswer).Include(m => m.Report).ThenInclude(r => r.ReportedComment)
                .ThenInclude(c => c.User).Include(m => m.Report).ThenInclude(r => r.ReportedUser).Include(m => m.Report).ThenInclude(r => r.ReportedPrivateMessage).ThenInclude(pm => pm.SenderUser).AsEnumerable();
        }

        public void Add(ModerationMailboxItem itemIn)
        {
            context.ModerationMailboxItems.Add(itemIn);
            context.SaveChanges();
        }

        public ModerationMailboxItem Find(int id)
        {
            return context.ModerationMailboxItems.Include(m => m.AssigneeUser).ThenInclude(u => u.Roles).Include(m => m.Report).ThenInclude(m => m.SenderUser).ThenInclude(u => u.Roles).Include(m => m.Report)
                .ThenInclude(r => r.ReportedQuestion).ThenInclude(q => q.Posts).ThenInclude(p => p.User).Include(m => m.Report).ThenInclude(r => r.ReportedAnswer).Include(m => m.Report).ThenInclude(r => r.ReportedComment)
                .ThenInclude(c => c.User).Include(m => m.Report).ThenInclude(r => r.ReportedUser).Include(m => m.Report).ThenInclude(r => r.ReportedPrivateMessage).ThenInclude(pm => pm.SenderUser).SingleOrDefault(q => q.MailboxItemID == id);
        }

        public bool Remove(ModerationMailboxItem itemIn)
        {
            itemIn.DeletedDate = DateTime.Now;

            context.SaveChanges();

            return true;
        }

        public bool Remove(int id)
        {
            return Remove(Find(id));
        }

        public void Update(ModerationMailboxItem itemIn)
        {
            context.ModerationMailboxItems.Update(itemIn);
            context.SaveChanges();
        }

        public override byte MailboxTypeID {
            get
            {
                return (byte) EnumMailboxType.Moderation;
            }
        }
    }
}