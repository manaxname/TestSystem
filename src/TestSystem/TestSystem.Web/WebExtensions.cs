using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using TestSystem.Common;
using TestSystem.Domain.Logic;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Mappers;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace TestSystem.Web
{
    public static class WebExtensions
    {
        public static string ImagesFolderFullName { get; private set; }
        public static string ImagesFolderName { get; private set; }

        public static IServiceCollection AddWebServices(this IServiceCollection services, string imagesFolderFullName)
        {
            ImagesFolderFullName = imagesFolderFullName;
            ImagesFolderName = Path.GetFileName(imagesFolderFullName);

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
            services.AddSession(s => {
                s.IdleTimeout = TimeSpan.FromMinutes(int.MaxValue);
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(s =>
                {
                    s.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    s.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddAuthorization(opts => {
                opts.AddPolicy("OnlyForAdmins", policy => {
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

            return services;
        }
    }
}
