using System;
using System.IO;
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

using reCAPTCHA.AspNetCore;

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

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string imagesFolderFullName = WebHostEnvironment.WebRootPath + @"\" + Configuration.GetConnectionString("ImagesFolderName");

            services.AddDataServices(connectionString);
            services.AddDomainServices();
            services.AddWebServices();

            WebExtensions.ImagesFolderFullName = imagesFolderFullName;
            WebExtensions.ImagesFolderName = Path.GetFileName(imagesFolderFullName);
            IConfigurationSection emailConstants = Configuration.GetSection("EmailConstants");
            WebExtensions.SenderEmail = emailConstants.GetValue<string>("SenderEmail");
            WebExtensions.SenderEmailPassword = emailConstants.GetValue<string>("SenderEmailPassword");
            WebExtensions.SmtpHost = emailConstants.GetValue<string>("SmtpHost");
            WebExtensions.SmtpPort = emailConstants.GetValue<int>("SmtpPort");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi().UseSwaggerUi3();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}