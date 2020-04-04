using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingProject.Domain.Models;

namespace TrainingProject.Web.Models
{
    public class UserAnswerOptionModel
    {
        public bool isValid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int AnswerOptionId { get; set; }
        public AnswerOption AnswerOption { get; set; }
    }
}
