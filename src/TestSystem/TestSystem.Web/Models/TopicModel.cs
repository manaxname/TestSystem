using System;
using System.Collections.Generic;
using TestSystem.Domain.Models;

namespace TestSystem.Web.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int PassingPoints { get; set; }

        public ICollection<Test> Tests { get; set; }
        public ICollection<UserTopic> UserTopics { get; set; }
    }
}
