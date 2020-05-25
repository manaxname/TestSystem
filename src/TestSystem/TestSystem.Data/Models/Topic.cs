using System;
using System.Collections.Generic;

namespace TestSystem.Data.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public int PassingPoints { get; set; }

        public ICollection<Test> Tests { get; set; }
        public ICollection<UserTopic> UserTopics { get; set; }
    }
}
