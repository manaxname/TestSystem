using System;
using System.Collections.Generic;
using System.Text;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IAnswersManager
    {
        int CreateAnswer(Answer answer);
        int CreateUserAnswer(UserAnswer userAnswer);
        int CreateUserAnswers(IEnumerable<UserAnswer> userAnswers);
        Answer GetAnswerById(int id);
        int GetAnswerCountByQuestionId(int questionId);
        UserAnswer GetUserAnswerByIds(int userId, int answerId);
        IEnumerable<UserAnswer> GetUserAnswersByQuestionId(int userId, int questionId);
        IEnumerable<Answer> GetAnswersByQuestionId(int questionId);
        void DeleteAnswer(int id);
        bool IsAnswerExists(int id);
        bool IsUserAnswerExists(int userId, int answerId);
        int UpdateUserAnswerValid(int userId, int answerId, bool isValid);
        int UpdateUserAnswerText(int userId, int answerId, string text);
    }
}
