using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TrainingProject.Data;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Logic.Managers;
using TrainingProject.Domain.Logic.Mappers;

namespace TrainingProject.Domain.Logic
{
    public static class DomainLogicExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, string connectionString)
        {
            //configure your Domain Logic Layer services here
            services.AddDataServices(connectionString);

            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<ITestManager, TestManager>();
            services.AddTransient<IQuestionManager, QuestionManager>();
            services.AddTransient<IAnswersManager, AnswersManager>();

            return services;
        }
    }
}