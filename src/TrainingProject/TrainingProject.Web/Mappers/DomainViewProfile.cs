using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using DomainTest = TrainingProject.Domain.Models.Test;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using DomainAnswerOption = TrainingProject.Domain.Models.Answer;
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswer;
using DomainUserTest = TrainingProject.Domain.Models.UserTest;

using TrainingProject.Web.Models;

namespace TrainingProject.Domain.Logic.Mappers
{
    public class DomainViewProfile : Profile
    {
        public DomainViewProfile()
        {
            CreateMap<DomainTest, TestModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(map => map.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(map => map.Name))
                .ReverseMap();

            CreateMap<DomainQuestion, QuestionModel>().ReverseMap();
            CreateMap<DomainAnswerOption, AnswerModel>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(map => map.Text))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(map => map.IsValid))
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(map => map.QuestionId))
                .ReverseMap();

            CreateMap<DomainUserAnswerOption, UserAnswerModel>().ReverseMap();
            CreateMap<DomainUserTest, UserTestModel>()
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(map => map.Test.Name))
                .ReverseMap();
        }
    }
}
