using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSystem.Common;

namespace TestSystem.Web.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public bool IsValid { get; set; }

        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionText { get; set; }
        public QuestionTypes QuestionType { get; set; }
    }
}
