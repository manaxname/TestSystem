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
using DomainAnswer = TrainingProject.Domain.Models.Answer;
using DomainUserAnswer = TrainingProject.Domain.Models.UserAnswer;
using DomainUserTest = TrainingProject.Domain.Models.UserTest;
using System.Text.RegularExpressions;
using TrainingProject.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TrainingProject.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        private readonly IQuestionManager _questionManager;

        private readonly IAnswersManager _answersManager;

        private readonly IMapper _mapper;

        private readonly IHostingEnvironment _hostingEnvironment;

        public TestController(ITestManager testManager, IMapper mapper,
            IQuestionManager questionManager, IUserManager userManager, IAnswersManager answersManager, IHostingEnvironment hostingEnvironment)
        {
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _questionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _answersManager = answersManager ?? throw new ArgumentNullException(nameof(answersManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public IActionResult ShowTestsToAdmin()
        {
            List<TestModel> tests = _testManager.GetTests().Select(test => _mapper.Map<TestModel>(test)).ToList();

            return View(tests);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForUsers")]
        public IActionResult ShowTestsToUser()
        {
            var userEmail = User.Identity.Name;
            var userId = _userManager.GetUserId(userEmail);

            var allTestIds = _testManager.GetTests().Select(x => x.Id).ToList();
            var userTestIds = _testManager.GetUserTests(userId).Select(x => x.TestId).ToList();

            foreach (var testId in allTestIds)
            {
                if (!userTestIds.Contains(testId))
                {
                    var test = _testManager.GetTestById(testId);
                    var domainUserTest = new DomainUserTest
                    {
                        Status = TestStatus.NotStarted,
                        TestId = testId,
                        UserId = userId,
                        TestMinutes = test.Minutes
                    };

                    if (!_testManager.IsUserTestExists(userId, testId))
                    {
                        _testManager.CreateUserTest(domainUserTest);
                    }
                }
            }

            var userTests = _testManager.GetUserTests(userId)
                .Select(x => _mapper.Map<UserTestModel>(x));

            return View(userTests);
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
                _testManager.CreateTest(model.Name, model.Minutes);

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
                .Select(question => {
                    var questionModel = _mapper.Map<QuestionModel>(question);

                    if (!string.IsNullOrWhiteSpace(question.ImageFullName))
                    {
                        questionModel.ImageLocation = "/questionImages/" + Path.GetFileName(questionModel.ImageFullName);
                    }

                    return questionModel;
                })
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
                IFormFile image = model.Image;

                if (image != null)
                {
                    string imageFullName = Path.Combine(_hostingEnvironment.WebRootPath + "\\questionImages",
                        Guid.NewGuid() + Path.GetFileName(image.FileName));
                    image.CopyTo(new FileStream(imageFullName, FileMode.Create));
                    model.ImageFullName = imageFullName;
                }

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

            var answers = _answersManager
                .GetAnswersByQuestionId(questionId)
                .Select(answer => _mapper.Map<AnswerModel>(answer))
                .ToList();

            ViewData["TestId"] = testId;
            ViewData["QuestionId"] = questionId;
            ViewData["QuestionType"] = questionType;
            ViewData["QuestionText"] = _questionManager.GetQuestionTextById(questionId);
            ViewData["AnswersCount"] = _answersManager.GetAnswerCountByQuestionId(questionId);

            return View(answers);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(int testId, int questionId, string questionType)
        {
            ViewData["TestId"] = testId;
            ViewData["QuestionId"] = questionId;
            ViewData["QuestionType"] = questionType;

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(AnswerModel model)
        {
            if (ModelState.IsValid && _questionManager.IsQuestionExists(model.QuestionId))
            {
                var domainAnswer = _mapper.Map<DomainAnswer>(model);
                var answersCount = _answersManager.GetAnswerCountByQuestionId(model.QuestionId);

                if (model.QuestionType == QuestionTypes.Text && answersCount >= 1)
                {
                    return BadRequest();
                }

                _answersManager.CreateAnswer(domainAnswer);

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
            var userTest = _testManager.GetUserTest(userId, testId);
            var testStages = _questionManager.GetTestStagesByTestId(testId).ToList();
            var stagesCount = testStages.Count;
            var userQuestionIds = new List<int>();
            DomainQuestion currQuestion = null;
            IEnumerable<UserAnswerModel> currQuestionUserAnswers = null;
            var currQuestionType = string.Empty;
            var currQuestionImageLocation = string.Empty;
            int currQuestionStage = 0;
            int secondsLeft = 0;
            DateTime startTime = default;

            if (userTest.Status == TestStatus.Finished)
            {
                return RedirectToAction("EndTest");
            }
            else if (userTest.Status == TestStatus.NotStarted)
            {
                _testManager.UpdateUserTestStatus(userId, testId, TestStatus.NotFinished);

                userQuestionIds = CreateUserAnswersAndGetQuestionIds(testId, userId);
                currQuestion = _questionManager.GetQuestionById(userQuestionIds[0]);
                secondsLeft = userTest.TestMinutes * 60;
                startTime = DateTime.Now;

                _testManager.UpdateUserTestStartTime(userId, testId, startTime);
            }
            else if (userTest.Status == TestStatus.NotFinished)
            {
                secondsLeft = userTest.TestMinutes * 60 - Convert.ToInt32(Math.Abs((userTest.StartTime - DateTime.Now).TotalSeconds));
                if (secondsLeft <= 0)
                {
                    _testManager.UpdateUserTestStatus(userId, testId, TestStatus.Finished);
                    return BadRequest("Time's been expired.");
                }

                _questionManager.GetUserQuestionsByTestId(userId, testId).ToList()
                    .ForEach(question => userQuestionIds.Add(question.Id));

                currQuestion = _questionManager.GetQuestionById(userQuestionIds[0]);
            }
            
            currQuestionUserAnswers = _answersManager.GetUserAnswersByQuestionId(userId, currQuestion.Id)
                .Select(x => _mapper.Map<UserAnswerModel>(x));
            currQuestionStage = currQuestion.Stage;
            currQuestionType = currQuestion.QuestionType;
            if (!string.IsNullOrWhiteSpace(currQuestion.ImageFullName))
            {
                currQuestionImageLocation = "/questionImages/" + Path.GetFileName(currQuestion.ImageFullName);
            }

            var startTest = new StartTestModel()
            {
                UserId = userId,
                TestId = testId,
                StagesCount = stagesCount,
                StartTime = startTime,
                TestMinutes = userTest.TestMinutes,
                SecondsLeft = secondsLeft,
                UserQuestionIds = string.Join(",", userQuestionIds),

                CurrQuestionId = currQuestion.Id,
                CurrQuestionText = currQuestion.Text,
                CurrQuestionStage = currQuestion.Stage,
                CurrQuestionType = currQuestionType,
                CurrQuestionUserAnswers = currQuestionUserAnswers,
                CurrQuestionImageLocation = currQuestionImageLocation
            };

            ViewData["SubmitButton_1"] = "Next";
            ViewData["SubmitButton_2"] = string.Empty;

            if (stagesCount == 1)
            {
                ViewData["SubmitButton_1"] = "Finish";
            }
            
            return View(startTest);
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> StartTest()
        {
            var dictReq = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var submitButtonKey = dictReq.Keys.Single(x => x.Contains("SubmitButton"));
            var submitButtonValue = dictReq[submitButtonKey];
            var testId = int.Parse(dictReq["TestId"]);
            var userId = int.Parse(dictReq["UserId"]);
            var userTest = _testManager.GetUserTest(userId, testId);
            var stagesCount = int.Parse(dictReq["StagesCount"]);
            var currQuestionId = int.Parse(dictReq["CurrQuestionId"]);
            var currQuestionStage = int.Parse(dictReq["CurrQuestionStage"]);
            var currQuestionType = dictReq["CurrQuestionType"];
            var userQuestionIds = dictReq["UserQuestionIds"].Split(",").Select(x => int.Parse(x)).ToList();
            var answerKeySubStr = "AnswerId";
            var currQuestionImageLocation = string.Empty;

            int secondsLeft = userTest.TestMinutes * 60 - Convert.ToInt32(Math.Abs((userTest.StartTime - DateTime.Now).TotalSeconds));
            if (secondsLeft <= 0)
            {
                FinishTest(userId, testId);

                return PartialView("_EndTest"); // to redirect from ajax post
            }

            if (currQuestionType == QuestionTypes.Option)
            {
                dictReq.Keys.Where(x => x.Contains(answerKeySubStr)).ToList()
                    .ForEach(key => {
                        int answerId = int.Parse(key.Substring(answerKeySubStr.Length));
                        bool isValid = dictReq[key].Contains("true") ? true : false;

                        _answersManager.UpdateUserAnswerValid(userId, answerId, isValid);
                    });
            }
            else if (currQuestionType == QuestionTypes.Text)
            {
                string answerKey = dictReq.Keys.First(x => x.Contains(answerKeySubStr));
                int answerId = int.Parse(answerKey.Substring(answerKeySubStr.Length));
                string userAnswerText = dictReq[answerKey];

                _answersManager.UpdateUserAnswerText(userId, answerId, userAnswerText);
            }

            ViewData["SubmitButton_1"] = "Next";
            ViewData["SubmitButton_2"] = "Back";
            SetSubmitButtonsTextAndChangeCurrQuestionStage(submitButtonValue, stagesCount, ref currQuestionStage);

            if (submitButtonValue == "Finish" && currQuestionStage == stagesCount)
            {
                FinishTest(userId, testId);

                return PartialView("_EndTest");
            }

            var currQuestion = _questionManager.GetQuestionById(userQuestionIds[currQuestionStage - 1]);
            var currQuestionUserAnswers = _answersManager.GetUserAnswersByQuestionId(userId, currQuestion.Id)
                .Select(x => _mapper.Map<UserAnswerModel>(x));
            if (!string.IsNullOrWhiteSpace(currQuestion.ImageFullName))
            {
                currQuestionImageLocation = "/questionImages/" + Path.GetFileName(currQuestion.ImageFullName);
            }

            var startTest = new StartTestModel()
            {
                TestId = testId,
                UserId = userId,
                StagesCount = stagesCount,
                TestMinutes = userTest.TestMinutes,
                SecondsLeft = secondsLeft,
                UserQuestionIds = dictReq["UserQuestionIds"],

                CurrQuestionId = currQuestion.Id,
                CurrQuestionText = currQuestion.Text,
                CurrQuestionStage = currQuestion.Stage,
                CurrQuestionType = currQuestion.QuestionType,
                CurrQuestionUserAnswers = currQuestionUserAnswers,
                CurrQuestionImageLocation = currQuestionImageLocation
            };

            return PartialView("_StartTest", startTest);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> EndTest(int userId , int testId)
        {
            FinishTest(userId, testId);

            return Ok();
        }

        private List<int> CreateUserAnswersAndGetQuestionIds(int testId, int userId)
        {
            var questionIds = new List<int>();
            var stagesCount = _questionManager.GetTestStagesByTestId(testId).ToList().Count;

            for (int stage = 1; stage <= stagesCount; stage++)
            {
                int questionId = _questionManager.GetRandomQuestionInTestByStage(testId, stage).Id;
                string questionType = _questionManager.GetQuestionTypeById(questionId);

                _answersManager.GetAnswersByQuestionId(questionId).ToList()
                    .ForEach(answer =>
                    {
                        var userAnswer = new DomainUserAnswer
                        {
                            isValid = false,
                            Text = string.Empty,
                            UserId = userId,
                            AnswerId = answer.Id
                        };

                        if (!_answersManager.IsUserAnswerExists(userId, answer.Id))
                        {
                            _answersManager.CreateUserAnswer(userAnswer);
                        }
                    });

                questionIds.Add(questionId);
            }

            return questionIds;
        }
        
        private void SetSubmitButtonsTextAndChangeCurrQuestionStage(string submitButtonValue, int stagesCount, ref int currQuestionStage)
        {
            if (submitButtonValue == "Next" && currQuestionStage < stagesCount - 1)
            {
                ViewData["SubmitButton_1"] = "Next";
                currQuestionStage++;
            }
            else if (submitButtonValue == "Next" && currQuestionStage == stagesCount - 1)
            {
                ViewData["SubmitButton_1"] = "Finish";
                currQuestionStage++;
            }
            else if (submitButtonValue == "Back" && currQuestionStage > 2)
            {
                ViewData["SubmitButton_2"] = "Back";
                currQuestionStage--;
            }
            else if (submitButtonValue == "Back" && currQuestionStage == 2)
            {
                ViewData["SubmitButton_1"] = "Next";
                ViewData["SubmitButton_2"] = string.Empty;
                currQuestionStage--;
            }

            // if pagination button was clicked
            if (submitButtonValue != "Next" && submitButtonValue != "Back" && submitButtonValue != "Finish")
            {
                currQuestionStage = int.Parse(submitButtonValue);
            }

            if (currQuestionStage == stagesCount)
            {
                ViewData["SubmitButton_1"] = "Finish";
            }
            else if (currQuestionStage == 1)
            {
                ViewData["SubmitButton_2"] = string.Empty;
            }
        }

        private void FinishTest(int userId, int testId)
        {
            _testManager.UpdateUserTestStatus(userId, testId, TestStatus.Finished);

            var questions = _questionManager.GetUserQuestionsByTestId(userId, testId).ToList();
            int points = 0;
            foreach (var question in questions)
            {
                if (question.QuestionType == QuestionTypes.Option)
                {
                    var correctAnswerIds = _answersManager
                        .GetAnswersByQuestionId(question.Id)
                        .Where(x => x.IsValid == true)
                        .Select(x => x.Id)
                        .ToHashSet();
                    var userSelectedAnswerIds = _answersManager
                        .GetUserAnswersByQuestionId(userId, question.Id)
                        .Where(x => x.isValid == true)
                        .Select(x => x.AnswerId)
                        .ToHashSet();
                    if (correctAnswerIds.SetEquals(userSelectedAnswerIds))
                    {
                        points += question.Points;
                    }
                }
                else if (question.QuestionType == QuestionTypes.Text)
                {
                    var answer = _answersManager.GetAnswerById(question.Id);
                    var userAnswer = _answersManager.GetUserAnswersByQuestionId(userId, question.Id).Single();
                    if (answer.Text == userAnswer.Text)
                    {
                        points += question.Points;
                    }
                }
            }

            _testManager.UpdateUserTestPoints(userId, testId, points);
        }
    }
}