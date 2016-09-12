using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeSynergy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeSynergy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<string>, string>
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
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
            });
            builder.Entity<Question>(e =>
            {
                e.ToTable("Question");
                e.HasKey("QuestionID");
            });
            builder.Entity<Post>(e =>
            {
                e.ToTable("Post");
                e.HasKey("QuestionID", "QuestionPostID");
                e.HasOne(x => x.Question);
                e.HasOne(x => x.User);
             });
            builder.Entity<Tag>(e =>
            {
                e.ToTable("Tag");
                e.HasKey("TagID");
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
            optionsBuilder.UseSqlServer(@"Server=tcp:codesynergy.database.windows.net,1433;Database=codesynergy;User ID=flashfire@codesynergy;Password=adm!nPasswurd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=aspnet5-CodeSynergy-5bbe0913-3c72-491f-95b2-9470bcbeb10a;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
