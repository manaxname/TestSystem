using System.Collections.Generic;
using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic.Interfaces
{
    public interface ITestManager
    {
        int CreateTest(string name);
        Test GetTestById(int id);
        void DeleteTest(int id);
        IEnumerable<Test> GetTests();
        bool IsTestExists(int id);
    }
}
