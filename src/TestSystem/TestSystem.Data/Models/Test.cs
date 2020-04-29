using System;
using System.Collections.Generic;

namespace TestSystem.Data.Models
{
    public class Test
    {
        public int Id { get; set; }
        public int GroupNumber { get; set; }
        public string Name { get; set; }
        public int Minutes { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<UserTest> UserTests { get; set; }
    }
}
