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
using System.Text.RegularExpressions;

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
            var userEmail = User.Identity.Name;
            var userId = _userManager.GetUserId(userEmail);
            var testStages = _questionManager.GetTestStagesByTestId(testId).ToList();
            int stagesCount = testStages.Count;
            var questionIds = new List<int>();
            
            for (int stage = 1; stage <= stagesCount; stage++)
            {
                int questionId = _questionManager.GetRandomQuestionInTestByStage(testId, stage).Id;
                _answersManager
                    .GetAnswerOptionsByQuestionId(questionId)
                    .ToList()
                    .ForEach(answerOption =>
                    {
                        var userAnswerOption = new DomainUserAnswerOption
                        {
                            AnswerOptionId = answerOption.Id,
                            isValid = false,
                            UserId = userId
                        };

                        _answersManager.CreateUserAnswerOption(userAnswerOption);
                    });

                questionIds.Add(questionId);
            }

            var currQuestion = _questionManager.GetQuestionById(questionIds[0]);
            var currQuestionUserAnswersOptions = _answersManager
                .GetUserAnswerOptionsByQuestionId(userId, currQuestion.Id)
                .Select(x => _mapper.Map<UserAnswerOptionModel>(x));

            var currQuestionStage = currQuestion.Stage;

            var startTest = new StartTestModel()
            {
                UserId = userId,
                TestId = testId,
                CurrQuestionId = currQuestion.Id,
                CurrQuestionText = currQuestion.Text,
                CurrQuestionStage= currQuestion.Stage,
                StagesCount = stagesCount,

                QuestionIds = string.Join(",", questionIds),
                CurrQuestionUserAnswersOptions = currQuestionUserAnswersOptions,
            };

            if (currQuestionStage == stagesCount)
            {
                ViewData["SubmitButton1"] = "Finish";
            }
            else
            {
                ViewData["SubmitButton1"] = "Next";
            }

            ViewData["SubmitButton2"] = string.Empty;

            return View(startTest);
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> StartTest()
        {
            var dictReq = Request.Form
                   .ToDictionary(x => x.Key, x => x.Value.ToString());

            string submitButtonKey = dictReq.ElementAt(0).Key;
            string submitButtonValue = dictReq.ElementAt(0).Value;

            var testId = int.Parse(dictReq["TestId"]);
            var userId = int.Parse(dictReq["UserId"]);
            var stagesCount = int.Parse(dictReq["StagesCount"]);
            var currQuestionId = int.Parse(dictReq["CurrQuestionId"]);
            var currQuestionStage = int.Parse(dictReq["CurrQuestionStage"]);
            var questionIds = dictReq["QuestionIds"]
                .Split(",")
                .Select(x => int.Parse(x))
                .ToList();

            var u = 1;

            var userAnswerOptionIds = dictReq
                .Where(x => x.Value == "true,false" || x.Value == "false")
                .Select(kvp => 
                {
                    bool isValid = false;

                    if (kvp.Value == "true,false")
                    {
                        isValid = true;
                    }

                    var newkvp = new KeyValuePair<int, bool>
                    (
                        int.Parse(kvp.Key),
                        isValid
                    );

                    return newkvp;
                })
                .ToList();

            userAnswerOptionIds.ForEach(x => {
                if(x.Value)
                {
                    _answersManager.SetUserAnswerOptionValid(userId, x.Key, x.Value);
                }
                else
                {
                    _answersManager.SetUserAnswerOptionValid(userId, x.Key, x.Value);
                }
            });

            ViewData["SubmitButton1"] = "Next";
            ViewData["SubmitButton2"] = "Back";

            if (submitButtonValue == "Next" && currQuestionStage < stagesCount - 1)
            {
                ViewData["SubmitButton1"] = "Next";
                currQuestionStage++;
            }
            else if (submitButtonValue == "Next" && currQuestionStage == stagesCount - 1)
            {
                ViewData["SubmitButton1"] = "Finish";
                currQuestionStage++;
            }
            else if (submitButtonValue == "Back" && currQuestionStage > 2)
            {
                ViewData["SubmitButton2"] = "Back";
                currQuestionStage--;
            }
            else if (submitButtonValue == "Back" && currQuestionStage == 2)
            {
                ViewData["SubmitButton1"] = "Next";
                ViewData["SubmitButton2"] = string.Empty;
                currQuestionStage--;
            }

            if (submitButtonValue != "Next" && submitButtonValue != "Back")
            {
                 currQuestionStage = int.Parse(submitButtonValue);
            }
            if (currQuestionStage == stagesCount)
            {
                ViewData["SubmitButton1"] = "Finish";
            }

            if (submitButtonValue == "Finish" && currQuestionStage == stagesCount)
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
            }

            var currQuestion = _questionManager.GetQuestionById(questionIds[currQuestionStage - 1]);
            var currQuestionUserAnswersOptions = _answersManager
                .GetUserAnswerOptionsByQuestionId(userId, currQuestion.Id)
                .Select(x => _mapper.Map<UserAnswerOptionModel>(x));

            var arrrr = currQuestionUserAnswersOptions.ToList();

            ViewData["TestId"] = testId;
            ViewData["UserId"] = userId;
            ViewData["CurrQuestionText"] = currQuestion.Text;
            ViewData["CurrQuestionStage"] = currQuestionStage;
            ViewData["StagesCount"] = stagesCount;

            ViewData["CurrQuestionId"] = currQuestion.Id;
            ViewData["QuestionIds"] = dictReq["QuestionIds"];
            ViewData["CurrQuestionUserAnswersOptions"] = currQuestionUserAnswersOptions;

            return PartialView("_StartTest");
        }
    }
}