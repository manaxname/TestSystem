using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TestSystem.Common;
using TestSystem.Data;
using TestSystem.Domain.Logic;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;
using TestSystem.Domain.Models;

namespace TestSystem.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);
            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = (TestSystemContext)services.GetRequiredService<ITestSystemContext>();
                    context.Database.Migrate();

                    var adminEmail = "adminIsCoolDude123@gmail.com";
                    var adminPassword = "adminIsCoolDude123!!!";
                    var adminRole = UserRoles.Admin;

                    var userManager = (UserManager)services.GetRequiredService<IUserManager>();
                    if (!userManager.IsUserExistsAsync(adminEmail).GetAwaiter().GetResult())
                    {
                        User admin =  userManager.CreateUserAsync(adminEmail, adminPassword, adminRole).GetAwaiter().GetResult();
                        userManager.UpdateUserConfirmStatus(admin.Id, true).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured creating the DB.");
                    throw;
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, config) =>
                {
                    config.ReadFrom.Configuration(ctx.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}