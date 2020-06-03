using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using DataTest = TestSystem.Data.Models.Test;
using DomainTest = TestSystem.Domain.Models.Test;
using DataUserTest = TestSystem.Data.Models.UserTest;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using DataTopic = TestSystem.Data.Models.Topic;
using DataUser = TestSystem.Data.Models.User;
using DomainTopic = TestSystem.Domain.Models.Topic;
using DataUserTopic = TestSystem.Data.Models.UserTopic;
using DomainUserTopic = TestSystem.Domain.Models.UserTopic;
using System.Threading.Tasks;
using TestSystem.Common.CustomExceptions;
using AutoMapper.QueryableExtensions;
using TestSystem.Common;
using System.Threading;
using TestSystem.Data.Models;

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
            var domainTests = await _dbContext.Tests.ProjectTo<DomainTest>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

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
                .AsNoTracking()
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

        public async Task UpdateUserTestStatusAsync(int userId, int testId, TestStatus status)
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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateTopicAsync(string name, int passingPoints, TopicType topicType, bool isLocked)
        {
            var domainTopic = Helper.CreateDomainTopic(name, passingPoints, topicType, isLocked);

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

        public IQueryable<DomainTopic> GetTopicsAsync(string search, int? fromIndex = null,
            int? toIndex = null, bool? isLocked = null, TopicType topicType = TopicType.Public)
        {
            var query = _dbContext.Topics.AsNoTracking();
            if (isLocked != null)
            {
                query = query.Where(x => x.IsLocked == isLocked);
            }

            query = query.Where(x => x.TopicType == topicType);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.OrderBy(x => x.Name);

            if (fromIndex.HasValue && toIndex.HasValue)
            {
                query = query.Skip(fromIndex.Value).Take(toIndex.Value - fromIndex.Value + 1);
            }

            return  _mapper.ProjectTo<DomainTopic>(query);
        }

        public IQueryable<DomainUserTopic> GetUserTopicsAsync(int userId, 
            string search, int? fromIndex = null, int? toIndex = null, bool? isLocked = false, TopicType topicType = TopicType.Public, bool? isTopicAssigned = null)
        {
            var query = _dbContext.UserTopics.AsNoTracking().Include(x => x.Topic).AsNoTracking();
            query = query.Where(x => x.UserId == userId && x.Topic.TopicType == topicType);

            if (isTopicAssigned != null)
            {
                query = query.Where(x => x.IsTopicAsigned == isTopicAssigned);
            }

            if (isLocked != null)
            {
                query = query.Where(x => x.Topic.IsLocked == isLocked);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Topic.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.OrderBy(x => x.Topic.Name);

            if (fromIndex.HasValue && toIndex.HasValue)
            {
                query = query.Skip(fromIndex.Value).Take(toIndex.Value - fromIndex.Value + 1);
            }

            return _mapper.ProjectTo<DomainUserTopic>(query);
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
                .AsNoTracking()
                .ToListAsync();

            return domainTests;
        }

        public async Task<IReadOnlyCollection<DomainUserTest>> GetUserTestsInTopicAsync(int topicId, int userId)
        {
            var domainUserTests = await _dbContext.UserTests
                .Include(x => x.Test)
                .Where(x => x.UserId == userId && x.Test.TopicId == topicId)
                .ProjectTo<DomainUserTest>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return domainUserTests;
        }

        public async Task UpdateUserTopicStatus(int userId, int topicId, TopicStatus status)
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

        public async Task<int> GetTestsInTopicsCountAsync()
        {
            return await _dbContext.Topics.CountAsync();
        }

        public async Task CreateTopicForAllUsers(int topicId)
        {
            List<DataUser> dataUsers = await _dbContext.Users.Where(x => x.Role == UserRoles.User).ToListAsync();
            DomainTopic domainTopic = await GetTopicByIdAsync(topicId);

            foreach (var dataUser in dataUsers)
            {
                var userTopic = new DomainUserTopic
                {
                    UserId = dataUser.Id,
                    TopicId = topicId,
                    Status = TopicStatus.NotStarted,
                    IsTopicAsigned = domainTopic.TopicType == TopicType.Public ? true : false,
                    Points = 0
                };

                await CreateUserTopicAsync(userTopic);
            }
        }

        public async Task<int> GetTopicsCountAsync(string search, bool? isLocked, TopicType topicType, CancellationToken cancellationToken = default)
        {
            IQueryable<DataTopic> query = _dbContext.Topics;

            if (isLocked != null)
            {
                query = query.Where(x => x.IsLocked == isLocked);
            }

            query = query.Where(x => x.TopicType == topicType);

            if (search == null)
            {
                return await query.CountAsync();
            }

            return await query.Where(x => x.Name.ToLower().Contains(search.ToLower())).CountAsync(cancellationToken);
        }

        public async Task UpdateUserTopicPoints(int userId, int topicId, int points)
        {
            var dataUserTopic = await _dbContext.UserTopics
                         .SingleOrDefaultAsync(x => x.UserId == userId && x.TopicId == topicId);

            if (dataUserTopic == null)
            {
                throw new UserTopicNotFoundException($"userId: {userId}, topicId: {topicId}");
            }

            dataUserTopic.Points = points;

            _dbContext.UserTopics.Update(dataUserTopic);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task UpdateTopicIsLocked(int topicId, bool isLocked)
        {
            var dataTopic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == topicId);

            if (dataTopic == null)
            {
                throw new TopicNotFoundException($"topicId: {topicId}");
            }

            dataTopic.IsLocked = isLocked;

            _dbContext.Topics.Update(dataTopic);

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task<IReadOnlyCollection<int>> GetTestStagesAsync(int testId)
        {
            var dataTest = await _dbContext.Tests.AsNoTracking()
                .Include(x => x.Questions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == testId);

            if (dataTest == null)
            {
                throw new TestNotFoundException($"testId: {testId}");
            }

            var stages = new List<int>();
            var questions = dataTest.Questions.ToList();

            foreach (var question in questions)
            {
                stages.Add(question.Stage);
            }

            stages.Sort();

            return stages;
        }

        public async Task<int> GetUserTopicsCountAsync(int userId, string search, bool? isLocked = null, TopicType topicType = TopicType.Public,
            bool? isTopicAssigned = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.UserTopics.AsNoTracking();
            query = query.Where(x => x.UserId == userId && x.Topic.TopicType == topicType);

            if (isTopicAssigned != null)
            {
                query = query.Where(x => x.IsTopicAsigned == isTopicAssigned);
            }

            if (isLocked != null)
            {
                query = query.Where(x => x.Topic.IsLocked == isLocked);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Topic.Name.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync(cancellationToken);
        }
    }
}
