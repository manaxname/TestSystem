using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class QuestionNotFoundException : Exception
    {
        public QuestionNotFoundException()
        {
        }

        public QuestionNotFoundException(string message)
            : base(message)
        {
        }

        public QuestionNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
