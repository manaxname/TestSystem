using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestSystem.Common;
using TestSystem.Data;
using TestSystem.Domain.Logic;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Mappers;

namespace TestSystem.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string imagesFolderFullName = WebHostEnvironment.WebRootPath + @"\" + Configuration.GetConnectionString("ImagesFolderName");

            services.AddDataServices(connectionString);
            services.AddDomainServices();
            services.AddWebServices(imagesFolderFullName);


            var adminEmail = "adminIsCoolDude123@gmail.com";
            var adminPassword = "adminIsCoolDude123!!!";
            var adminRole = UserRoles.Admin;

            var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<IUserManager>();

            if (!userManager.IsUserExistsAsync(adminEmail).GetAwaiter().GetResult())
            {
                userManager.CreateUserAsync(adminEmail, adminPassword, adminRole).GetAwaiter().GetResult();
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi().UseSwaggerUi3();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}