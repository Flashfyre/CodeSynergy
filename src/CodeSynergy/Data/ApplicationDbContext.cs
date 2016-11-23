using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeSynergy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CodeSynergy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<string>, string>
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RankingCategory> RankingCategories { get; set; }
        public DbSet<RankingPos> RankingPos { get; set; }
        public DbSet<Ranking> Rankings { get; set; }
        public DbSet<RepVote> RepVotes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QAPost> QAPosts { get; set; }
        public DbSet<QAPostVote> QAPostVotes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<UserMailbox> UserMailboxes { get; set; }
        public DbSet<UserMailboxItem> UserMailboxItems { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<ModerationMailboxItem> ModerationMailboxItems { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<UntrustedURLPattern> UntrustedURLPatterns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(e =>
            {
                e.ToTable("AspNetUsers");
                e.HasKey("Id");
                e.HasOne(x => x.Country);
                e.HasOne(x => x.Region);
                e.HasMany(x => x.Roles);
                e.HasMany(x => x.QAPosts).WithOne(x => x.User);
                e.HasMany(x => x.Comments).WithOne(x => x.User);
                e.HasMany(x => x.Stars).WithOne(x => x.User);
                e.HasMany(x => x.UserTags).WithOne(x => x.User);
            });
            builder.Entity<RankingCategory>(e =>
            {
                e.ToTable("RankingCategory");
                e.HasKey("RankingCategoryID");
                e.Property(x => x.RankingCategoryID).ValueGeneratedNever();
            });
            builder.Entity<RankingPos>(e =>
            {
                e.ToTable("RankingPos");
                e.HasKey("RankingPosID");
                e.HasMany(x => x.Rankings).WithOne(x => x.RankingPos);
            });
            builder.Entity<Ranking>(e =>
            {
                e.ToTable("Ranking");
                e.HasKey("RankingCategoryID", "RankingPosID");
                e.HasOne(x => x.RankingCategory);
                e.HasOne(x => x.RankingPos);
                e.HasOne(x => x.User);
            });
            builder.Entity<RepVote>(e =>
            {
                e.ToTable("RepVote");
                e.HasKey("VoteeUserID", "VoterUserID");
                e.HasOne(x => x.VoteeUser);
                e.HasOne(x => x.VoterUser);
            });
            builder.Entity<Question>(e =>
            {
                e.ToTable("Question");
                e.HasKey("QuestionID");
                e.HasOne(x => x.LockedByUser);
                e.HasMany(x => x.Posts).WithOne(x => x.Question);
                e.HasMany(x => x.Stars).WithOne(x => x.Question);
                e.HasMany(x => x.QuestionTags).WithOne(x => x.Question);
            });
            builder.Entity<QAPost>(e =>
            {
                e.ToTable("Post");
                e.HasKey("QuestionID", "QuestionPostID");
                e.HasOne(x => x.Question);
                e.HasOne(x => x.User).WithMany(x => x.QAPosts);
                e.HasMany(x => x.Comments).WithOne(x => x.Post);
            });
            builder.Entity<QAPostVote>(e =>
            {
                e.ToTable("PostVote");
                e.HasKey("QuestionID", "QuestionPostID", "UserID");
                e.HasOne(x => x.Post);
                e.HasOne(x => x.User);
            });
            builder.Entity<Comment>(e =>
            {
                e.ToTable("Comment");
                e.HasKey("QuestionID", "QuestionPostID", "PostCommentID");
                e.HasOne(x => x.Post);
                e.HasOne(x => x.User).WithMany(x => x.Comments);
            });
            builder.Entity<CommentVote>(e =>
            {
                e.ToTable("CommentVote");
                e.HasKey("QuestionID", "QuestionPostID", "PostCommentID", "UserID");
                e.HasOne(x => x.Comment);
                e.HasOne(x => x.User);
            });
            builder.Entity<Star>(e =>
            {
                e.ToTable("Star");
                e.HasKey("UserID", "QuestionID");
                e.HasOne(x => x.User).WithMany(x => x.Stars);
                e.HasOne(x => x.Question).WithMany(x => x.Stars);
            });
            builder.Entity<Tag>(e =>
            {
                e.ToTable("Tag");
                e.HasKey("TagID");
            });
            builder.Entity<QuestionTag>(e =>
            {
                e.ToTable("QuestionTag");
                e.HasKey("QuestionID", "TagID");
                e.HasOne(x => x.Question);
                e.HasOne(x => x.Tag);
            });
            builder.Entity<UserTag>(e =>
            {
                e.ToTable("UserTag");
                e.HasKey("UserID", "TagID");
                e.HasOne(x => x.User).WithMany(x => x.UserTags);
                e.HasOne(x => x.Tag);
            });
            builder.Entity<Country>(e =>
            {
                e.ToTable("Country");
                e.HasKey("ISO");
            });
            builder.Entity<Region>(e =>
            {
                e.ToTable("Region");
                e.HasKey("RegionID");
                e.HasOne(x => x.Country);
            });
            builder.Entity<UserMailbox>(e =>
            {
                e.ToTable("UserMailbox");
                e.HasKey("UserID", "MailboxTypeID");
                e.HasOne(x => x.User);
                e.HasMany(x => x.UserItems).WithOne(x => x.Mailbox);
            });
            builder.Entity<UserMailboxItem>(e =>
            {
                e.ToTable("UserMailboxItem");
                e.HasKey("UserID", "MailboxTypeID", "MailboxItemID");
                e.HasOne(x => x.User);
                e.HasOne(x => x.Mailbox).WithMany(x => x.UserItems);
                e.HasOne(x => x.PrivateMessage);
            });
            builder.Entity<PrivateMessage>(e =>
            {
                e.ToTable("PrivateMessage");
                e.HasKey("PrivateMessageID");
                e.HasOne(x => x.SenderUser);
                e.HasOne(x => x.RecipientUser);
            });
            builder.Entity<ModerationMailboxItem>(e =>
            {
                e.ToTable("ModerationMailboxItem");
                e.HasKey("MailboxItemID");
                e.HasOne(x => x.Report);
                e.HasOne(x => x.AssigneeUser);
                e.HasOne(x => x.AssignerUser);
            });
            builder.Entity<Report>(e =>
            {
                e.ToTable("Report");
                e.HasKey("ReportID");
                e.HasOne(x => x.SenderUser);
                e.HasOne(x => x.ReportedQuestion);
                e.HasOne(x => x.ReportedAnswer);
                e.HasOne(x => x.ReportedComment);
                e.HasOne(x => x.ReportedUser);
                e.HasOne(x => x.ReportedPrivateMessage);
            });
            builder.Entity<Ban>(e =>
            {
                e.ToTable("Ban");
                e.HasKey("BanID");
                e.HasOne(x => x.BannedUser);
                e.HasOne(x => x.BanningUser);
            });
            builder.Entity<UntrustedURLPattern>(e =>
            {
                e.ToTable("UntrustedURLPattern");
                e.HasKey("PatternID");
                e.HasOne(x => x.AddedByUser);
                e.HasOne(x => x.LastUpdatedByUser);
                e.HasOne(x => x.RemovedByUser);
            });
            /*foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }*/
            builder.ForSqlServerUseIdentityColumns();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // I realize this connection string is unencrypted, but these credentials are only used in this application and this application only contains test data so if some
            // hacker wants to use it to get into the db, they can go right ahead because it will just be a waste of time for them since there's nothing to be gained
            optionsBuilder.UseSqlServer(@"Server=tcp:codesynergy.database.windows.net,1433;Database=codesynergy;User ID=flashfire@codesynergy;Password=adm!nPasswurd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true");
        }
    }
}
