using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;
using TestSystem.Domain.Logic.Mappers;

namespace TestSystem.Domain.Logic
{
    public static class DomainLogicExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<ITestManager, TestManager>();
            services.AddTransient<IQuestionManager, QuestionManager>();
            services.AddTransient<IAnswersManager, AnswersManager>();

            return services;
        }
    }
}