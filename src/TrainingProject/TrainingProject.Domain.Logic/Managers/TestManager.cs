using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingProject.Data;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Models;
using DataTest = TrainingProject.Data.Models.Test;
using DomainTest = TrainingProject.Domain.Models.Test;
using DataUserTest = TrainingProject.Data.Models.UserTest;
using DomainUserTest = TrainingProject.Domain.Models.UserTest;

namespace TrainingProject.Domain.Logic.Managers
{
    public class TestManager : ITestManager
    {
        private readonly ITrainingProjectContext _tpContext;

        private readonly IUserManager _userManager;

        private IMapper _mapper;

        public TestManager(ITrainingProjectContext tpContext, IUserManager userManager, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public int CreateTest(string name, int time)
        {
            var domainTest = Helper.CreateDomainTest(name, time);
            var dataTest = _mapper.Map<DataTest>(domainTest);

            _tpContext.Tests.Add(dataTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUserTest(DomainUserTest userTest)
        {
            if (!_userManager.IsUserExists(userTest.UserId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User does't exist.");
            }

            if (!IsTestExists(userTest.TestId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Test does't exist.");
            }

            var dataUserTest = _mapper.Map<DataUserTest>(userTest);

            _tpContext.UserTests.Add(dataUserTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public void DeleteTest(int id)
        {
            throw new NotImplementedException();
        }

        public DomainTest GetTestById(int id)
        {
            if (!IsTestExists(id))
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

        public DomainUserTest GetUserTest(int userId, int testId)
        {
            if (!IsUserTestExists(userId, testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserTest does't exist.");
            }

            if (!IsTestExists(testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Test does't exist.");
            }

            var dataUserTest = _tpContext.UserTests
                .SingleOrDefault(x => x.UserId == userId && x.TestId == testId);
            var domainUserTest = _mapper.Map<DomainUserTest>(dataUserTest);

            return domainUserTest;
        }

        public IEnumerable<DomainUserTest> GetUserTests(int userId)
        {
            if (!_userManager.IsUserExists(userId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User does't exist.");
            }

            var domainUserTests = _tpContext.UserTests
                .Include(x => x.User)
                .Include(x => x.Test)
                .Where(x => x.UserId == userId)
                .Select(x => _mapper.Map<DomainUserTest>(x));

            return domainUserTests;
        }

        public bool IsTestExists(int id)
        {
            return _tpContext.Tests.Any(test => test.Id == id);
        }

        public bool IsUserTestExists(int usetId, int testId)
        {
            return _tpContext.UserTests.Any(x => x.UserId == usetId && x.TestId == testId);
        }

        public int UpdateUserTestPoints(int userId, int testId, int points)
        {
            if (!IsUserTestExists(userId, testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserTest does't exist.");
            }

            var userTest = _tpContext.UserTests.Single(x => x.UserId == userId && x.TestId == testId);

            userTest.Points = points;

            _tpContext.UserTests.Update(userTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int UpdateUserTestStartTime(int userId, int testId, DateTime time)
        {
            if (!IsUserTestExists(userId, testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserTest does't exist.");
            }

            var userTest = _tpContext.UserTests.Single(x => x.UserId == userId && x.TestId == testId);

            userTest.StartTime = time;

            _tpContext.UserTests.Update(userTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int UpdateUserTestStatus(int userId, int testId, string status)
        {
            if (!IsUserTestExists(userId, testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserTest does't exist.");
            }

            var userTest = _tpContext.UserTests.Single(x => x.UserId == userId && x.TestId == testId);

            userTest.Status = status;

            _tpContext.UserTests.Update(userTest);

            return _tpContext.SaveChangesAsync(default).Result;
        }
    }
}
