using System;
using System.Collections.Generic;
using System.Linq;
using TrainingProject.Common;
using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic
{
    public static class Helper
    {
        public static User CreateDomainUser(string email, string passwordHash, string role)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };

            return user;
        }

        public static Test CreateDomainTest(string name, int time)
        {
            var test = new Test
            {
                Name = name,
                Minutes = time
            };

            return test;
        }

        public static Question CreateDomainQuestion(string text, int stage, int points, string questionType, int testId)
        {
            var question = new Question
            {
                Text = text,
                Stage = stage,
                Points = points,
                QuestionType = questionType,
                TestId = testId,
            };

            return question;
        }

        public static AnswerOption CreateDomainAnswerWithOption(string text, bool isValid, int QuestionId)
        {
            var answerWithOption = new AnswerOption
            {
                Text = text,
                IsValid = isValid,
                QuestionId = QuestionId
            };

            return answerWithOption;
        }

        public static UserAnswerOption CreateDomainUserAnswerWithOption(int userId, int answerOptionId, bool isValid)
        {
            var userAnswerWithOption = new UserAnswerOption
            {
                UserId = userId,
                AnswerOptionId = answerOptionId,
                isValid = isValid
            };

            return userAnswerWithOption;
        }

        public static UserTest CreateDomainUserTest(int userId, int testId, string status)
        {
            var userTest = new UserTest
            {
                UserId = userId,
                TestId = testId,
                Status = status
            };

            return userTest;
        }
    }
}
