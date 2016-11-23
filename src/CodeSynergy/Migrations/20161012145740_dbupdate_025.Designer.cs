using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CodeSynergy.Data;

namespace CodeSynergy.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161012145740_dbupdate_025")]
    partial class dbupdate_025
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CodeSynergy.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("AnswerScore")
                        .HasColumnType("int");

                    b.Property<int>("AnswersPosted")
                        .HasColumnType("int");

                    b.Property<int>("BestAnswerCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("CommentScore")
                        .HasColumnType("int");

                    b.Property<int>("CommentsPosted")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("CountryID")
                        .HasColumnType("varchar(2)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("ExcludeFromRanking")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(40)");

                    b.Property<bool?>("Gender")
                        .HasColumnType("bit");

                    b.Property<string>("GitHubID")
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("JobTitle")
                        .HasColumnType("nvarchar(40)");

                    b.Property<DateTime>("LastActivityDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(40)");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("Online")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<bool>("ProfileGitHub")
                        .HasColumnType("bit");

                    b.Property<string>("ProfileMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)");

                    b.Property<int>("ProfileViewCount")
                        .HasColumnType("int");

                    b.Property<int>("QuestionScore")
                        .HasColumnType("int");

                    b.Property<int>("QuestionsPosted")
                        .HasColumnType("int");

                    b.Property<string>("RegionID")
                        .HasColumnType("varchar(4)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Reputation")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("StarCount")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<bool>("UseProfileBackground")
                        .HasColumnType("bit");

                    b.Property<bool>("UseSearchGrid")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("CountryID");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("RegionID");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("CodeSynergy.Models.Ban", b =>
                {
                    b.Property<int>("BanID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("BanDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("BanLiftDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("BanReason")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte>("BanTerm")
                        .HasColumnType("tinyint");

                    b.Property<string>("BannedUserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BanningUserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BanID");

                    b.HasIndex("BannedUserID");

                    b.HasIndex("BanningUserID");

                    b.ToTable("Ban");
                });

            modelBuilder.Entity("CodeSynergy.Models.Comment", b =>
                {
                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<int>("QuestionPostID")
                        .HasColumnType("int");

                    b.Property<short>("PostCommentID")
                        .HasColumnType("smallint");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("DeletedFlag")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("EditDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PostDate")
                        .HasColumnType("datetime2");

                    b.Property<short>("Score")
                        .HasColumnType("smallint");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("QuestionID", "QuestionPostID", "PostCommentID");

                    b.HasIndex("QuestionID");

                    b.HasIndex("UserID");

                    b.HasIndex("QuestionID", "QuestionPostID");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("CodeSynergy.Models.CommentVote", b =>
                {
                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<int>("QuestionPostID")
                        .HasColumnType("int");

                    b.Property<short>("PostCommentID")
                        .HasColumnType("smallint");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Vote")
                        .HasColumnType("bit");

                    b.HasKey("QuestionID", "QuestionPostID", "PostCommentID", "UserID");

                    b.HasIndex("UserID");

                    b.HasIndex("QuestionID", "QuestionPostID", "PostCommentID");

                    b.ToTable("CommentVote");
                });

            modelBuilder.Entity("CodeSynergy.Models.Country", b =>
                {
                    b.Property<string>("ISO")
                        .HasColumnName("country_iso_code")
                        .HasColumnType("varchar(2)");

                    b.Property<string>("ContinentCode")
                        .IsRequired()
                        .HasColumnName("continent_code")
                        .HasColumnType("varchar(2)");

                    b.Property<string>("ContinentName")
                        .IsRequired()
                        .HasColumnName("continent_name")
                        .HasColumnType("varchar(40)");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasColumnName("country_name")
                        .HasColumnType("varchar(40)");

                    b.HasKey("ISO");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("CodeSynergy.Models.QAPost", b =>
                {
                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<int>("QuestionPostID")
                        .HasColumnType("int");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)");

                    b.Property<bool>("DeletedFlag")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("EditDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PostDate")
                        .HasColumnType("datetime2");

                    b.Property<short>("Score")
                        .HasColumnType("smallint");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("QuestionID", "QuestionPostID");

                    b.HasIndex("QuestionID");

                    b.HasIndex("UserID");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("CodeSynergy.Models.QAPostVote", b =>
                {
                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<int>("QuestionPostID")
                        .HasColumnType("int");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Vote")
                        .HasColumnType("bit");

                    b.HasKey("QuestionID", "QuestionPostID", "UserID");

                    b.HasIndex("UserID");

                    b.HasIndex("QuestionID", "QuestionPostID");

                    b.ToTable("PostVote");
                });

            modelBuilder.Entity("CodeSynergy.Models.Question", b =>
                {
                    b.Property<int>("QuestionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("BestAnswerQuestionPostID")
                        .HasColumnType("int");

                    b.Property<int>("DupeOriginalID")
                        .HasColumnType("int");

                    b.Property<string>("LockedByUserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LockedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SolvedDate")
                        .HasColumnType("datetime2");

                    b.Property<short>("StarCount")
                        .HasColumnType("smallint");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("QuestionID");

                    b.HasIndex("LockedByUserID");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("CodeSynergy.Models.QuestionTag", b =>
                {
                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<int>("TagID")
                        .HasColumnType("int");

                    b.HasKey("QuestionID", "TagID");

                    b.HasIndex("QuestionID");

                    b.HasIndex("TagID");

                    b.ToTable("QuestionTag");
                });

            modelBuilder.Entity("CodeSynergy.Models.Ranking", b =>
                {
                    b.Property<byte>("RankingCategoryID")
                        .HasColumnType("tinyint");

                    b.Property<short>("RankingPosID")
                        .HasColumnType("smallint");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RankingCategoryID", "RankingPosID");

                    b.HasIndex("RankingCategoryID");

                    b.HasIndex("RankingPosID");

                    b.HasIndex("UserID");

                    b.ToTable("Ranking");
                });

            modelBuilder.Entity("CodeSynergy.Models.RankingCategory", b =>
                {
                    b.Property<byte>("RankingCategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint");

                    b.Property<string>("RankingName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("RankingCategoryID");

                    b.ToTable("RankingCategory");
                });

            modelBuilder.Entity("CodeSynergy.Models.RankingPos", b =>
                {
                    b.Property<short>("RankingPosID")
                        .HasColumnType("smallint");

                    b.HasKey("RankingPosID");

                    b.ToTable("RankingPos");
                });

            modelBuilder.Entity("CodeSynergy.Models.Region", b =>
                {
                    b.Property<string>("RegionID")
                        .HasColumnName("region_id")
                        .HasColumnType("varchar(4)");

                    b.Property<string>("ISO")
                        .IsRequired()
                        .HasColumnName("country_iso_code")
                        .HasColumnType("varchar(2)");

                    b.Property<string>("RegionName")
                        .IsRequired()
                        .HasColumnName("region_name")
                        .HasColumnType("varchar(40)");

                    b.HasKey("RegionID");

                    b.HasIndex("ISO");

                    b.ToTable("Region");
                });

            modelBuilder.Entity("CodeSynergy.Models.RepVote", b =>
                {
                    b.Property<string>("VoteeUserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VoterUserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Vote")
                        .HasColumnType("bit");

                    b.HasKey("VoteeUserID", "VoterUserID");

                    b.HasIndex("VoteeUserID");

                    b.HasIndex("VoterUserID");

                    b.ToTable("RepVote");
                });

            modelBuilder.Entity("CodeSynergy.Models.Star", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("QuestionID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StarredDate")
                        .HasColumnType("datetime2");

                    b.HasKey("UserID", "QuestionID");

                    b.HasIndex("QuestionID");

                    b.HasIndex("UserID");

                    b.ToTable("Star");
                });

            modelBuilder.Entity("CodeSynergy.Models.Tag", b =>
                {
                    b.Property<int>("TagID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("TagID");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("CodeSynergy.Models.UntrustedURLPattern", b =>
                {
                    b.Property<int>("PatternID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AddedByUserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LastUpdatedByUserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PatternText")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RemovedByUserID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PatternID");

                    b.HasIndex("AddedByUserID");

                    b.HasIndex("LastUpdatedByUserID");

                    b.HasIndex("RemovedByUserID");

                    b.ToTable("UntrustedURLPattern");
                });

            modelBuilder.Entity("CodeSynergy.Models.UserTag", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TagID")
                        .HasColumnType("int");

                    b.Property<int>("AnswerCount")
                        .HasColumnType("int");

                    b.Property<int>("AnswerScore")
                        .HasColumnType("int");

                    b.Property<int>("BestAnswerCount")
                        .HasColumnType("int");

                    b.Property<int>("CommentCount")
                        .HasColumnType("int");

                    b.Property<int>("CommentScore")
                        .HasColumnType("int");

                    b.Property<int>("QuestionCount")
                        .HasColumnType("int");

                    b.Property<int>("QuestionScore")
                        .HasColumnType("int");

                    b.HasKey("UserID", "TagID");

                    b.HasIndex("TagID");

                    b.HasIndex("UserID");

                    b.ToTable("UserTag");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<string>", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("CodeSynergy.Models.ApplicationUser", b =>
                {
                    b.HasOne("CodeSynergy.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryID");

                    b.HasOne("CodeSynergy.Models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionID");
                });

            modelBuilder.Entity("CodeSynergy.Models.Ban", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "BannedUser")
                        .WithMany()
                        .HasForeignKey("BannedUserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "BanningUser")
                        .WithMany()
                        .HasForeignKey("BanningUserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.Comment", b =>
                {
                    b.HasOne("CodeSynergy.Models.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.QAPost", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("QuestionID", "QuestionPostID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.CommentVote", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("QuestionID", "QuestionPostID", "PostCommentID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.QAPost", b =>
                {
                    b.HasOne("CodeSynergy.Models.Question", "Question")
                        .WithMany("Posts")
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany("QAPosts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.QAPostVote", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.QAPost", "Post")
                        .WithMany()
                        .HasForeignKey("QuestionID", "QuestionPostID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.Question", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "LockedByUser")
                        .WithMany()
                        .HasForeignKey("LockedByUserID");
                });

            modelBuilder.Entity("CodeSynergy.Models.QuestionTag", b =>
                {
                    b.HasOne("CodeSynergy.Models.Question", "Question")
                        .WithMany("QuestionTags")
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.Ranking", b =>
                {
                    b.HasOne("CodeSynergy.Models.RankingCategory", "RankingCategory")
                        .WithMany()
                        .HasForeignKey("RankingCategoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.RankingPos", "RankingPos")
                        .WithMany("Rankings")
                        .HasForeignKey("RankingPosID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.Region", b =>
                {
                    b.HasOne("CodeSynergy.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("ISO")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.RepVote", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "VoteeUser")
                        .WithMany()
                        .HasForeignKey("VoteeUserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "VoterUser")
                        .WithMany()
                        .HasForeignKey("VoterUserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.Star", b =>
                {
                    b.HasOne("CodeSynergy.Models.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany("Stars")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CodeSynergy.Models.UntrustedURLPattern", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser", "AddedByUser")
                        .WithMany()
                        .HasForeignKey("AddedByUserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "LastUpdatedByUser")
                        .WithMany()
                        .HasForeignKey("LastUpdatedByUserID");

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "RemovedByUser")
                        .WithMany()
                        .HasForeignKey("RemovedByUserID");
                });

            modelBuilder.Entity("CodeSynergy.Models.UserTag", b =>
                {
                    b.HasOne("CodeSynergy.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser", "User")
                        .WithMany("UserTags")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<string>")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CodeSynergy.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<string>")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeSynergy.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
