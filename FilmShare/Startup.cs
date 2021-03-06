using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmShare.Classes.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using FilmShare.Data;
using FilmShare.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FilmShare{
    public class Startup{
        public Startup(IConfiguration configuration, IWebHostEnvironment env){
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration{ get; }
        
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services){
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(
                            Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            
            services.AddSignalR();
            services.AddSingleton(_env);
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
            if (env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else{
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                // endpoints.MapRazorPages();
                endpoints.MapHub<FilmHub>("/film");
            });
        }
    }
}