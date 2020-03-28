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
using DataAnswerText = TrainingProject.Data.Models.AnswerText;
using DomainAnswerText = TrainingProject.Domain.Models.AnswerText;
using DataUser = TrainingProject.Data.Models.User;
using DomainUser = TrainingProject.Domain.Models.User;
using DataUserAnswerOption = TrainingProject.Data.Models.UserAnswerOption;
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswerOption;

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
            throw new NotImplementedException();
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

        public bool IsAnswerOptionExists(int id)
        {
            return _tpContext.AnswersOptions.Any(answersOptions => answersOptions.Id == id);
        }
    }
}
