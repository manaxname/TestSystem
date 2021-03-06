﻿using System;
using System.Collections.Generic;
using TestSystem.Common;

namespace TestSystem.Domain.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public TopicType TopicType { get; set; }

        public int PassingPoints { get; set; }

        public ICollection<Test> Tests { get; set; }
        public ICollection<UserTopic> UserTopics { get; set; }
    }
}
