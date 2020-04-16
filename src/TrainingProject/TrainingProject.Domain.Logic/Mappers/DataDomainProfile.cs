using AutoMapper;
using DomainUser = TrainingProject.Domain.Models.User;
using DataUser = TrainingProject.Data.Models.User;
using DomainTest = TrainingProject.Domain.Models.Test;
using DataTest = TrainingProject.Data.Models.Test;
using DataQuestion = TrainingProject.Data.Models.Question;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using DataAnswer = TrainingProject.Data.Models.Answer;
using DomainAnswer = TrainingProject.Domain.Models.Answer;
using DataUserAnswer = TrainingProject.Data.Models.UserAnswer;
using DomainUserAnswer = TrainingProject.Domain.Models.UserAnswer;
using DataUserTest = TrainingProject.Data.Models.UserTest;
using DomainUserTest = TrainingProject.Domain.Models.UserTest;

namespace TrainingProject.Domain.Logic.Mappers
{
    public class DataDomainProfile : Profile
    {
        public DataDomainProfile()
        {
            CreateMap<DataUser, DomainUser>().ReverseMap();
            CreateMap<DataTest, DomainTest>().ReverseMap();
            CreateMap<DataQuestion, DomainQuestion>().ReverseMap();
            CreateMap<DataAnswer, DomainAnswer>().ReverseMap();
            CreateMap<DataUserAnswer, DomainUserAnswer>().ReverseMap();
            CreateMap<DataUserTest, DomainUserTest>().ReverseMap();
        }
    }
}
