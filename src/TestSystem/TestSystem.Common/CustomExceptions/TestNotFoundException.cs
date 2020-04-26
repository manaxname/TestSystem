using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class TestNotFoundException : Exception
    {
        public TestNotFoundException()
        {
        }

        public TestNotFoundException(string message)
            : base(message)
        {
        }

        public TestNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
