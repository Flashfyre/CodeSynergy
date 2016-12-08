using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using CodeSynergy.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    // Class representing the AspNetUsers table
    public class ApplicationUser : IdentityUser, ISearchable, IReportable
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
        public string CountryID { get; set; }
        [ForeignKey("CountryID")]
        public Country Country { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string RegionID { get; set; }
        [ForeignKey("RegionID")]
        public Region Region { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public string FirstName { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public string LastName { get; set; }
        [Column(TypeName = "nvarchar(40)")]
        public string JobTitle { get; set; }
        [Column(TypeName = "bit")]
        public bool? Gender { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string City { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string GitHubID { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool ProfileGitHub { get; set; } // Whether to link to the user's GitHub profile on their user profile
        [Required]
        [Column(TypeName = "nvarchar(4000)")]
        public string ProfileMessage { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool ExcludeFromRanking { get; set; } // Whether to exclude this user from the ranking
        [Required]
        [Column(TypeName = "bit")]
        public bool UseProfileBackground { get; set; } // Whether to use the user's profile background throughout the site
        [Required]
        [Column(TypeName = "bit")]
        public bool UseSearchGrid { get; set; } // Whether to use wider, more detailed grids where necessary
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
        public int QuestionScore { get; set; } // Total score for all posted questions
        [Required]
        [Column(TypeName = "int")]
        public int AnswerScore { get; set; } // Total score for all posted answers
        [Required]
        [Column(TypeName = "int")]
        public int CommentScore { get; set; } // Total score for all posted comments
        [Required]
        [Column(TypeName = "int")]
        public int BestAnswerCount { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int StarCount { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int ProfileViewCount { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int Reputation { get; set; }
        [Required]
        [Column(TypeName = "bit")]
        public bool Online { get; set; }
        public ICollection<QAPost> QAPosts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Star> Stars { get; set; }
        public ICollection<UserTag> UserTags { get; set; }
        [NotMapped]
        public string Role
        {
            get
            {
                return Roles.FirstOrDefault().RoleId;
            }
            set
            {
                Roles.Clear();
                Roles.Add(new IdentityUserRole<string>()
                {
                    UserId = Id,
                    RoleId = value
                });
            }
        }
        [NotMapped]
        public int PostCount
        {
            get
            {
                return Posts.Count();
            }
        }
        [NotMapped]
        public List<Post> Posts
        {
            get
            {
                return QAPosts.Concat<Post>(Comments).ToList();
            }
        }
        [NotMapped]
        public int TotalPosts {
            get {
                return QuestionsPosted + AnswersPosted + CommentsPosted;
            }
        }
        [NotMapped]
        public int TotalScore
        {
            get
            {
                return QuestionScore + AnswerScore + CommentScore;
            }
        }
        [NotMapped]
        public string Location
        {
            get
            {
                return CountryID != null ? (RegionID != null ? (City != null ? City + ", " : "") + Region.RegionName + ", " : "") + Country.CountryName : "";
            }
        }
        [NotMapped]
        public string TimeSinceRegistrationDate
        {
            get
            {
                return FormatHelper.GetTimeSinceDate(RegistrationDate);
            }
        }
        [NotMapped]
        public string TimeSinceLastActivityDate
        {
            get
            {
                return FormatHelper.GetTimeSinceDate(LastActivityDate);
            }
        }
        /*[NotMapped]
        public string Role {
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
        }*/
        [NotMapped]
        public string RoleBadge
        {
            get
            {
                string roleBadge;
                switch (Role)
                {
                    case "Member":
                        roleBadge = "";
                        break;
                    case "Moderator":
                        roleBadge = "<i class=\"fa fa-shield role-badge\" title=\"Moderator\"></i>&nbsp;";
                        break;
                    case "Administrator":
                        if (Email == "admin@codesynergy.com")
                        {
                            roleBadge = "<i class=\"fa fa-key role-badge\" title=\"Head Administrator\"></i>&nbsp;";
                        }
                        else
                        {
                            roleBadge = "<i class=\"fa fa-wrench role-badge\" title=\"Administrator\"></i>&nbsp;";
                        }
                        break;
                    default:
                        roleBadge = "";
                        break;
                }

                return roleBadge;
            }
        }
        [NotMapped]
        public string FormattedDisplayName
        {
            get
            {
                return DisplayName.Replace('_', ' ');
            }
        }
        // Formatted Display Name with Role Badge
        [NotMapped]
        public string FullFormattedDisplayName
        {
            get
            {
                return RoleBadge + FormattedDisplayName;
            }
        }
        [NotMapped]
        public string PrivateMessageButton
        {
            get
            {
                return "&nbsp;<i class=\"fa fa-envelope hover-green btn-private-message\" data-display-name=\"" + DisplayName + "\"></i>";
            }
        }
        [NotMapped]
        public string GitHubLink
        {
            get
            {
                return ProfileGitHub && GitHubID != null ? "&nbsp;<a href=\"https://github.com/" + GitHubID + "\" class=\"github-link hover-green\" title=\"" + FormattedDisplayName + (FormattedDisplayName.ToLower().Last() != 's' ? "'s" : "'") + " GitHub Profile\"><i class=\"fa fa-github\"></i></a>" : "";
            }
        }
        public string ProfileImageUrl
        {
            get
            {
                if (File.Exists("wwwroot/images/user/profile/" + Id + ".png"))
                {
                    return "/images/user/profile/" + Id + ".png";
                } else
                {
                    return "/images/LogoSm.png";
                }
            }
        }

        public bool HasCustomBackground
        {
            get
            {
                return File.Exists("wwwroot/images/user/background/" + Id + ".png");
            }
        }

        public string BackgroundImageUrl
        {
            get
            {
                if (File.Exists("wwwroot/images/user/background/" + Id + ".png"))
                {
                    return "/images/user/background/" + Id + ".png";
                }
                else
                {
                    return "/images/stripebg.png";
                }
            }
        }

        /*
         * Returns the HTML for the user's Status Indicator icon
         */
        public string GetStatusIndicatorHtml(UserManager userManager)
        {
            Ban activeBan = userManager.GetActiveBan(this);
            return "&nbsp;<i class=\"fa " + (activeBan == null ? "fa-square status-" + (Online ? "online" : "offline") + "\" title=\"" + (Online ? "Online" : "Offline") : "fa-ban status-offline\" title=\"Banned (" + (activeBan.BanTerm != (byte) EnumBanTerm.Perm ? "expires " + ((DateTime) activeBan.BanLiftDate).ToLocalTime() : "Permanent") + ")") + "\"></i>";
        }

        /*
         * Returns the HTML for the user's Display Name including the Role Badge if applicable and the user's status if a UserManager instance is provided
         */
        public string GetFullFormattedDisplayName(UserManager userManager, bool isUrl = false, string appendToURL = "")
        {
            string formattedDisplayName = FormattedDisplayName;
            string fullFormattedDisplayName = RoleBadge;

            if (!isUrl)
                fullFormattedDisplayName += formattedDisplayName;
            else
                fullFormattedDisplayName += "<a href=\"/User/" + formattedDisplayName + appendToURL + "\" class=\"profile-link blue hover-green\">" + formattedDisplayName + "</a>";

            if (userManager != null)
                fullFormattedDisplayName += GetStatusIndicatorHtml(userManager);

            return fullFormattedDisplayName;
        }

        public Post GetLastActivityPostForQuestion(Question question)
        {
            List<Post> posts = Posts.Where(p => p.QuestionID == question.QuestionID).ToList();
            if (posts.Count > 0)
            {
                Post lastActivityPost = posts[0];
                foreach (Post post in posts)
                {
                    DateTime dateToCompare = post.LastActivityDate;
                    if (dateToCompare > lastActivityPost.LastActivityDate)
                    {
                        lastActivityPost = post;
                    }
                }

                return lastActivityPost;
            }
            else
                return null;
        }

        public RepVote GetVote(ApplicationDbContext context, string username)
        {
            return context.RepVotes.SingleOrDefault(u => u.VoterUser.Email == username && u.VoteeUserID == Id);
        }

        public async Task AddVote(ApplicationDbContext context, UserRepository userRepository, ApplicationUser voter, bool vote)
        {
            RepVote RepVote = new RepVote()
            {
                VoteeUserID = Id,
                VoterUserID = voter.Id,
                Vote = vote
            };
            context.RepVotes.Add(RepVote);
            if (vote)
                Reputation++;
            else
                Reputation--;
            await userRepository.UpdateAsync(this);
            context.SaveChanges();
        }

        public async Task UpdateVote(ApplicationDbContext context, UserRepository userRepository, RepVote RepVote)
        {
            context.RepVotes.Update(RepVote);
            Reputation += RepVote.Vote ? 2 : -2;
            await userRepository.UpdateAsync(this);
            context.SaveChanges();
        }

        public async Task RemoveVote(ApplicationDbContext context, UserRepository userRepository, RepVote RepVote)
        {
            if (RepVote.Vote)
                Reputation--;
            else
                Reputation++;
            context.RepVotes.Remove(RepVote);
            await userRepository.UpdateAsync(this);
            context.SaveChanges();
        }

        public string[] GetSearchText()
        {
            return new string[] { FormattedDisplayName };
        }

        public EnumReportType GetReportType()
        {
            return EnumReportType.User_Profile;
        }

        public string GetReportableItemID()
        {
            return DisplayName;
        }

        public string GetReportableItemDescription()
        {
            return "<strong>" + FormattedDisplayName + "</strong>'" + (FormattedDisplayName.ToLower().Last() != 's' ? "s" : "") + " CodeSynergy profile";
        }

        public string GetReportableItemLinkHtml(string returnUrl = null)
        {
            return "<a href=\"/User/" + DisplayName + "\" class=\"blue hover-green\">" + GetReportableItemDescription() + "</a>";
        }
    }
}
