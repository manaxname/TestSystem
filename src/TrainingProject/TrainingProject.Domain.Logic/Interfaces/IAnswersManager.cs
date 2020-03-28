using System;
using System.Collections.Generic;
using System.Text;
using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic.Interfaces
{
    public interface IAnswersManager
    {
        int CreateAnswerOption(AnswerOption answerWithOption);
        int CreateUserAnswerOption(UserAnswerOption userAnswerOption);

        void DeleteAnswerOption(int id);

        AnswerOption GetAnswerOptionById(int id);
        bool IsAnswerOptionExists(int id);
        IEnumerable<AnswerOption> GetAnswerOptionsByQuestionId(int questionId);
    }
}
