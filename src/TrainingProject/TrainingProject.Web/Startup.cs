using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.Domain.Logic;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Logic.Mappers;
using TrainingProject.Web.Controllers;
using DomainUser = TrainingProject.Domain.Models.User;

namespace TrainingProject.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDomainServices(connectionString);
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainViewProfile());
                cfg.AddProfile(new DataDomainProfile());
            }).CreateMapper());


            services.AddOpenApiDocument();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(int.MaxValue);
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddAuthorization(opts => {
                opts.AddPolicy("OnlyForAdmins", policy => {
                    //policy.RequireClaim("role", UserRoles.Admin);
                    policy.RequireRole(UserRoles.Admin);
                });
                opts.AddPolicy("OnlyForUsers", policy => {
                    policy.RequireRole(UserRoles.User);
                });
            });

            var adminEmail = "adminIsCoolDude123@gmail.com";
            var adminPassword = "adminIsCoolDude123!!!";
            var adminRole = UserRoles.Admin;
            var domainUser = Helper.CreateDomainUser(adminEmail, adminPassword, adminRole);

            var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetRequiredService<IUserManager>();
            if (!userManager.IsUserExists(adminEmail))
            {
                userManager.CreateUser(adminEmail, adminPassword, adminRole);
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