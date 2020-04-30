using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface ITestManager
    {
        Task<int> CreateTopicAsync(string name);
        Task<int> CreateTestAsync(int topicId, string name, int time);
        Task<int> CreateUserTopicAsync(UserTopic userTopic);
        Task<int> CreateUserTestAsync(UserTest userTest);
        Task<Test> GetTestByIdAsync(int id);
        Task DeleteTestAsync(int id);
        Task<IReadOnlyCollection<Topic>> GetTopicsAsync();
        Task<IReadOnlyCollection<UserTest>> GetUserTopicsAsync(int userId, int topicId);
        Task<IReadOnlyCollection<Test>> GetTestsAsync();
        Task<IReadOnlyCollection<Test>> GetTestsInTopicAsync(int topicId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsInTopicAsync(int topicId, int userId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId, params int[] testIds);
        Task<UserTest> GetUserTestAsync(int userId, int testId);
        Task<int> UpdateUserTestStatusAsync(int userId, int testId, string status);
        Task<int> UpdateUserTestPointsAsync(int userId, int testId, int points);
        Task<int> UpdateUserTestStartTimeAsync(int userId, int testId, DateTime time);
        Task<bool> IsTestExistsAsync(int id);
        Task<bool> IsUserTestExistsAsync(int usetId, int testId);
        Task<bool> IsUserTopicExistsAsync(int usetId, int topicId);
        Task ThrowIfTestNotExistsAsync(int testId);
        Task ThrowIfTestAlreadyExistsAsync(int testId);
        Task ThrowIfUserTopicNotExistsAsync(int userId, int topicId);
        Task ThrowIfUserTopicAlreadyExistsAsync(int userId, int topicId);
    }
}
