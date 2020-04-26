using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class TestAlreadyExsitsException : Exception
    {
        public TestAlreadyExsitsException()
        {
        }

        public TestAlreadyExsitsException(string message)
            : base(message)
        {
        }

        public TestAlreadyExsitsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
