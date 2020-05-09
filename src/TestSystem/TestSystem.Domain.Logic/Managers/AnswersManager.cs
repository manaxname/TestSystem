using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Models;
using DataAnswer = TestSystem.Data.Models.Answer;
using DomainAnswer = TestSystem.Domain.Models.Answer;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;
using DataUserAnswer = TestSystem.Data.Models.UserAnswer;
using DomainUserAnswer = TestSystem.Domain.Models.UserAnswer;
using DataQuestion = TestSystem.Data.Models.Question;
using DomainQuestion = TestSystem.Domain.Models.Question;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TestSystem.Common.CustomExceptions;
using AutoMapper.QueryableExtensions;

namespace TestSystem.Domain.Logic.Managers
{
    public class AnswersManager : IAnswersManager
    {
        private readonly ITestSystemContext _dbContext;

        private readonly IQuestionManager _qusetionManager;

        private readonly IUserManager _userManager;

        private IMapper _mapper;

        public AnswersManager(ITestSystemContext dbContext, IQuestionManager questionManager, IUserManager userManager, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _qusetionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<int> CreateAnswerAsync(DomainAnswer answer)
        {
            await ThrowIfAnswerAlreadyExistsAsync(answer.Id);

            var dataAnswer = _mapper.Map<DataAnswer>(answer);

            _dbContext.Answers.Add(dataAnswer);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> CreateUserAnswerAsync(DomainUserAnswer userAnswer)
        {
            await ThrowIfUserAnswerAlreadyExistsAsync(userAnswer.UserId, userAnswer.AnswerId);

            var dataUserAnswer = _mapper.Map<DataUserAnswer>(userAnswer);

            _dbContext.UserAnswers.Add(dataUserAnswer);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> CreateUserAnswersAsync(IReadOnlyCollection<DomainUserAnswer> userAnswers)
        {
            // TODO: CHECK HERE

            _dbContext.UserAnswers.AddRange(
                userAnswers.Select(x => _mapper.Map<DataUserAnswer>(x)));

            return await _dbContext.SaveChangesAsync(default);
        }

        public Task DeleteAnswerAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainAnswer> GetAnswerByIdAsync(int id)
        {
            var dataAnswer = await _dbContext.Answers.FirstOrDefaultAsync(answer => answer.Id == id);

            if (dataAnswer == null)
            {
                throw new AnswerNotFoundException(id.ToString());
            }

            var domainAnswer = _mapper.Map<DomainAnswer>(dataAnswer);

            return domainAnswer;
        }

        public async Task<int> GetAnswerCountByQuestionIdAsync(int questionId)
        {
            await _qusetionManager.ThrowIfQuestionNotExistsAsync(questionId);

            return await _dbContext.Answers.CountAsync(answer => answer.QuestionId == questionId);
        }

        public async Task<IReadOnlyCollection<DomainAnswer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            await _qusetionManager.ThrowIfQuestionNotExistsAsync(questionId);

            var domainAnswers = await _dbContext.Answers
                .Where(answer => answer.QuestionId == questionId)
                .ProjectTo<DomainAnswer>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return domainAnswers;
        }

        public async Task<DomainUserAnswer> GetUserAnswerByIdsAsync(int userId, int answerId)
        {
            var dataUserAnswer = await _dbContext.UserAnswers
                .FirstOrDefaultAsync(userAnswer => userAnswer.UserId == userId && 
                    userAnswer.AnswerId == answerId);

            if (dataUserAnswer == null)
            {
                throw new UserAnswerNotFoundException($"userId: {userId}, answerId: {answerId}");
            }

            var domainUserAnswer = _mapper.Map<DomainUserAnswer>(dataUserAnswer);

            return domainUserAnswer;
        }

        public async Task<IReadOnlyCollection<DomainUserAnswer>> GetUserAnswersByQuestionIdAsync(int userId, int questionId)
        {
            var domainUserAnswers = await _dbContext.UserAnswers.Include(x => x.Answer)
                .Where(x => x.UserId == userId && x.Answer.QuestionId == questionId)
                .Select(x => _mapper.Map<DomainUserAnswer>(x))
                .AsNoTracking()
                .ToListAsync();

            return domainUserAnswers;
        }

        public async Task<bool> IsAnswerExistsAsync(int id)
        {
            return await _dbContext.Answers.AnyAsync(answers => answers.Id == id);
        }

        public async Task<bool> IsUserAnswerExistsAsync(int userId, int answerId)
        {
            return await _dbContext.UserAnswers.AnyAsync(x => x.UserId == userId && x.AnswerId == answerId);
        }

        public async Task<int> UpdateUserAnswerValidAsync(int userId, int answerId, bool isValid)
        {
            var dataUserAnswer = await _dbContext.UserAnswers.FirstOrDefaultAsync(x => x.UserId == userId && x.AnswerId == answerId);

            if (dataUserAnswer == null)
            {
                throw new UserAnswerNotFoundException($"userId: {userId}, answerId: {answerId}");
            }

            dataUserAnswer.isValid = isValid;

            _dbContext.UserAnswers.Update(dataUserAnswer);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task<int> UpdateUserAnswerTextAsync(int userId, int answerId, string text)
        {
            var dataUserAnswer = await _dbContext.UserAnswers.FirstOrDefaultAsync(x => x.UserId == userId && x.AnswerId == answerId);

            if (dataUserAnswer == null)
            {
                throw new UserAnswerNotFoundException($"userId: {userId}, answerId: {answerId}");
            }

            dataUserAnswer.Text = text;

            _dbContext.UserAnswers.Update(dataUserAnswer);

            return await _dbContext.SaveChangesAsync(default);
        }

        public async Task ThrowIfAnswerNotExistsAsync(int answerId)
        {
            if (!await IsAnswerExistsAsync(answerId))
            {
                throw new AnswerNotFoundException(answerId.ToString());
            }
        }

        public async Task ThrowIfAnswerAlreadyExistsAsync(int answerId)
        {
            if (await IsAnswerExistsAsync(answerId))
            {
                throw new AnswerAlreadyExistsException(answerId.ToString());
            }
        }

        public async Task ThrowIfUserAnswerNotExistsAsync(int userId, int answerId)
        {
            if (!await IsUserAnswerExistsAsync(userId, answerId))
            {
                throw new UserAnswerNotFoundException(answerId.ToString());
            }
        }

        public async Task ThrowIfUserAnswerAlreadyExistsAsync(int userId, int answerId)
        {
            if (await IsUserAnswerExistsAsync(userId, answerId))
            {
                throw new UserAnswerAlreadyExistsException(answerId.ToString());
            }
        }
    }
}
