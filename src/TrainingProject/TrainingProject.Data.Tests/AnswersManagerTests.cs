using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingProject.Common;
using TrainingProject.Domain.Logic;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Logic.Managers;

namespace TrainingProject.Data.Tests
{
    [TestFixture]
    internal class AnswersManagerTests : TestBase
    {
        private readonly IAnswersManager _answerManager;

        private readonly IQuestionManager _questionManager;

        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        public AnswersManagerTests()
        {
            _userManager = new UserManager(base.DbContext, base.Mapper);
            _testManager = new TestManager(base.DbContext, base.Mapper);
            _questionManager = new QuestionManager(base.DbContext, _testManager, base.Mapper);
            _answerManager = new AnswersManager(base.DbContext,_questionManager, base.Mapper);
        }

        [TestCase("testName1", "text1", 1, 1, 2, "answOptText1", true)]
        [TestCase("testName2", "text2", 2, 2, 2, "answOptText2", false)]
        [TestCase("testName2", "text2", 2, 2, 2, "answOptText2", false)]
        [TestCase("testName3", "text1", 3, 3, 2, "answOptText3", true)]
        public void CreateAnswersOption_Test(string testName, string text, int stage, 
            int points, int questType, string answerText, bool isValid)
        {
            int testId = _testManager.CreateTest(testName);

            var questionType = questType == 1 ? QuestionTypes.Text : QuestionTypes.Options;
            var domainQuestion = Helper.CreateDomainQuestion(text, stage, points, questionType, testId);

            var questionId = _questionManager.CreateQuestion(domainQuestion);

            var answerWithOptrion = Helper.CreateDomainAnswerWithOption(answerText, isValid, questionId);

            _answerManager.CreateAnswerOption(answerWithOptrion);
        }

        [TestCase("testName1", "text1", 1, 1, 2, "answOptText1", true, "email1", "passw1", "role1", true)]
        [TestCase("testName2", "text2", 2, 2, 2, "answOptText2", false, "email2", "passw2", "role1", true)]
        [TestCase("testName2", "text2", 2, 2, 2, "answOptText2", false, "email3", "passw3", "role1", false)]
        [TestCase("testName3", "text1", 3, 3, 2, "answOptText3", true, "email4", "passw4", "role1", false)]
        public void CreateUserAnswersOption_Test(
            string testName,
            string text, int stage, int points, int questType, string answerText, bool isValid,
            string email, string passwordHash, string role,
            bool isValidUserAnswer)
        {
            int testId = _testManager.CreateTest(testName);

            var questionType = questType == 1 ? QuestionTypes.Text : QuestionTypes.Options;
            var domainQuestion = Helper.CreateDomainQuestion(text, stage, points, questionType, testId);
            var questionId = _questionManager.CreateQuestion(domainQuestion);

            var domainAnswerWithOption = Helper.CreateDomainAnswerWithOption(answerText, isValid, questionId);
            var answerWithOptionId = _answerManager.CreateAnswerOption(domainAnswerWithOption);

            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);
            var userId = _userManager.CreateUser(domainUser);

            var domainUserAnswerOption = Helper.CreateDomainUserAnswerWithOption(userId, answerWithOptionId, isValidUserAnswer);
            _answerManager.CreateUserAnswerOption(domainUserAnswerOption);
        }
    }
}
