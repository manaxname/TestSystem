using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using DomainTest = TrainingProject.Domain.Models.Test;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using DomainAnswerOption = TrainingProject.Domain.Models.AnswerOption;
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswerOption;

using DomainAnswerText = TrainingProject.Domain.Models.AnswerText;
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
            CreateMap<DomainAnswerOption, AnswerOptionModel>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(map => map.Text))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(map => map.IsValid))
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(map => map.QuestionId))
                .ReverseMap();

            CreateMap<DomainUserAnswerOption, UserAnswerOptionModel>().ReverseMap();
        }
    }
}
