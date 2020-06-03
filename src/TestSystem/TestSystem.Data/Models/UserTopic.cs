using System;
using System.Collections.Generic;
using System.Text;
using TestSystem.Common;

namespace TestSystem.Data.Models
{
    public class UserTopic
    {
        public TopicStatus Status { get; set; }
        public int Points { get; set; }
        public bool IsDeleted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
        public bool IsTopicAsigned { get; set; }
    }
}
