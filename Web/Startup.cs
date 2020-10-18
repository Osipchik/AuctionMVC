using AutoMapper;
using Data;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Repository;
using Service;
using Web.Hubs;
using Westwind.AspNetCore.Markdown;

namespace Web
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

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = Configuration.GetSection("Authentication:Google");
                    
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddRazorRuntimeCompilation()
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
            
            services.AddSignalR();

            services.AddHangfire(i => i.UseSqlServerStorage(dbConnectionString));

            services.AddAutoMapper(typeof(Startup));


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
            
            
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new []{new HangfireAuthFilter()}
            });
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<CommentHub>("/commentsHub");
            });
        }
    }
}