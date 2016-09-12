using CodeSynergy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeSynergy.Models.Repositories
{
    public class UserRepository : IKeyValueRepository<ApplicationUser, string>
    {
        ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            List<ApplicationUser> usersList = context.Users.ToList();
            List<IdentityUserRole<string>> rolesList = context.UserRoles.ToList();

            foreach (ApplicationUser user in usersList)
            {
                if (rolesList.SingleOrDefault(r => r.UserId == user.Id) == null)
                {
                    ((ApplicationUser)context.Users.Single(u => u.Id == user.Id)).Role = "Member";
                }
            }

            if (rolesList.Count < usersList.Count)
            {
                context.SaveChanges();
            }
        }

        public void Add(string key, ApplicationUser val)
        {
            context.Users.Add(val);
            context.SaveChanges();
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return context.Users.AsEnumerable();
        }

        public ApplicationUser Find(string key)
        {
            ApplicationUser user = context.Users.Single(u => u.Id == key);
            return user;
        }

        public ApplicationUser FindByDisplayName(string displayName)
        {
            ApplicationUser user = context.Users.SingleOrDefault(u => u.DisplayName.ToLower() == displayName.ToLower());
            return user;
        }

        public ApplicationUser FindByUserName(string userName)
        {
            ApplicationUser user = context.Users.SingleOrDefault(u => u.UserName == userName);
            return user;
        }

        public bool Remove(string key)
        {
            bool successful = context.Users.Remove(Find(key)) != null;
            
            if (successful) {
                context.SaveChanges();
            }

            return successful;
        }

        public void Update(ApplicationUser val)
        {
            context.Users.Update(val);
            context.SaveChanges();
        }
    }
}