using AutoMapper;
using DomainUser = TestSystem.Domain.Models.User;
using DataUser = TestSystem.Data.Models.User;
using DomainTest = TestSystem.Domain.Models.Test;
using DataTest = TestSystem.Data.Models.Test;
using DataQuestion = TestSystem.Data.Models.Question;
using DomainQuestion = TestSystem.Domain.Models.Question;
using DataAnswer = TestSystem.Data.Models.Answer;
using DomainAnswer = TestSystem.Domain.Models.Answer;
using DataUserAnswer = TestSystem.Data.Models.UserAnswer;
using DomainUserAnswer = TestSystem.Domain.Models.UserAnswer;
using DataUserTest = TestSystem.Data.Models.UserTest;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using DataTopic = TestSystem.Data.Models.Topic;
using DomainTopic = TestSystem.Domain.Models.Topic;
using DataUserTopic = TestSystem.Data.Models.UserTopic;
using DomainUserTopic = TestSystem.Domain.Models.UserTopic;

namespace TestSystem.Domain.Logic.Mappers
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
            CreateMap<DataUserTest, DomainUserTest>().ReverseMap();
            CreateMap<DataTopic, DomainTopic>().ReverseMap();
            CreateMap<DataUserTopic, DomainUserTopic>().ReverseMap();
        }
    }
}
