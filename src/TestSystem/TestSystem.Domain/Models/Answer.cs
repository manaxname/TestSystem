﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Domain.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsValid { get; set; }

        public int QuestionId { get; set; }
    }
}
