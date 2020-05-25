using System;
using System.Collections.Generic;
using TestSystem.Common;
using TestSystem.Domain.Models;

namespace TestSystem.Web.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public int PassingPoints { get; set; }

        public ICollection<Test> Tests { get; set; }
        public ICollection<UserTopic> UserTopics { get; set; }

        // userTopic
        public TopicStatus Status { get; set; }
        public int Points { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
