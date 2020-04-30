using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Data.Models
{
    public class UserTopic
    {
        public string Status { get; set; }
        public int Points { get; set; }
        public bool IsDeleted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
