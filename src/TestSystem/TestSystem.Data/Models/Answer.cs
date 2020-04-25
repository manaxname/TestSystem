using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Data.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsValid { get; set; }
        
        public int QuestionId { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
