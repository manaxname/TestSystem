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
using DomainUserAnswerOption = TrainingProject.Domain.Models.UserAnswerOption;

namespace TrainingProject.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        private readonly IQuestionManager _questionManager;

        private readonly IAnswersManager _answersManager;

        private readonly IMapper _mapper;
        public TestController(ITestManager testManager, IMapper mapper,
            IQuestionManager questionManager, IUserManager userManager, IAnswersManager answersManager)
        {
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _questionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _answersManager = answersManager ?? throw new ArgumentNullException(nameof(answersManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
        public IActionResult ShowTestsToUser()
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

        [HttpGet]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> StartTest(int testId)
        {
            if (!_testManager.IsTestExists(testId))
            {
                return BadRequest(testId);
            }

            var userEmail = User.Identity.Name;
            var userId = _userManager.GetUserId(userEmail);

            var currQuestionStage = 1;
            var currQuestion = _questionManager.GetRandomQuestionInTestByStage(testId, currQuestionStage);
            var answerOptions = _answersManager
                .GetAnswerOptionsByQuestionId(currQuestion.Id)
                .Select(domainAnswerOption => _mapper.Map<AnswerOptionModel>(domainAnswerOption))
                .ToList();

            ViewData["TestId"] = testId;
            ViewData["CurrQuestionId"] = currQuestion.Id;
            ViewData["CurrQuestionText"] = currQuestion.Text;
            ViewData["CurrQuestionStage"] = currQuestionStage;
            ViewData["UserId"] = userId;

            return View(answerOptions);
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> StartTest()
        {
            var b = 1;

            var dictReq = Request.Form
                .ToDictionary(x => x.Key, x => x.Value.ToString());

            // TODO: check if exists.
            var userId = int.Parse(dictReq["userId"]);
            var testId = int.Parse(dictReq["testId"]);
            var prevQuestionStage = int.Parse(dictReq["currQuestionStage"]);
            int prevQuestionId = int.Parse(dictReq["currQuestionId"]);

            if (!_userManager.IsUserExists(userId))
            {
                return BadRequest();
            }

            if (!_testManager.IsTestExists(testId))
            {
                return BadRequest();
            }

            if (!_questionManager.IsQuestionExists(prevQuestionId))
            {
                return BadRequest();
            }

            var userSelectedAnswerOptionIds = dictReq
                .Where(x => x.Value == "on")
                .Select(kvp => int.Parse(kvp.Key)).ToList();

            foreach (var selectedId in userSelectedAnswerOptionIds)
            {
                if (!_answersManager.IsAnswerOptionExists(selectedId))
                {
                    return BadRequest();
                }
            }

            foreach (var answerOptionId in userSelectedAnswerOptionIds)
            {
                var domainUserAnswerOption = new DomainUserAnswerOption
                {
                    AnswerOptionId = answerOptionId,
                    isValid = true, // user selected this answeres => true
                    UserId = userId
                };

                _answersManager.CreateUserAnswerOption(domainUserAnswerOption);
            }

            // next?? prev??
            var currQuestionStage = prevQuestionStage + 1;
            // if exists reload??
            var currQuestion = _questionManager.GetRandomQuestionInTestByStage(testId, currQuestionStage);
            var answerOptions = _answersManager
                .GetAnswerOptionsByQuestionId(currQuestion.Id)
                .Select(domainAnswerOption => _mapper.Map<AnswerOptionModel>(domainAnswerOption))
                .ToList();

            ViewData["TestId"] = testId;
            ViewData["UserId"] = userId;

            ViewData["CurrQuestionId"] = currQuestion.Id;
            ViewData["CurrQuestionText"] = currQuestion.Text;
            ViewData["CurrQuestionStage"] = currQuestionStage;

            ViewData["PrevQuestionId"] = prevQuestionId;
            ViewData["PrevQuestionStage"] = prevQuestionStage;


            return PartialView("_StartTest", answerOptions);
        }
    }
}