using System;
using System.Collections.Generic;
using System.Text;
using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic.Interfaces
{
    public interface IAnswersManager
    {
        int CreateAnswerOption(AnswerOption answerWithOption);
        int CreateUserAnswerOption(UserAnswerOption userAnswerOption);
        int CreateUserAnswerOptions(IEnumerable<UserAnswerOption> userAnswerOptions);
        AnswerOption GetAnswerOptionById(int id);
        UserAnswerOption GetUserAnswerOptionByIds(int userId, int answerOptionId);
        IEnumerable<AnswerOption> GetAnswerOptionsByQuestionId(int questionId);
        void DeleteAnswerOption(int id);
        bool IsAnswerOptionExists(int id);
        bool IsUserAnswerOptionExists(int userId, int answerOptionId);
    }
}
