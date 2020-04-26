using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IAnswersManager
    {
        Task<int> CreateAnswerAsync(Answer answer);
        Task<int> CreateUserAnswerAsync(UserAnswer userAnswer);
        Task<int> CreateUserAnswersAsync(IReadOnlyCollection<UserAnswer> userAnswers);
        Task<Answer> GetAnswerByIdAsync(int id);
        Task<int> GetAnswerCountByQuestionIdAsync(int questionId);
        Task<UserAnswer> GetUserAnswerByIdsAsync(int userId, int answerId);
        Task<IReadOnlyCollection<UserAnswer>> GetUserAnswersByQuestionIdAsync(int userId, int questionId);
        Task<IReadOnlyCollection<Answer>> GetAnswersByQuestionIdAsync(int questionId);
        Task DeleteAnswerAsync(int id);
        Task<bool> IsAnswerExistsAsync(int id);
        Task<bool> IsUserAnswerExistsAsync(int userId, int answerId);
        Task<int> UpdateUserAnswerValidAsync(int userId, int answerId, bool isValid);
        Task<int> UpdateUserAnswerTextAsync(int userId, int answerId, string text);
        Task ThrowIfAnswerNotExistsAsync(int testId);
        Task ThrowIfAnswerAlreadyExistsAsync(int testId);
        Task ThrowIfUserAnswerNotExistsAsync(int userId, int answerId);
        Task ThrowIfUserAnswerAlreadyExistsAsync(int userId, int answerId);
    }
}
