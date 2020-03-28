﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.Data.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsValid { get; set; }

        public int QuestionId { get; set; }
        public ICollection<UserAnswerOption> UserAnswerOptions { get; set; }
    }
}
