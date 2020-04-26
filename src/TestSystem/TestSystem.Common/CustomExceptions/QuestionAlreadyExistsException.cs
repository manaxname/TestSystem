using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class QuestionAlreadyExistsException : Exception
    {
        public QuestionAlreadyExistsException()
        {
        }

        public QuestionAlreadyExistsException(string message)
            : base(message)
        {
        }

        public QuestionAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
