﻿using System;
using System.Collections.Generic;
using System.Linq;
using TestSystem.Common;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic
{
    public static class Helper
    {
        public static User CreateDomainUser(string email, string passwordHash, UserRoles role)
        {
            return new User
            {
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                ConfirmationToken = Guid.NewGuid(),
                IsConfirmed = false
            };
        }

        public static Test CreateDomainTest(int topicId, string name, int time)
        {
            return new Test
            {
                TopicId = topicId,
                Name = name,
                Minutes = time
            };
        }

        public static Topic CreateDomainTopic(string name, int passingPoints, TopicType topicType, bool islocked = true)
        {
            return new Topic
            {
                Name = name,
                PassingPoints = passingPoints,
                TopicType = topicType,
                IsLocked = islocked,
            };
        }

        public static Question CreateDomainQuestion(string text, int stage, int points, QuestionTypes questionType, int testId)
        {
            return new Question
            {
                Text = text,
                Stage = stage,
                Points = points,
                QuestionType = questionType,
                TestId = testId,
            };
        }

        public static Answer CreateDomainAnswer(string text, bool isValid, int QuestionId)
        {
            return new Answer
            {
                Text = text,
                IsValid = isValid,
                QuestionId = QuestionId
            };
        }

        public static UserAnswer CreateDomainUserAnswer(int userId, int answerId, bool isValid, string text)
        {
            return new UserAnswer
            {
                UserId = userId,
                AnswerId = answerId,
                isValid = isValid,
                Text = text
            };
        }

        public static UserTest CreateDomainUserTest(int userId, int testId, TestStatus status)
        {
            return new UserTest
            {
                UserId = userId,
                TestId = testId,
                Status = status
            };
        }
    }
}
