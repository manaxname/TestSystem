using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.Domain.Models
{
    public class UserAnswerOption
    {
        public int Id { get; set; }
        public bool isValid { get; set; }

        public int UserId { get; set; }
        public int AnswerOptionId { get; set; }
    }
}
