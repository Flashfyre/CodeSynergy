using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CodeSynergy.Models;
using CodeSynergy.Services;
using NonFactors.Mvc.Grid;
using Microsoft.EntityFrameworkCore;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using System;

namespace CodeSynergy
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administration", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });
                options.AddPolicy("Moderation", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Moderator");
                });
                options.AddPolicy("Members", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Member");
                });
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<string>>>();

            services.AddIdentity<ApplicationUser, IdentityRole<string>>(o =>
                {
                    o.Password.RequiredLength = 6;
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext, string>()
                .AddUserStore<UserRepository>()
                .AddRoleStore<RoleStore<IdentityRole<string>, ApplicationDbContext, string>>()
                .AddUserManager<UserManager>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddMvcGrid();
            
            services.AddScoped<UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string>, UserRepository>();
            services.AddScoped<IJoinTableRepository<UserMailbox, string, byte>, MailboxRepository>();
            services.AddScoped<IRepository<PrivateMessage, long>, PrivateMessageRepository>();
            services.AddSingleton<IRepository<ModerationMailboxItem, int>, ModerationMailbox>();
            services.AddScoped<IRepository<Report, int>, ReportRepository>();
            services.AddScoped<IRepository<Ban, int>, BanRepository>();
            services.AddScoped<IRepository<UntrustedURLPattern, int>, UntrustedURLPatternRepository>();
            services.AddScoped<IRepository<RankingPos, short>, RankingPosRepository>();
            services.AddScoped<IRepository<Question, int>, QuestionRepository>();
            services.AddScoped<IJoinTableRepository<Star, string, int>, StarRepository>();
            services.AddScoped<IRepository<Tag, int>, TagRepository>();
            services.AddScoped<IJoinTableRepository<QuestionTag, int, int>, QuestionTagRepository>();
            services.AddScoped<ApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<SignInManager, SignInManager>();
            services.AddScoped<UserManager, UserManager>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id:int?}/{modal?}");
                routes.MapRoute(
                   name: "question",
                   template: "{controller=Question}/{id:int}/{action=Index}");
                routes.MapRoute(
                    name: "mailbox",
                    template: "{controller=Mailbox}/{mailboxtypeid:int}/{mailboxitemid:int?}/{action=Index}");
                routes.MapRoute(
                    name: "user",
                    template: "{controller=User}/{displayname?}/{action=Index}");
                routes.MapRoute(
                    name: "search",
                    template: "{controller=Search}/{searchtype?}/{query?}/{action=Index}/{rowsperpage?}");
            });

            SeedData.Initialize(app.ApplicationServices);
        }
    }
}
