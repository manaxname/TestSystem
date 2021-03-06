﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSystem.Domain.Models;

namespace TestSystem.Web.Models
{
    public class UserAnswerModel
    {
        public bool isValid { get; set; }
        public string Text { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}
