using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class UserTopicNotFoundException : Exception
    {
        public UserTopicNotFoundException()
        {
        }

        public UserTopicNotFoundException(string message)
            : base(message)
        {
        }

        public UserTopicNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
