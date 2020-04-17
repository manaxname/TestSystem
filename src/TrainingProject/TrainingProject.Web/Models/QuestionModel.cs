using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingProject.Web.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Stage { get; set; }
        public int Points { get; set; }
        public string QuestionType { get; set; }
        public string ImageFullName { get; set; }
        public string ImageLocation { get; set; }
        public IFormFile Image { get; set; }

        public int TestId { get; set; }
    }
}
