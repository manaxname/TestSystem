using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingProject.Data;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Models;
using DataAnswerOption = TrainingProject.Data.Models.AnswerOption;
using DomainAnswerOption = TrainingProject.Domain.Models.AnswerOption;
using DataUser = TrainingProject.Data.Models.User;
using DomainUser = TrainingProject.Domain.Models.User;
using DataUserAnswerOption = TrainingProject.Data.Models.UserAnswerOption;
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswerOption;
using DataQuestion = TrainingProject.Data.Models.Question;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using Microsoft.EntityFrameworkCore;

namespace TrainingProject.Domain.Logic.Managers
{
    public class AnswersManager : IAnswersManager
    {
        private readonly ITrainingProjectContext _tpContext;

        private readonly IQuestionManager _qusetionManager;

        private IMapper _mapper;

        public AnswersManager(ITrainingProjectContext tpContext, IQuestionManager questionManager, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _qusetionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public int CreateAnswerOption(DomainAnswerOption answerOption)
        {
            if (!_qusetionManager.IsQuestionExists(answerOption.QuestionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            if (this.IsAnswerOptionExists(answerOption.Id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "AnswerOption doesn't exist.");
            }

            var dataAnswerOption = _mapper.Map<DataAnswerOption>(answerOption);

            _tpContext.AnswersOptions.Add(dataAnswerOption);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUserAnswerOption(DomainUserAnswerOption userAnswerOption)
        {
            if (!this.IsAnswerOptionExists(userAnswerOption.AnswerOptionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "AnswerOption doesn't exist.");
            }

            if (this.IsUserAnswerOptionExists(userAnswerOption.UserId, userAnswerOption.AnswerOptionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserAnswerOption already exists.");
            }

            var dataUserAnswerOption = _mapper.Map<DataUserAnswerOption>(userAnswerOption);

            _tpContext.UserAnswerOptions.Add(dataUserAnswerOption);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUserAnswerOptions(IEnumerable<DomainUserAnswerOption> userAnswerOptions)
        {
            _tpContext.UserAnswerOptions.AddRange(
                userAnswerOptions.Select(x => _mapper.Map<DataUserAnswerOption>(x)));

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public void DeleteAnswerOption(int id)
        {
            throw new NotImplementedException();
        }

        public DomainAnswerOption GetAnswerOptionById(int id)
        {
            if (!this.IsAnswerOptionExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "AnswerOption does't exist.");
            }

            var dataAnswerOption = _tpContext.AnswersOptions.First(answerOption => answerOption.Id == id);

            var domainAnswerOption = _mapper.Map<DomainAnswerOption>(dataAnswerOption);

            return domainAnswerOption;
        }

        public int GetAnswerOptionCountByQuestionId(int questionId)
        {
            if (!_qusetionManager.IsQuestionExists(questionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            return _tpContext.AnswersOptions
                .Count(answerOption => answerOption.QuestionId == questionId);
        }

        public IEnumerable<DomainAnswerOption> GetAnswerOptionsByQuestionId(int questionId)
        {
            if (!_qusetionManager.IsQuestionExists(questionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question doesn't exist.");
            }

            var domainAnswerOptions = _tpContext.AnswersOptions
                .Where(answerOption => answerOption.QuestionId == questionId)
                .Select(answerOption => _mapper.Map<DomainAnswerOption>(answerOption));

            if (domainAnswerOptions == null)
            {
               return Enumerable.Empty<DomainAnswerOption>();
            }

            return domainAnswerOptions;
        }

        public DomainUserAnswerOption GetUserAnswerOptionByIds(int userId, int answerOptionId)
        {
            if (!this.IsUserAnswerOptionExists(userId, answerOptionId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "UserAnswerOption does't exist.");
            }

            var dataUserAnswerOption = _tpContext.UserAnswerOptions
                .First(userAnswerOption => userAnswerOption.UserId == userId && 
                    userAnswerOption.AnswerOptionId == answerOptionId);

            var domainUserAnswerOption = _mapper.Map<DomainUserAnswerOption>(dataUserAnswerOption);

            return domainUserAnswerOption;
        }

        public IEnumerable<DomainUserAnswerOption> GetUserAnswerOptionsByQuestionId(int userId, int questionId)
        {
            IEnumerable<DomainUserAnswerOption> domainUserAnswerOptions;

            domainUserAnswerOptions = _tpContext.UserAnswerOptions.Include(x => x.AnswerOption)
                .Where(x => x.UserId == userId && x.AnswerOption.QuestionId == questionId)
                .Select(x => _mapper.Map<DomainUserAnswerOption>(x));

            return domainUserAnswerOptions;
        }

        public bool IsAnswerOptionExists(int id)
        {
            return _tpContext.AnswersOptions.Any(answersOptions => answersOptions.Id == id);
        }

        public bool IsUserAnswerOptionExists(int userId, int answerOptionId)
        {
            return _tpContext.UserAnswerOptions.Any(x => x.UserId == userId && x.AnswerOptionId == answerOptionId);
        }

        public int SetUserAnswerOptionValid(int userId, int answerOptionId, bool isValid)
        {
            var dataUserAnswerOption = _tpContext.UserAnswerOptions.FirstOrDefault(x => x.UserId == userId && x.AnswerOptionId == answerOptionId);

            dataUserAnswerOption.isValid = isValid;

            _tpContext.UserAnswerOptions.Update(dataUserAnswerOption);

            return _tpContext.SaveChangesAsync(default).Result;
        }
    }
}
