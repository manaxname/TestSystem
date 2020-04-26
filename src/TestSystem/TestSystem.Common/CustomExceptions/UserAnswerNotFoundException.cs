using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class UserAnswerNotFoundException : Exception
    {
        public UserAnswerNotFoundException()
        {
        }

        public UserAnswerNotFoundException(string message)
            : base(message)
        {
        }

        public UserAnswerNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
