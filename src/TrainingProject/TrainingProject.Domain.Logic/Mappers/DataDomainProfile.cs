using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using DomainUser = TrainingProject.Domain.Models.User;
using DataUser = TrainingProject.Data.Models.User;
using DomainTest = TrainingProject.Domain.Models.Test;
using DataTest = TrainingProject.Data.Models.Test;
using DataQuestion = TrainingProject.Data.Models.Question;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using DataAnswerOption = TrainingProject.Data.Models.AnswerOption;
using DomainAnswerOption = TrainingProject.Domain.Models.AnswerOption;
using DataAnswerText = TrainingProject.Data.Models.AnswerText;
using DomainAnswerText = TrainingProject.Domain.Models.AnswerText;
using DataUserAnswerOption = TrainingProject.Data.Models.UserAnswerOption;
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswerOption;

using TrainingProject.Data.Models;

namespace TrainingProject.Domain.Logic.Mappers
{
    public class DataDomainProfile : Profile
    {
        public DataDomainProfile()
        {
            CreateMap<DataUser, DomainUser>()
                .ReverseMap();

            CreateMap<DataTest, DomainTest>().ReverseMap();
            CreateMap<DataQuestion, DomainQuestion>().ReverseMap();
            CreateMap<DataAnswerOption, DomainAnswerOption>().ReverseMap();
            CreateMap<DataAnswerText, DomainAnswerText>().ReverseMap();
            CreateMap<DataUserAnswerOption, DomainUserAnswerOption>().ReverseMap();
        }
    }
}
