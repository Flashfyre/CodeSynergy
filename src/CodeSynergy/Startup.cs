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
                .AddUserStore<UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string>>()
                .AddRoleStore<RoleStore<IdentityRole<string>, ApplicationDbContext, string>>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddMvcGrid();
            
            services.AddScoped<IKeyValueRepository<ApplicationUser, string>, UserRepository>();
            services.AddScoped<IRepository<Ban, int>, BanRepository>();
            services.AddScoped<IRepository<UntrustedURLPattern, int>, UntrustedURLPatternRepository>();
            services.AddScoped<IRepository<Question, int>, QuestionRepository>();
            services.AddScoped<IRepository<Tag, int>, TagRepository>();
            services.AddScoped<ApplicationDbContext, ApplicationDbContext>();

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

            //EnsureRoles(app, loggerFactory);

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CodeSynergy.Models.SeedData.Initialize(app.ApplicationServices);
        }

        private async void EnsureRoles(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            ILogger logger = loggerFactory.CreateLogger<Startup>();
            RoleManager<IdentityRole<string>> roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole<string>>>();

            string[] roleNames = { "Administrator", "Moderator", "Member" };

            foreach (string roleName in roleNames)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    logger.LogInformation(String.Format("!roleExists for roleName {0}", roleName));
                    IdentityRole<string> identityRole = new IdentityRole<string>();
                    identityRole.Id = roleName;
                    identityRole.Name = roleName;
                    IdentityResult identityResult = await roleManager.CreateAsync(identityRole);
                    if (!identityResult.Succeeded)
                    {
                        logger.LogCritical(
                            String.Format(
                                "!identityResult.Succeeded after roleManager.CreateAsync(identityRole) for  identityRole with roleName {0}", roleName));
                        foreach (var error in identityResult.Errors)
                        {
                            logger.LogCritical(
                                String.Format(
                                    "identityResult.Error.Description: {0}",
                                    error.Description));
                            logger.LogCritical(
                                String.Format(
                                    "identityResult.Error.Code: {0}",
                                 error.Code));
                        }
                    }
                }
            }
        }
    }
}
