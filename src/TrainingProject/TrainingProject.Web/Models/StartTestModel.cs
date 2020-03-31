using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingProject.Web.Models
{
    public class StartTestModel
    {
        public int QuestionNumber { get; private set; }
        public int TotalQuestions { get; private set; }

        public StartTestModel(int questionNumber, int totalQuestions)
        {
            QuestionNumber = questionNumber;
            TotalQuestions = totalQuestions;
        }

        public bool HasPreviousQuestion
        {
            get
            {
                return (QuestionNumber > 1);
            }
        }

        public bool HasNextQuestion
        {
            get
            {
                return (QuestionNumber < TotalQuestions);
            }
        }
    }
}
