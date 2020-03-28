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
    internal class QuestionManagerTests : TestBase
    {
        private readonly IQuestionManager _questionManager;

        private readonly ITestManager _testManager;

        public QuestionManagerTests()
        {
            _testManager = new TestManager(base.DbContext, base.Mapper);
            _questionManager = new QuestionManager(base.DbContext, _testManager, base.Mapper);
        }

        [TestCase("testName1", "text1", 1, 1, "options")]
        [TestCase("testName2", "text2", 2, 2, "options")]
        [TestCase("testName3", "text3", 3, 3, "options")]
        public void CreateQuestion_Test(string testName, string text, int stage, int points, string questType)
        {
            int testId = _testManager.CreateTest(testName);
            int questionId = _questionManager.CreateQuestion(testId, text, stage, points, questType);
        }
    }
}
