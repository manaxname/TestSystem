﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TestSystem.Common;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IQuestionManager
    {
        Task<int> CreateQuestionAsync(Question user);
        Task<int> CreateQuestionAsync(int testId, string text, int stage, int points, QuestionTypes questionType);
        Task<Question> GetQuestionByIdAsync(int id);
        Task<bool> IsQuestionExistsAsync(int id);
        Task<IReadOnlyCollection<Question>> GetQuestionsByTestIdSortedByStageAsync(int testId);
        Task<QuestionTypes> GetQuestionTypeByIdAsync(int id);
        Task<string> GetQuestionTextByIdAsync(int id);
        Task<int> GetQuestionCountByTestIdAsync(int testId);
        Task<IReadOnlyCollection<int>> GetTestStagesByTestIdAsync(int testId);
        Task<Question> GetRandomQuestionInTestByStageAsync(int testId, int stage);
        Task DeleteQuestionAsync(int id);
        Task<IReadOnlyCollection<Question>> GetUserQuestionsByTestIdSortedByStageAsync(int userId, int testId);
        Task ThrowIfQuestionNotExistsAsync(int testId);
        Task ThrowIfQuestionAlreadyExistsAsync(int testId);
    }
}
