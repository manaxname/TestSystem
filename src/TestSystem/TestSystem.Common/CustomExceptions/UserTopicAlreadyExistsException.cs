using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class UserTopicAlreadyExistsException : Exception
    {
        public UserTopicAlreadyExistsException()
        {
        }

        public UserTopicAlreadyExistsException(string message)
            : base(message)
        {
        }

        public UserTopicAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
