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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TestSystem.Web
{
    public static class WebExtensions
    {
        public static string ImagesFolderFullName { get; set; }
        public static string ImagesFolderName { get; set; }

        public static string SenderEmail  { get; set; }
        public static string SenderEmailPassword { get; set; }
        public static string SmtpHost  { get; set; }
        public static int SmtpPort { get; set; }

        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
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
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(s =>
                {
                    s.AccessDeniedPath = new PathString("/Account/Login");
                    s.LoginPath = new PathString("/Account/Login");
                });
            services.AddAuthorization(opts => {
                opts.AddPolicy("OnlyForAdmins", policy => {
                    policy.RequireRole(UserRoles.Admin);
                });
                opts.AddPolicy("OnlyForUsers", policy => {
                    policy.RequireRole(UserRoles.User);
                });
            });

            return services;
        }
    }
}
