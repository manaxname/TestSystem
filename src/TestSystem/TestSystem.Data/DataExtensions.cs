using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestSystem.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ITestSystemContext, TestSystemContext>(item => item.UseSqlServer(connectionString));
            
            return services;
        }
    }
}