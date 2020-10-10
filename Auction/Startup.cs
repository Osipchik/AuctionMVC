using Auction.Data;
using Auction.Models;
using Auction.Services;
using Auction.Services.CloudStorage;
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
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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