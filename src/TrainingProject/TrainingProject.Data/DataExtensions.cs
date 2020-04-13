using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TrainingProject.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            //configure your Data Layer services here
            services.AddDbContext<ITrainingProjectContext, TrainingProjectContext>(item => item.UseSqlServer(connectionString));
            
            return services;
        }
    }
}