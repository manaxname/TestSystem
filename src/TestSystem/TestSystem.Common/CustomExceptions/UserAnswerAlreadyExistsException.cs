using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class UserAnswerAlreadyExistsException : Exception
    {
        public UserAnswerAlreadyExistsException()
        {
        }

        public UserAnswerAlreadyExistsException(string message)
            : base(message)
        {
        }

        public UserAnswerAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
