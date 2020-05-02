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
using DataTopic = TestSystem.Data.Models.Topic;
using DomainTopic = TestSystem.Domain.Models.Topic;
using DataUserTopic = TestSystem.Data.Models.UserTopic;
using DomainUserTopic = TestSystem.Domain.Models.UserTopic;
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

        public async Task CreateTestAsync(int topicId, string name, int time)
        {
            var domainTest = Helper.CreateDomainTest(topicId, name, time);

            var dataTest = _mapper.Map<DataTest>(domainTest);

            _dbContext.Tests.Add(dataTest);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task CreateUserTestAsync(DomainUserTest userTest)
        {
            await _userManager.ThrowIfUserNotExistsAsync(userTest.UserId);
            await ThrowIfTestNotExistsAsync(userTest.TestId);

            var dataUserTest = _mapper.Map<DataUserTest>(userTest);

            _dbContext.UserTests.Add(dataUserTest);

            await _dbContext.SaveChangesAsync(default);
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

        public async Task<bool> IsUserTopicExistsAsync(int usetId, int topicId)
        {
            return await _dbContext.UserTopics.AnyAsync(x => x.UserId == usetId && x.TopicId == topicId);
        }

        public async Task UpdateUserTestPointsAsync(int userId, int testId, int points)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.Points = points;

            _dbContext.UserTests.Update(dataUserTest);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task UpdateUserTestStartTimeAsync(int userId, int testId, DateTime time)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.StartTime = time;

            _dbContext.UserTests.Update(dataUserTest);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task UpdateUserTestStatusAsync(int userId, int testId, string status)
        {
            var dataUserTest = await _dbContext.UserTests.FirstOrDefaultAsync(x => x.UserId == userId && x.TestId == testId);

            if (dataUserTest == null)
            {
                throw new UserTestNotFoundException($"userId: {userId}, testId: {testId}");
            }

            dataUserTest.Status = status;

            _dbContext.UserTests.Update(dataUserTest);

            await _dbContext.SaveChangesAsync(default);
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

        public async Task<int> CreateTopicAsync(string name, int passingPoints)
        {
            var domainTopic = Helper.CreateDomainTopic(name, passingPoints);

            var dataTopic = _mapper.Map<DataTopic>(domainTopic);

            _dbContext.Topics.Add(dataTopic);

            await _dbContext.SaveChangesAsync(default);

            return dataTopic.Id;
        }

        public async Task CreateUserTopicAsync(DomainUserTopic userTopic)
        {
            await ThrowIfUserTopicAlreadyExistsAsync(userTopic.UserId, userTopic.TopicId);

            var dataUserTopic = _mapper.Map<DataUserTopic>(userTopic);

            _dbContext.UserTopics.Add(dataUserTopic);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task<IReadOnlyCollection<DomainTopic>> GetTopicsAsync()
        {
            var domainTopic = await _dbContext.Topics.ProjectTo<DomainTopic>(_mapper.ConfigurationProvider).ToListAsync();

            return domainTopic;
        }

        public async Task<IReadOnlyCollection<DomainUserTest>> GetUserTopicsAsync(int userId, int topicId)
        {
            await ThrowIfUserTopicNotExistsAsync(userId, topicId);

            var domainUserTests = await _dbContext.UserTopics
                .Where(x => x.UserId == userId && x.TopicId == topicId)
                .ProjectTo<DomainUserTest>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return domainUserTests;
        }

        public async Task ThrowIfUserTopicNotExistsAsync(int userId, int topicId)
        {
            if (!await IsUserTopicExistsAsync(userId, topicId))
            {
                throw new UserTopicNotFoundException($"userId: {userId}, topicId: {topicId}");
            }
        }

        public async Task ThrowIfUserTopicAlreadyExistsAsync(int userId, int topicId)
        {
            if (await IsUserTopicExistsAsync(userId, topicId))
            {
                throw new UserTopicNotFoundException($"userId: {userId}, topicId: {topicId}");
            }
        }

        public async Task<IReadOnlyCollection<DomainTest>> GetTestsInTopicAsync(int topicId)
        {
            var domainTests = await _dbContext.Tests
                .Where(x => x.TopicId == topicId)
                .ProjectTo<DomainTest>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return domainTests;
        }

        public async Task<IReadOnlyCollection<DomainUserTest>> GetUserTestsInTopicAsync(int topicId, int userId)
        {
            var domainUserTests = await _dbContext.UserTests
                .Include(x => x.Test)
                .Where(x => x.UserId == userId && x.Test.TopicId == topicId)
                .ProjectTo<DomainUserTest>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return domainUserTests;
        }

        public async Task UpdateUserTopicStatus(int userId, int topicId, string status)
        {
            var dataUserTopic = await _dbContext.UserTopics
              .SingleOrDefaultAsync(x => x.UserId == userId && x.TopicId == topicId);

            if (dataUserTopic == null)
            {
                throw new UserTopicNotFoundException($"userId: {userId}, topicId: {topicId}");
            }

            dataUserTopic.Status = status;

            _dbContext.UserTopics.Update(dataUserTopic);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task<DomainUserTopic> GetUserTopicAsync(int userId, int topicId)
        {
            var dataUserTopic = await _dbContext.UserTopics
                .SingleOrDefaultAsync(x => x.UserId == userId && x.TopicId == topicId);

            if (dataUserTopic == null)
            {
                throw new UserTopicNotFoundException($"userId: {userId}, topicId: {topicId}");
            }

            var domainUserTopic = _mapper.Map<DomainUserTopic>(dataUserTopic);

            return domainUserTopic;
        }

        public async Task<DomainTopic> GetTopicByIdAsync(int id)
        {
            var dataTopic = await _dbContext.Topics.FirstOrDefaultAsync(topic => topic.Id == id);

            if (dataTopic == null)
            {
                throw new TopicNotFoundException(id.ToString());
            }

            var domainTopic = _mapper.Map<DomainTopic>(dataTopic);

            return domainTopic;
        }
    }
}
