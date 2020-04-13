using System.Collections.Generic;
using TrainingProject.Common;
using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic.Interfaces
{
    public interface IQuestionManager
    {
        int CreateQuestion(Question user);
        int CreateQuestion(int testId, string text, int stage, int points, string questionType);
        Question GetQuestionById(int id);
        bool IsQuestionExists(int id);
        IEnumerable<Question> GetQuestionsByTestId(int testId);
        string GetQuestionTypeById(int id);
        string GetQuestionTextById(int id);
        int GetQuestionCountByTestId(int testId);
        IEnumerable<int> GetTestStagesByTestId(int testId);
        Question GetRandomQuestionInTestByStage(int testId, int stage);
        void DeleteQuestion(int id);
        public IEnumerable<Question> GetUserQuestionsByTestId(int userId, int testId);
    }
}
