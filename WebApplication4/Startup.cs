using Domain.Core;
using Domain.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.Data.SortOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using Serilog;
using WebApplication4.EmailSender;
using WebApplication4.Hubs;
using Westwind.AspNetCore.Markdown;

namespace WebApplication4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = Configuration.GetConnectionString("DefaultConnection");

            // services.AddDbContext<AppDbContext>(options =>
            // {
            //     options.UseSqlServer(dbConnectionString);
            // });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString, b => b.MigrationsAssembly("WebApplication4"));
            });


            services.AddHangfire(i => i.UseSqlServerStorage(dbConnectionString));
            services.AddHangfireServer();

            services.AddMarkdown();
            services.AddAntiforgery(x => x.HeaderName = "X-ANTI-FORGERY-TOKEN");

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/";
                options.SlidingExpiration = true;
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);

            services.AddSignalR();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ILotRepository<SortBy, ShowOptions>, LotRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<ICloudStorage, GoogleCloudStorage>();

            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddTransient<IEmailSender<MailMessage>, EmailSender.EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler("/Error/500");
            app.UseStatusCodePagesWithRedirects("/Error/{0}");

            app.UseHttpsRedirection();

            app.UseMarkdown();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthFilter() }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<BetHub>("/betHub");
            });
        }
    }
}
