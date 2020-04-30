using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class TopicAlreadyExsitsException : Exception
    {
        public TopicAlreadyExsitsException()
        {
        }

        public TopicAlreadyExsitsException(string message)
            : base(message)
        {
        }

        public TopicAlreadyExsitsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
