using System;
using System.Collections.Generic;
using System.Text;
using TrainingProject.Common;

namespace TrainingProject.Domain.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Stage { get; set; }
        public int Points { get; set; }
        public string QuestionType { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public int TestId { get; set; }
    }
}
