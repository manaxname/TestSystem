using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingProject.Data;
using TrainingProject.Domain.Logic.Interfaces;
using DataTest = TrainingProject.Data.Models.Test;
using DomainTest = TrainingProject.Domain.Models.Test;

namespace TrainingProject.Domain.Logic.Managers
{
    public class TestManager : ITestManager
    {
        private readonly ITrainingProjectContext _tpContext;

        private IMapper _mapper;

        public TestManager(ITrainingProjectContext tpContext, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public int CreateTest(string name)
        {
            var domainTest = Helper.CreateDomainTest(name);
            var dataTest = _mapper.Map<DataTest>(domainTest);

            _tpContext.Tests.Add(dataTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public void DeleteTest(int id)
        {
            throw new NotImplementedException();
        }

        public DomainTest GetTestById(int id)
        {
            if (!this.IsTestExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Test does't exist.");
            }

            var dataTest = _tpContext.Tests.First(test => test.Id == id);

            var domainTest = _mapper.Map<DomainTest>(dataTest);

            return domainTest;
        }

        public IEnumerable<DomainTest> GetTests()
        {
            var domainTests = _tpContext.Tests.Select(test => _mapper.Map<DomainTest>(test));

            if (domainTests == null)
            {
                return Enumerable.Empty<DomainTest>();
            }

            return domainTests;
        }

        public bool IsTestExists(int id)
        {
            return _tpContext.Tests.Any(test => test.Id == id);
        }
    }
}
