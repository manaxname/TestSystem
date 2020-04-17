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
        public string UserQuestionIds { get; set; }

        public int CurrQuestionId { get; set; }
        public int CurrQuestionStage { get; set; }
        public string CurrQuestionText { get; set; }
        public string CurrQuestionType { get; set; }
        public string CurrQuestionImageLocation { get; set; }
        public IEnumerable<UserAnswerModel> CurrQuestionUserAnswers { get; set; }
    }
}
