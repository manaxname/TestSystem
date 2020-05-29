using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestSystem.Common;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface ITestManager
    {
        Task<int> CreateTopicAsync(string name, int passingPoints, TopicType topicType, bool isLocked);
        Task CreateTestAsync(int topicId, string name, int time);
        Task CreateUserTopicAsync(UserTopic userTopic);
        Task CreateUserTestAsync(UserTest userTest);
        Task CreateTopicForAllUsers(int topicId);
        Task<Test> GetTestByIdAsync(int id);
        Task<Topic> GetTopicByIdAsync(int id);
        Task<UserTopic> GetUserTopicAsync(int userId, int topicId);
        Task<IReadOnlyCollection<int>> GetTestStagesAsync(int testId);
        Task DeleteTestAsync(int id);
        Task<int> GetTopicsCountAsync(string search, bool? isLocked);
        Task<IReadOnlyCollection<Topic>> GetTopicsAsync(string search, int? fromIndex = null, int? toIndex = null, bool? isLocked = false,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UserTopic>> GetUserTopicsAsync(int userId,
            string search, int? fromIndex = null, int? toIndex = null, bool isLocked = false, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Test>> GetTestsAsync();
        Task<IReadOnlyCollection<Test>> GetTestsInTopicAsync(int topicId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsInTopicAsync(int topicId, int userId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId, params int[] testIds);
        Task<UserTest> GetUserTestAsync(int userId, int testId);
        Task<int> GetTestsInTopicsCountAsync();
        Task UpdateUserTestStatusAsync(int userId, int testId, TestStatus status);
        Task UpdateUserTestPointsAsync(int userId, int testId, int points);
        Task UpdateUserTestStartTimeAsync(int userId, int testId, DateTime time);
        Task UpdateUserTopicStatus(int userId, int topicId, TopicStatus status);
        Task UpdateUserTopicPoints(int userId, int topicId, int points);
        Task UpdateTopicIsLocked(int topicId, bool isLocked);
        Task<bool> IsTestExistsAsync(int id);
        Task<bool> IsUserTestExistsAsync(int usetId, int testId);
        Task<bool> IsUserTopicExistsAsync(int usetId, int topicId);
        Task ThrowIfTestNotExistsAsync(int testId);
        Task ThrowIfTestAlreadyExistsAsync(int testId);
        Task ThrowIfUserTopicNotExistsAsync(int userId, int topicId);
        Task ThrowIfUserTopicAlreadyExistsAsync(int userId, int topicId);
    }
}
