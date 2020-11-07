using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizManager.Areas.Identity.Data;
using System;

namespace QuizManager
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            if (bool.Parse(Configuration["FirstTimeSetup"]))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                roleManager.CreateAsync(new IdentityRole("Moderator")).Wait();
                roleManager.CreateAsync(new IdentityRole("Default")).Wait();

                userManager.CreateAsync(new ApplicationUser() { UserName = "Quiz_Admin" }, "Testing123!").Wait();
                userManager.CreateAsync(new ApplicationUser() { UserName = "Quiz_Moderator" }, "Testing123!").Wait();
                userManager.CreateAsync(new ApplicationUser() { UserName = "Quiz_User" }, "Testing123!").Wait();

                var admin = userManager.FindByNameAsync("Quiz_Admin").Result;
                var moderator = userManager.FindByNameAsync("Quiz_Moderator").Result;
                var user = userManager.FindByNameAsync("Quiz_User").Result;

                userManager.AddToRoleAsync(admin, "Admin");
                userManager.AddToRoleAsync(moderator, "Moderator");
                userManager.AddToRoleAsync(user, "Default");
            }
        }
    }
}
