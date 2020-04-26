using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class AnswerNotFoundException : Exception
    {
        public AnswerNotFoundException()
        {
        }

        public AnswerNotFoundException(string message)
            : base(message)
        {
        }

        public AnswerNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
