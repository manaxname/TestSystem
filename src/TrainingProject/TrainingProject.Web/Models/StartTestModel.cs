using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingProject.Web.Models
{
    public class StartTestModel
    {
        public int TestId { get; set; }
        public int UserId { get; set; }
        public int StagesCount { get; set; }
        public DateTime StartTime { get; set; }
        public int TestMinutes { get; set; }
        public int SecondsLeft { get; set; }

        public string CurrQuestionText { get; set; }
        public int CurrQuestionStage { get; set; }

        public IEnumerable<UserAnswerOptionModel> CurrQuestionUserAnswersOptions { get; set; }
        public string QuestionIds { get; set; }
        public int CurrQuestionId { get; set; }
    }
}
