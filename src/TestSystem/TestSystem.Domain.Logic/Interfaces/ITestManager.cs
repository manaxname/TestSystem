using System;
using System.Collections.Generic;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface ITestManager
    {
        int CreateTest(string name, int time);
        Test GetTestById(int id);
        void DeleteTest(int id);
        IEnumerable<Test> GetTests();
        int CreateUserTest(UserTest userTest);
        IEnumerable<UserTest> GetUserTests(int userId);
        UserTest GetUserTest(int userId, int testId);
        int UpdateUserTestStatus(int userId, int testId, string status);
        int UpdateUserTestPoints(int userId, int testId, int points);
        int UpdateUserTestStartTime(int userId, int testId, DateTime time);
        bool IsTestExists(int id);
        bool IsUserTestExists(int usetId, int testId);
    }
}
