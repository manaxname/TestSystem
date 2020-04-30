using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class TopicNotFoundException : Exception
    {
        public TopicNotFoundException()
        {
        }

        public TopicNotFoundException(string message)
            : base(message)
        {
        }

        public TopicNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
