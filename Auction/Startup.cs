using System;
using Auction.Data;
using Auction.Models;
using Auction.Services;
using Auction.Services.CloudStorage;
using Auction.Services.Hangfire;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Serilog;
using Westwind.AspNetCore.Markdown;

namespace Auction
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
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConnectionString));

            services.AddMarkdown();
            services.AddAntiforgery(x => x.HeaderName = "X-ANTI-FORGERY-TOKEN");
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });
            
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddRazorRuntimeCompilation()
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);


            services.AddHangfire(i => i.UseSqlServerStorage(dbConnectionString));

            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ILotRepository, LotRepository>();
            services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMarkdown();
            app.UseStaticFiles();
            
            app.UseRouting();

            // app.UseSerilogRequestLogging();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            
            // app.UseHangfireServer(new BackgroundJobServerOptions
            // {
            //     SchedulePollingInterval = TimeSpan.FromSeconds(5)
            // });
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new []{new HangfireAuthFilter()}
            });
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("update",
                    "UpdateLot/Id-{lotId}",
                    new { Controller = "Lot", action = "Update" });
                
                endpoints.MapControllerRoute("lot",
                    "Lot/Id-{lotId}",
                    new { Controller = "Lot", action = "Get" });

                endpoints.MapDefaultControllerRoute();
            });
        }    
    }
}