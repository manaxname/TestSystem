﻿using System;
using System.Collections.Generic;
using System.Text;
using TestSystem.Common;

namespace TestSystem.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Stage { get; set; }
        public int Points { get; set; }
        public QuestionTypes QuestionType { get; set; }
        public string ImageFullName { get; set; }
        public bool IsDeleted { get; set; }

        public int TestId { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
