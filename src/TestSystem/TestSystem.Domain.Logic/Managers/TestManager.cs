using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Models;
using DataTest = TestSystem.Data.Models.Test;
using DomainTest = TestSystem.Domain.Models.Test;
using DataUserTest = TestSystem.Data.Models.UserTest;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using System.Threading.Tasks;
using TestSystem.Common.CustomExceptions;
using AutoMapper.QueryableExtensions;

namespace TestSystem.Domain.Logic.Managers
{
    public class TestManager : ITestManager
    {
        private readonly ITestSystemContext _dbContext;

        private readonly IUserManager _userManager;

        private IMapper _mapper;

        public TestManager(ITestSystemContext dbContext, IUserManager userManager, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<int> CreateTestAsync(string name, int time)
        {
            var domainTest = Helper.CreateDomainTest(name, time);

            var dataTest = _mapper.Map<DataTest>(domainTest);

            _dbContext.Tests.Add(dataTest);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> CreateUserTestAsync(DomainUserTest userTest)
        {
            await _userManager.ThrowIfUserNotExistsAsync(userTest.UserId);
            await ThrowIfTestNotExistsAsync(userTest.TestId);

            var dataUserTest = _mapper.Map<DataUserTest>(userTest);

            _dbContext.UserTests.Add(dataUserTest);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task DeleteTestAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainTest> GetTestByIdAsync(int id)
        {
            var dataTest = await _dbContext.Tests.FirstOrDefaultAsync(test => test.Id == id);

            if (dataTest == null)
            {
                throw new TestNotFoundException(id.ToString());
            }

            var domainTest = _mapper.Map<DomainTest>(dataTest);

            return domainTest;
        }

        public async Task<IReadOnlyCollection<DomainTest>> GetTestsAsync()
        {
            var domainTests = await _dbContext.Tests.ProjectTo<DomainTest>(_mapper.ConfigurationProvider).ToListAsync();

            return domainTests;
        }

        public async Task<DomainUserTest> GetUserTestAsync(int userId, int testId)
        {
            var dataUserTest = await _dbContext.UserTests
                .SingleOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            var domainUserTest = _mapper.Map<DomainUserTest>(dataUserTest);

            return domainUserTest;
        }

        public async Task<IReadOnlyCollection<DomainUserTest>> GetUserTestsAsync(int userId)
        {
            await _userManager.ThrowIfUserNotExistsAsync(userId);

            var domainUserTests = await _dbContext.UserTests
                .Include(x => x.User)
                .Include(x => x.Test)
                .Where(x => x.UserId == userId)
                .ProjectTo<DomainUserTest>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return domainUserTests;
        }

        public async Task<bool> IsTestExistsAsync(int id)
        {
            return await _dbContext.Tests.AnyAsync(test => test.Id == id);
        }

        public async Task<bool> IsUserTestExistsAsync(int usetId, int testId)
        {
            return await _dbContext.UserTests.AnyAsync(x => x.UserId == usetId && x.TestId == testId);
        }

        public async Task<int> UpdateUserTestPointsAsync(int userId, int testId, int points)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.Points = points;

            _dbContext.UserTests.Update(dataUserTest);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> UpdateUserTestStartTimeAsync(int userId, int testId, DateTime time)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.StartTime = time;

            _dbContext.UserTests.Update(dataUserTest);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> UpdateUserTestStatusAsync(int userId, int testId, string status)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.Status = status;

            _dbContext.UserTests.Update(dataUserTest);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task ThrowIfTestNotExistsAsync(int testId)
        {
            if (! await IsTestExistsAsync(testId))
            {
                throw new TestNotFoundException(testId.ToString());
            }
        }

        public async Task ThrowIfTestAlreadyExistsAsync(int testId)
        {
            if (await IsTestExistsAsync(testId))
            {
                throw new TestAlreadyExsitsException(testId.ToString());
            }
        }

        public async Task<IReadOnlyCollection<DomainUserTest>> GetUserTestsAsync(int userId, params int[] testIds)
        {
            if (testIds == null)
            {
                throw new ArgumentNullException(nameof(testIds));
            }

            return await _dbContext.UserTests
                .Where(x => x.UserId == userId && testIds.Contains(x.TestId))
                .ProjectTo<DomainUserTest>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
