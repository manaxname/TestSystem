using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using DomainTest = TestSystem.Domain.Models.Test;
using DomainQuestion = TestSystem.Domain.Models.Question;
using DomainAnswer = TestSystem.Domain.Models.Answer;
using DomainUserAnswer = TestSystem.Domain.Models.UserAnswer;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using DomainTopic = TestSystem.Domain.Models.Topic;
using DomainUserTopic = TestSystem.Domain.Models.UserTopic;
using DomainUser = TestSystem.Domain.Models.User;

using TestSystem.Web.Models;

namespace TestSystem.Domain.Logic.Mappers
{
    public class DomainViewProfile : Profile
    {
        public DomainViewProfile()
        {
            CreateMap<DomainTopic, TopicModel>().ReverseMap();
            CreateMap<DomainUser, UserModel>().ReverseMap();

            CreateMap<DomainTest, TestModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(map => map.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(map => map.Name))
                .ReverseMap();

            CreateMap<DomainQuestion, QuestionModel>().ReverseMap();
            CreateMap<DomainAnswer, AnswerModel>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(map => map.Text))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(map => map.IsValid))
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(map => map.QuestionId))
                .ReverseMap();

            CreateMap<DomainUserAnswer, UserAnswerModel>().ReverseMap();
            CreateMap<DomainUserTest, UserTestModel>()
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(map => map.Test.Name))
                .ReverseMap();
        }
    }
}
