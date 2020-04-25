using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Data.Models
{
    public class UserTest
    {
        public string Status { get; set; }
        public int Points { get; set; }
        public DateTime StartTime { get; set; }
        public int TestMinutes { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}
