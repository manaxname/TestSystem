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

        public static Test CreateDomainTest(string name)
        {
            var test = new Test
            {
                Name = name,
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
        public static AnswerText CreateDomainAnswerWithText(string text, int QuestionId)
        {
            var answerWithText = new AnswerText
            {
                Text = text,
                QuestionId = QuestionId
            };

            return answerWithText;
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
    }
}
