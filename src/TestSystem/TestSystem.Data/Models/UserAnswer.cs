﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Data.Models
{
    public class UserAnswer
    {
        public bool isValid { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}
