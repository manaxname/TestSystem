using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.Data.Models;
using TrainingProject.Domain.Logic.Interfaces;
using DataQuestion = TrainingProject.Data.Models.Question;
using DomainQuestion = TrainingProject.Domain.Models.Question;

namespace TrainingProject.Domain.Logic.Managers
{
    public class QuestionManager : IQuestionManager
    {
        private readonly ITrainingProjectContext _tpContext;

        private readonly ITestManager _testManager;

        private IMapper _mapper;

        public QuestionManager(ITrainingProjectContext tpContext, ITestManager testManager, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
        }

        public int CreateQuestion(DomainQuestion question)
        {
            if (IsQuestionExists(question.Id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question already exists.");
            }

            if (!_testManager.IsTestExists(question.TestId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Test doesn't exist.");
            }

            var dataQuestion = _mapper.Map<DataQuestion>(question);

            _tpContext.Questions.Add(dataQuestion);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateQuestion(int testId, string text, int stage, int points, string questionType)
        {
            var domainQuestion = Helper.CreateDomainQuestion(text, stage, points, questionType, testId);

            return this.CreateQuestion(domainQuestion);
        }

        public void DeleteQuestion(int id)
        {
            throw new NotImplementedException();
        }

        public DomainQuestion GetQuestionById(int id)
        {
            if (!IsQuestionExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question does't exist.");
            }

            var dataQuestion = _tpContext.Questions.First(question => question.Id == id);

            var domainQuestion = _mapper.Map<DomainQuestion>(dataQuestion);

            return domainQuestion;
        }

        public IEnumerable<DomainQuestion> GetQuestionsByTestId(int testId)
        {
            if (!_testManager.IsTestExists(testId))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Test does't exist.");
            }

            var domainQuestions = _tpContext.Questions
                .Where(question => question.TestId == testId)
                .Select(question => _mapper.Map<DomainQuestion>(question));


            if (domainQuestions == null)
            {
                return Enumerable.Empty<DomainQuestion>();
            }

            return domainQuestions;
        }

        public string GetQuestionTextById(int id)
        {
            if (!IsQuestionExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "Question does't exist.");
            }

            return GetQuestionById(id).Text;
        }

        public string GetQuestionTypeById(int id)
        {
            return GetQuestionById(id).QuestionType;
        }

        public bool IsQuestionExists(int id)
        {
            return _tpContext.Questions.Any(question => question.Id == id);
        }
    }
}
