using System;
using System.Collections.Generic;
using System.Text;

namespace TestSystem.Common.CustomExceptions
{
    public class UserTestNotFoundException : Exception
    {
        public UserTestNotFoundException()
        {
        }

        public UserTestNotFoundException(string message)
            : base(message)
        {
        }

        public UserTestNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
