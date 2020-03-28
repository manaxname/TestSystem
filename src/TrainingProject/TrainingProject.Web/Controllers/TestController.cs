using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Web.Models;
using AutoMapper;
using DomainTest = TrainingProject.Domain.Models.Test;
using DomainQuestion = TrainingProject.Domain.Models.Question;
using DomainAnswerOption = TrainingProject.Domain.Models.AnswerOption;

namespace TrainingProject.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestManager _testManager;

        private readonly IQuestionManager _questionManager;

        private readonly IAnswersManager _answersManager;

        private readonly IMapper _mapper;
        public TestController(ITestManager testManager, IMapper mapper, IQuestionManager questionManager, IAnswersManager answersManager)
        {
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _questionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _answersManager = answersManager ?? throw new ArgumentNullException(nameof(answersManager));
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public IActionResult ShowTestsToAdmin()
        {
            List<TestModel> tests = _testManager.GetTests().Select(test => _mapper.Map<TestModel>(test))
                .ToList();

            return View(tests);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForUsers")]
        public IActionResult ShowTestsToUsers()
        {
            List<TestModel> tests = _testManager.GetTests()
                .Select(test => _mapper.Map<TestModel>(test))
                .ToList();

            return View(tests);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public IActionResult CreateTest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateTest(TestModel model)
        {
            if (ModelState.IsValid)
            {
                _testManager.CreateTest(model.Name);

                return RedirectToAction("ShowTestsToAdmin", "Test");
            }

            ModelState.AddModelError("", "Invalid Test information");

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> EditTest(int testId)
        {
            var questions = _questionManager.GetQuestionsByTestId(testId)
                .Select(question => _mapper.Map<QuestionModel>(question))
                .ToList();

            var test = _testManager.GetTestById(testId);

            ViewData["TestName"] = test.Name;
            ViewData["TestId"] = test.Id;

            return View(questions);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateQuestion(int testId)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateQuestion(QuestionModel model)
        {
            if (ModelState.IsValid && _testManager.IsTestExists(model.TestId))
            {
                var domainQuestion = _mapper.Map<DomainQuestion>(model);

                _questionManager.CreateQuestion(domainQuestion);

                return RedirectToAction("EditTest", "Test", new { @TestId = model.TestId });
            }

            ModelState.AddModelError("", "Invalid Question information");

            return View(model);
        }


        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> EditQuestion(int testId, int questionId, string questionType)
        {
            if (!_testManager.IsTestExists(testId))
            {
                return BadRequest(testId);
            }

            if (!_questionManager.IsQuestionExists(questionId))
            {
                return BadRequest(questionId);
            }

            if (_questionManager.GetQuestionTypeById(questionId) != questionType)
            {
                return BadRequest(questionType);
            }

            var answerOptions = _answersManager
                .GetAnswerOptionsByQuestionId(questionId)
                .Select(answerOption => _mapper.Map<AnswerOptionModel>(answerOption))
                .ToList();

            ViewData["TestId"] = testId;
            ViewData["QuestionId"] = questionId;
            ViewData["QuestionType"] = questionType;
            ViewData["QuestionText"] = _questionManager.GetQuestionTextById(questionId);

            return View(answerOptions);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(int testId, int questionId, string questionType)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(AnswerOptionModel model)
        {
            if (ModelState.IsValid && 
                _questionManager.IsQuestionExists(model.QuestionId))
            {
                var domainAnswerOption = _mapper.Map<DomainAnswerOption>(model);

                _answersManager.CreateAnswerOption(domainAnswerOption);

                return RedirectToAction("EditQuestion", "Test", 
                    new { @TestId = model.TestId, @QuestionId = model.QuestionId, @QuestionType = model.QuestionType });
            }

            ModelState.AddModelError("", "Invalid Question information");

            return View(model);
        }
    }
}