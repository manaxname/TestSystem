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

namespace TestSystem.Domain.Logic.Managers
{
    public class AnswersManager : IAnswersManager
    {
        private readonly ITestSystemContext _tpContext;

        private readonly IQuestionManager _qusetionManager;

        private IMapper _mapper;

        public AnswersManager(ITestSystemContext tpContext, IQuestionManager questionManager, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _qusetionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public int CreateAnswer(DomainAnswer answer)
        {
            if (!_qusetionManager.IsQuestionExists(answer.QuestionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            if (this.IsAnswerExists(answer.Id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Answer doesn't exist.");
            }

            var dataAnswer = _mapper.Map<DataAnswer>(answer);

            _tpContext.Answers.Add(dataAnswer);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUserAnswer(DomainUserAnswer userAnswer)
        {
            if (!this.IsAnswerExists(userAnswer.AnswerId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Answer doesn't exist.");
            }

            if (this.IsUserAnswerExists(userAnswer.UserId, userAnswer.AnswerId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserAnswer already exists.");
            }

            var dataUserAnswer = _mapper.Map<DataUserAnswer>(userAnswer);

            _tpContext.UserAnswers.Add(dataUserAnswer);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUserAnswers(IEnumerable<DomainUserAnswer> userAnswers)
        {
            _tpContext.UserAnswers.AddRange(
                userAnswers.Select(x => _mapper.Map<DataUserAnswer>(x)));

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public void DeleteAnswer(int id)
        {
            throw new NotImplementedException();
        }

        public DomainAnswer GetAnswerById(int id)
        {
            if (!this.IsAnswerExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Answer does't exist.");
            }

            var dataAnswer = _tpContext.Answers.First(answer => answer.Id == id);

            var domainAnswer = _mapper.Map<DomainAnswer>(dataAnswer);

            return domainAnswer;
        }

        public int GetAnswerCountByQuestionId(int questionId)
        {
            if (!_qusetionManager.IsQuestionExists(questionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            return _tpContext.Answers.Count(answer => answer.QuestionId == questionId);
        }

        public IEnumerable<DomainAnswer> GetAnswersByQuestionId(int questionId)
        {
            if (!_qusetionManager.IsQuestionExists(questionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            var domainAnswers = _tpContext.Answers
                .Where(answer => answer.QuestionId == questionId)
                .Select(answer => _mapper.Map<DomainAnswer>(answer));

            if (domainAnswers == null)
            {
               return Enumerable.Empty<DomainAnswer>();
            }

            return domainAnswers;
        }

        public DomainUserAnswer GetUserAnswerByIds(int userId, int answerId)
        {
            if (!this.IsUserAnswerExists(userId, answerId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserAnswer does't exist.");
            }

            var dataUserAnswer = _tpContext.UserAnswers
                .First(userAnswer => userAnswer.UserId == userId && 
                    userAnswer.AnswerId == answerId);

            var domainUserAnswer = _mapper.Map<DomainUserAnswer>(dataUserAnswer);

            return domainUserAnswer;
        }

        public IEnumerable<DomainUserAnswer> GetUserAnswersByQuestionId(int userId, int questionId)
        {
            IEnumerable<DomainUserAnswer> domainUserAnswers;

            domainUserAnswers = _tpContext.UserAnswers.Include(x => x.Answer)
                .Where(x => x.UserId == userId && x.Answer.QuestionId == questionId)
                .Select(x => _mapper.Map<DomainUserAnswer>(x));

            return domainUserAnswers;
        }

        public bool IsAnswerExists(int id)
        {
            return _tpContext.Answers.Any(answers => answers.Id == id);
        }

        public bool IsUserAnswerExists(int userId, int answerId)
        {
            return _tpContext.UserAnswers.Any(x => x.UserId == userId && x.AnswerId == answerId);
        }

        public int UpdateUserAnswerValid(int userId, int answerId, bool isValid)
        {
            var dataUserAnswer = _tpContext.UserAnswers.FirstOrDefault(x => x.UserId == userId && x.AnswerId == answerId);

            dataUserAnswer.isValid = isValid;

            _tpContext.UserAnswers.Update(dataUserAnswer);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int UpdateUserAnswerText(int userId, int answerId, string text)
        {
            var dataUserAnswer = _tpContext.UserAnswers.FirstOrDefault(x => x.UserId == userId && x.AnswerId == answerId);

            dataUserAnswer.Text = text;

            _tpContext.UserAnswers.Update(dataUserAnswer);

            return _tpContext.SaveChangesAsync(default).Result;
        }
    }
}
