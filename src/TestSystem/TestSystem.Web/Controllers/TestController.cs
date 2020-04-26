using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Web.Models;
using AutoMapper;
using DomainQuestion = TestSystem.Domain.Models.Question;
using DomainAnswer = TestSystem.Domain.Models.Answer;
using DomainUserAnswer = TestSystem.Domain.Models.UserAnswer;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using DomainTest = TestSystem.Domain.Models.Test;
using TestSystem.Common;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TestSystem.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        private readonly IQuestionManager _questionManager;

        private readonly IAnswersManager _answersManager;

        private readonly IMapper _mapper;

        private readonly int _toSecondsConstant = 60;

        public TestController(ITestManager testManager, IMapper mapper, IQuestionManager questionManager,
            IUserManager userManager, IAnswersManager answersManager)
        {
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _questionManager = questionManager ?? throw new ArgumentNullException(nameof(questionManager));
            _answersManager = answersManager ?? throw new ArgumentNullException(nameof(answersManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> ShowTestsToAdmin()
        {
            IEnumerable<TestModel> tests = (await _testManager.GetTestsAsync())
                .Select(test => _mapper.Map<TestModel>(test));

            return View(tests);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForUsers")]
        public async Task<IActionResult> ShowTestsToUser()
        {
            string userEmail = User.Identity.Name;
            int userId = await _userManager.GetUserIdAsync(userEmail);

            List<int> allTestIds = (await _testManager.GetTestsAsync()).Select(x => x.Id).ToList();
            List<int> userTestIds = (await _testManager.GetUserTestsAsync(userId)).Select(x => x.TestId).ToList();

            foreach (int testId in allTestIds)
            {
                if (!userTestIds.Contains(testId))
                {
                    DomainTest test = await _testManager.GetTestByIdAsync(testId);

                    var domainUserTest = new DomainUserTest
                    {
                        Status = TestStatus.NotStarted,
                        TestId = testId,
                        UserId = userId,
                        TestMinutes = test.Minutes
                    };

                    if (! await _testManager.IsUserTestExistsAsync(userId, testId))
                    {
                        await _testManager.CreateUserTestAsync(domainUserTest);
                    }
                }
            }

            IReadOnlyCollection<UserTestModel> userTests = (await _testManager.GetUserTestsAsync(userId))
                .Select(x => _mapper.Map<UserTestModel>(x)).ToList();

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
                await _testManager.CreateTestAsync(model.Name, model.Minutes);

                return RedirectToAction("ShowTestsToAdmin", "Test");
            }

            ModelState.AddModelError("", "Invalid Test information");

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> EditTest(int testId)
        {
            List<QuestionModel> questions = (await _questionManager.GetQuestionsByTestIdAsync(testId))
                .Select(question => 
                {
                    var questionModel = _mapper.Map<QuestionModel>(question);

                    if (!string.IsNullOrWhiteSpace(question.ImageFullName))
                    {
                        questionModel.ImageLocation = $"/{WebExtensions.ImagesFolderName}/" + 
                            Path.GetFileName(questionModel.ImageFullName);
                    }

                    return questionModel;
                })
                .ToList();

            DomainTest test = await _testManager.GetTestByIdAsync(testId);

            ViewData["TestName"] = test.Name;
            ViewData["TestId"] = test.Id;

            return View(questions);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public IActionResult CreateQuestion(int testId)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateQuestion(QuestionModel model)
        {
            if (ModelState.IsValid && await _testManager.IsTestExistsAsync(model.TestId))
            {
                IFormFile image = model.Image;

                if (image != null)
                {
                    string imageFullName = Path.Combine(WebExtensions.ImagesFolderFullName,
                        Guid.NewGuid() + Path.GetFileName(image.FileName));
                    image.CopyTo(new FileStream(imageFullName, FileMode.Create));
                    model.ImageFullName = imageFullName;
                }

                var domainQuestion = _mapper.Map<DomainQuestion>(model);

                await _questionManager.CreateQuestionAsync(domainQuestion);

                return RedirectToAction("EditTest", "Test", new { @TestId = model.TestId });
            }

            ModelState.AddModelError("", "Invalid Question information");

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> EditQuestion(int testId, int questionId, string questionType)
        {
            if (! await _testManager.IsTestExistsAsync(testId))
            {
                return BadRequest();
            }

            if (! await _questionManager.IsQuestionExistsAsync(questionId))
            {
                return BadRequest();
            }

            if (await _questionManager.GetQuestionTypeByIdAsync(questionId) != questionType)
            {
                return BadRequest();
            }

            List<AnswerModel> answers = (await _answersManager.GetAnswersByQuestionIdAsync(questionId))
                .Select(answer => _mapper.Map<AnswerModel>(answer))
                .ToList();

            ViewData["TestId"] = testId;
            ViewData["QuestionId"] = questionId;
            ViewData["QuestionType"] = questionType;
            ViewData["QuestionText"] = await _questionManager.GetQuestionTextByIdAsync(questionId);
            ViewData["AnswersCount"] = await _answersManager.GetAnswerCountByQuestionIdAsync(questionId);

            return View(answers);
        }

        [HttpGet]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(int testId, int questionId, string questionType)
        {
            if (!await _testManager.IsTestExistsAsync(testId))
            {
                return BadRequest();
            }

            if (!await _questionManager.IsQuestionExistsAsync(questionId))
            {
                return BadRequest();
            }

            if (await _questionManager.GetQuestionTypeByIdAsync(questionId) != questionType)
            {
                return BadRequest();
            }

            ViewData["TestId"] = testId;
            ViewData["QuestionId"] = questionId;
            ViewData["QuestionType"] = questionType;

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OnlyForAdmins")]
        public async Task<IActionResult> CreateAnswer(AnswerModel model)
        {
            if (ModelState.IsValid && await _questionManager.IsQuestionExistsAsync(model.QuestionId))
            {
                var domainAnswer = _mapper.Map<DomainAnswer>(model);
                int answersCount = await _answersManager.GetAnswerCountByQuestionIdAsync(model.QuestionId);

                if (model.QuestionType == QuestionTypes.Text && answersCount >= 1)
                {
                    return BadRequest();
                }

                await _answersManager.CreateAnswerAsync(domainAnswer);

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
            string  userEmail = User.Identity.Name;
            int userId = await _userManager.GetUserIdAsync(userEmail);
            DomainUserTest userTest = await _testManager.GetUserTestAsync(userId, testId);
            List<int> testStages = (await _questionManager.GetTestStagesByTestIdAsync(testId)).ToList();
            int stagesCount = testStages.Count;
            var userQuestionIds = new List<int>();
            DomainQuestion currQuestion = null;
            List<UserAnswerModel> currQuestionUserAnswers = null;
            string currQuestionType = string.Empty;
            string currQuestionImageLocation = string.Empty;
            int currQuestionStage = 0;
            int secondsLeft = 0;
            DateTime startTime = default;

            if (userTest.Status == TestStatus.Finished)
            {
                return RedirectToAction("EndTest");
            }
            else if (userTest.Status == TestStatus.NotStarted)
            {
                await _testManager.UpdateUserTestStatusAsync(userId, testId, TestStatus.NotFinished);

                userQuestionIds = await CreateUserAnswersAndGetQuestionIdsAsync(testId, userId);
                currQuestion = await _questionManager.GetQuestionByIdAsync(userQuestionIds[0]);
                secondsLeft = userTest.TestMinutes * _toSecondsConstant;
                startTime = DateTime.Now;

                await _testManager.UpdateUserTestStartTimeAsync(userId, testId, startTime);
            }
            else if (userTest.Status == TestStatus.NotFinished)
            {
                secondsLeft = userTest.TestMinutes * _toSecondsConstant - 
                    Convert.ToInt32(Math.Abs((userTest.StartTime - DateTime.Now).TotalSeconds));

                if (secondsLeft <= 0)
                {
                    await _testManager.UpdateUserTestStatusAsync(userId, testId, TestStatus.Finished);

                    return BadRequest("Time's been expired.");
                }

                (await _questionManager.GetUserQuestionsByTestIdAsync(userId, testId)).ToList()
                    .ForEach(question => userQuestionIds.Add(question.Id));

                currQuestion = await _questionManager.GetQuestionByIdAsync(userQuestionIds[0]);
            }
            
            currQuestionUserAnswers = (await _answersManager.GetUserAnswersByQuestionIdAsync(userId, currQuestion.Id))
                .Select(x => _mapper.Map<UserAnswerModel>(x)).ToList();
            currQuestionStage = currQuestion.Stage;
            currQuestionType = currQuestion.QuestionType;

            if (!string.IsNullOrWhiteSpace(currQuestion.ImageFullName))
            {
                currQuestionImageLocation = $"/{WebExtensions.ImagesFolderName}/" + Path.GetFileName(currQuestion.ImageFullName);
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
            Dictionary<string, string> dictReq = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            string submitButtonKey = dictReq.Keys.Single(x => x.Contains("SubmitButton"));
            string submitButtonValue = dictReq[submitButtonKey];
            int testId = int.Parse(dictReq["TestId"]);
            int userId = int.Parse(dictReq["UserId"]);
            DomainUserTest userTest = await _testManager.GetUserTestAsync(userId, testId);
            int stagesCount = int.Parse(dictReq["StagesCount"]);
            int currQuestionId = int.Parse(dictReq["CurrQuestionId"]);
            int currQuestionStage = int.Parse(dictReq["CurrQuestionStage"]);
            string currQuestionType = dictReq["CurrQuestionType"];
            List<int> userQuestionIds = dictReq["UserQuestionIds"].Split(",").Select(x => int.Parse(x)).ToList();
            var answerKeySubStr = "AnswerId";
            var currQuestionImageLocation = string.Empty;

            int secondsLeft = userTest.TestMinutes * 60 - Convert.ToInt32(Math.Abs((userTest.StartTime - DateTime.Now).TotalSeconds));
            if (secondsLeft <= 0)
            {
                await FinishTestAsync(userId, testId);

                return PartialView("_EndTest"); // to redirect from ajax post
            }

            if (currQuestionType == QuestionTypes.Option)
            {
                List<string> answersKeys = dictReq.Keys.Where(x => x.Contains(answerKeySubStr)).ToList();

                foreach (string answerKey in answersKeys)
                {
                    int answerId = int.Parse(answerKey.Substring(answerKeySubStr.Length));
                    bool isValid = dictReq[answerKey].Contains("true") ? true : false;

                    await _answersManager.UpdateUserAnswerValidAsync(userId, answerId, isValid);
                }
            }
            else if (currQuestionType == QuestionTypes.Text)
            {
                string answerKey = dictReq.Keys.First(x => x.Contains(answerKeySubStr));
                int answerId = int.Parse(answerKey.Substring(answerKeySubStr.Length));
                string userAnswerText = dictReq[answerKey];

                await _answersManager.UpdateUserAnswerTextAsync(userId, answerId, userAnswerText);
            }

            ViewData["SubmitButton_1"] = "Next";
            ViewData["SubmitButton_2"] = "Back";
            SetSubmitButtonsTextAndChangeCurrQuestionStage(submitButtonValue, stagesCount, ref currQuestionStage);

            if (submitButtonValue == "Finish" && currQuestionStage == stagesCount)
            {
                await FinishTestAsync(userId, testId);

                return PartialView("_EndTest");
            }

            DomainQuestion currQuestion = await _questionManager.GetQuestionByIdAsync(userQuestionIds[currQuestionStage - 1]);
            List<UserAnswerModel> currQuestionUserAnswers = (await _answersManager.GetUserAnswersByQuestionIdAsync(userId, currQuestion.Id))
                .Select(x => _mapper.Map<UserAnswerModel>(x))
                .ToList();

            if (!string.IsNullOrWhiteSpace(currQuestion.ImageFullName))
            {
                currQuestionImageLocation = $"/{WebExtensions.ImagesFolderName}/" +  Path.GetFileName(currQuestion.ImageFullName);
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
            await FinishTestAsync(userId, testId);

            return Ok();
        }

        private async Task<List<int>> CreateUserAnswersAndGetQuestionIdsAsync(int testId, int userId)
        {
            var questionIds = new List<int>();
            int stagesCount = (await _questionManager.GetTestStagesByTestIdAsync(testId)).ToList().Count;

            for (int stage = 1; stage <= stagesCount; stage++)
            {
                int questionId = (await _questionManager.GetRandomQuestionInTestByStageAsync(testId, stage)).Id;

                List<DomainAnswer> answers = (await _answersManager.GetAnswersByQuestionIdAsync(questionId)).ToList();

                foreach (DomainAnswer answer in answers)
                {
                    var userAnswer = new DomainUserAnswer
                    {
                        isValid = false,
                        Text = string.Empty,
                        UserId = userId,
                        AnswerId = answer.Id
                    };

                    if (! await _answersManager.IsUserAnswerExistsAsync(userId, answer.Id))
                    {
                        await _answersManager.CreateUserAnswerAsync(userAnswer);
                    }
                }

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

        private async Task FinishTestAsync(int userId, int testId)
        {
            await _testManager.UpdateUserTestStatusAsync(userId, testId, TestStatus.Finished);

            List<DomainQuestion> questions = (await _questionManager.GetUserQuestionsByTestIdAsync(userId, testId)).ToList();
            int points = 0;
            foreach (var question in questions)
            {
                if (question.QuestionType == QuestionTypes.Option)
                {
                    var correctAnswerIds = (await _answersManager.GetAnswersByQuestionIdAsync(question.Id))
                        .Where(x => x.IsValid == true)
                        .Select(x => x.Id)
                        .ToHashSet();
                    var userSelectedAnswerIds = (await _answersManager.GetUserAnswersByQuestionIdAsync(userId, question.Id))
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
                    DomainAnswer answer = (await _answersManager.GetAnswersByQuestionIdAsync(question.Id)).Single();
                    DomainUserAnswer userAnswer = (await _answersManager.GetUserAnswersByQuestionIdAsync(userId, question.Id))
                        .Single();

                    if (answer.Text == userAnswer.Text)
                    {
                        points += question.Points;
                    }
                }
            }

            await _testManager.UpdateUserTestPointsAsync(userId, testId, points);
        }
    }
}