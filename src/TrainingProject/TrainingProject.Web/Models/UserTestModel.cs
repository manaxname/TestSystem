using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingProject.Web.Models
{
    public class UserTestModel
    {
        public string TestName { get; set; }
        public string Status { get; set; }
        public int Points { get; set; }
        public DateTime StartTime { get; set; }
        public int TestMinutes { get; set; }

        public int UserId { get; set; }
        public int TestId { get; set; }
    }
}
