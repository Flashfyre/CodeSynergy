using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeSynergy.Data;

namespace CodeSynergy.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {

        }

        public ApplicationUser(string userName) : base(userName)
        {

        }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string DisplayName { get; set; }
        [Column(TypeName = "varchar(2)")]
        public String CountryID { get; set; }
        [ForeignKey("CountryID")]
        public Country Country { get; set; }
        [Column(TypeName = "varchar(4)")]
        public String RegionID { get; set; }
        [ForeignKey("RegionID")]
        public Region Region { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public String FirstName { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public String LastName { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public String JobTitle { get; set; }
        [Column(TypeName = "bit")]
        public bool? Gender { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public String City { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public String GitHubID { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool ProfileGitHub { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public String ProfileMessage { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime RegistrationDate { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime LastActivityDate { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int QuestionsPosted { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int AnswersPosted { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int CommentsPosted { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int ProfileViewCount { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int Reputation { get; set; }
        [Required]
        [Column(TypeName = "tinyint")]
        public byte Status { get; set; }
        [NotMapped]
        public String Role {
            get
            {
                using (var context = new ApplicationDbContext())
                {
                    IdentityUserRole<string> role = context.UserRoles.SingleOrDefault(r => r.UserId == Id);
                    return role != null ? role.RoleId : "Member";
                }
            }
            set
            {
                using (var context = new ApplicationDbContext())
                {
                    IdentityUserRole<string> role = context.UserRoles.SingleOrDefault(r => r.UserId == Id);
                    if (role != null)
                    {
                        context.UserRoles.Remove(role);
                    }
                    context.UserRoles.Add(new IdentityUserRole<string>()
                    {
                        UserId = Id,
                        RoleId = value
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
