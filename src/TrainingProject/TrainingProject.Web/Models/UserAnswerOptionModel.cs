using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingProject.Web.Models
{
    public class UserAnswerOptionModel
    {
        public int Id { get; set; }
        public bool isValid { get; set; }
        public string Text { get; set; }

        public int UserId { get; set; }
        public int AnswerOptionId { get; set; }
    }
}
