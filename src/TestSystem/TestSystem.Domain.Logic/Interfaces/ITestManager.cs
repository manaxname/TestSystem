using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface ITestManager
    {
        Task<int> CreateTestAsync(string name, int time);
        Task<int> CreateUserTestAsync(UserTest userTest);
        Task<Test> GetTestByIdAsync(int id);
        Task DeleteTestAsync(int id);
        Task<IReadOnlyCollection<Test>> GetTestsAsync();
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId);
        Task<IReadOnlyCollection<UserTest>> GetUserTestsAsync(int userId, params int[] testIds);
        Task<UserTest> GetUserTestAsync(int userId, int testId);
        Task<int> UpdateUserTestStatusAsync(int userId, int testId, string status);
        Task<int> UpdateUserTestPointsAsync(int userId, int testId, int points);
        Task<int> UpdateUserTestStartTimeAsync(int userId, int testId, DateTime time);
        Task<bool> IsTestExistsAsync(int id);
        Task<bool> IsUserTestExistsAsync(int usetId, int testId);
        Task ThrowIfTestNotExistsAsync(int testId);
        Task ThrowIfTestAlreadyExistsAsync(int testId);
    }
}
