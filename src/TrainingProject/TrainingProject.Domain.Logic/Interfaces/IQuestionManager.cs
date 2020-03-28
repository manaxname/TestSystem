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
        void DeleteQuestion(int id);
    }
}
