using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.Data.Models
{
    public class UserAnswerOption
    {
        public bool isValid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int AnswerOptionId { get; set; }
        public AnswerOption AnswerOption { get; set; }
    }
}
