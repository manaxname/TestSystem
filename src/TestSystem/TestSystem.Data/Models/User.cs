﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestSystem.Common;

namespace TestSystem.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRoles Role { get; set; }
        public bool IsDeleted { get; set; }
        public Guid ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }
        public ICollection<UserTest> UserTests { get; set; }
        public ICollection<UserTopic> UserTopics { get; set; }
    }
}
