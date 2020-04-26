using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSystem.Common;
using TestSystem.Common.CustomExceptions;
using TestSystem.Data;
using TestSystem.Data.Models;
using TestSystem.Domain.Logic.Interfaces;
using DataQuestion = TestSystem.Data.Models.Question;
using DomainQuestion = TestSystem.Domain.Models.Question;

namespace TestSystem.Domain.Logic.Managers
{
    public class QuestionManager : IQuestionManager
    {
        private readonly ITestSystemContext _dbContext;

        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        private IMapper _mapper;

        public QuestionManager(ITestSystemContext dbContext, ITestManager testManager, IUserManager userManager, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<int> CreateQuestionAsync(DomainQuestion question)
        {
            await ThrowIfQuestionAlreadyExistsAsync(question.Id);

            var dataQuestion = _mapper.Map<DataQuestion>(question);

            _dbContext.Questions.Add(dataQuestion);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> CreateQuestionAsync(int testId, string text, int stage, int points, string questionType)
        {
            var domainQuestion = Helper.CreateDomainQuestion(text, stage, points, questionType, testId);

            return await CreateQuestionAsync(domainQuestion);
        }

        public async Task DeleteQuestionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainQuestion> GetQuestionByIdAsync(int id)
        {
            var dataQuestion = await _dbContext.Questions.FirstOrDefaultAsync(question => question.Id == id);

            if (dataQuestion == null)
            {
                throw new QuestionNotFoundException(id.ToString());
            }

            var domainQuestion = _mapper.Map<DomainQuestion>(dataQuestion);

            return domainQuestion;
        }

        public async Task<int> GetQuestionCountByTestIdAsync(int testId)
        {
            await _testManager.ThrowIfTestNotExistsAsync(testId);

            return await _dbContext.Questions.CountAsync(q => q.TestId == testId);
        }

        public async Task<IReadOnlyCollection<DomainQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            await _testManager.ThrowIfTestNotExistsAsync(testId);

            var domainQuestions = await _dbContext.Questions
                .Where(question => question.TestId == testId)
                .ProjectTo<DomainQuestion>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return domainQuestions;
        }

        public async Task<string> GetQuestionTextByIdAsync(int id)
        {
            return (await GetQuestionByIdAsync(id)).Text;
        }

        public async Task<string> GetQuestionTypeByIdAsync(int id)
        {
            return (await GetQuestionByIdAsync(id)).QuestionType;
        }

        public async Task<DomainQuestion> GetRandomQuestionInTestByStageAsync(int testId, int stage)
        {
            await _testManager.ThrowIfTestNotExistsAsync(testId);

            var dataQuestions = await _dbContext.Questions
                .Where(question => question.TestId == testId && question.Stage == stage)
                .ToListAsync();

            var rnd = new Random();
            var randomIndex = rnd.Next(dataQuestions.Count);

            var randomDataQuestion = dataQuestions[randomIndex];

            var domainQuestion = _mapper.Map<DomainQuestion>(randomDataQuestion);

            return domainQuestion;
        }

        public async Task<IReadOnlyCollection<int>> GetTestStagesByTestIdAsync(int testId)
        {
            await _testManager.ThrowIfTestNotExistsAsync(testId);

            var stages = await _dbContext.Questions
                .Where(question => question.TestId == testId)
                .GroupBy(x => x.Stage)
                .Select(x => x.Key)
                .ToListAsync();

            return stages;
        }

        public async Task<bool> IsQuestionExistsAsync(int id)
        {
            return await _dbContext.Questions.AnyAsync(question => question.Id == id);
        }

        public async Task<IReadOnlyCollection<DomainQuestion>> GetUserQuestionsByTestIdAsync(int userId, int testId)
        {
            await _userManager.ThrowIfUserNotExistsAsync(userId);
            await _testManager.ThrowIfTestNotExistsAsync(testId);

            var dataQuestions = await _dbContext.Questions
                .Where(x => x.TestId == testId)
                .Include(x => x.Answers)
                    .ThenInclude(x => x.UserAnswers)
                .ToListAsync();

            var questions = new Dictionary<int, DomainQuestion>();

            foreach (var question in dataQuestions)
            {
                foreach (var answer in question.Answers)
                {
                    foreach (var item in answer.UserAnswers)
                    {
                        if (item.UserId == userId && !questions.ContainsKey(question.Id))
                        {
                            questions.Add(question.Id, _mapper.Map<DomainQuestion>(question));
                        }
                    }
                }
            }

            return questions.Values;
        }

        public async Task ThrowIfQuestionNotExistsAsync(int questionId)
        {
            if (! await IsQuestionExistsAsync(questionId))
            {
                throw new QuestionNotFoundException(questionId.ToString());
            }
        }

        public async Task ThrowIfQuestionAlreadyExistsAsync(int questionId)
        {
            if (await IsQuestionExistsAsync(questionId))
            {
                throw new QuestionAlreadyExistsException(questionId.ToString());
            }
        }
    }
}
