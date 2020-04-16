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

        public static Answer CreateDomainAnswerWith(string text, bool isValid, int QuestionId)
        {
            var answerWith = new Answer
            {
                Text = text,
                IsValid = isValid,
                QuestionId = QuestionId
            };

            return answerWith;
        }

        public static UserAnswer CreateDomainUserAnswerWith(int userId, int answerId, bool isValid, string text)
        {
            var userAnswerWith = new UserAnswer
            {
                UserId = userId,
                AnswerId = answerId,
                isValid = isValid,
                Text = text
            };

            return userAnswerWith;
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
