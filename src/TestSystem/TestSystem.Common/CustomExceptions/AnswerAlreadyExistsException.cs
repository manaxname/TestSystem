using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class AnswerAlreadyExistsException : Exception
    {
        public AnswerAlreadyExistsException()
        {
        }

        public AnswerAlreadyExistsException(string message)
            : base(message)
        {
        }

        public AnswerAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
