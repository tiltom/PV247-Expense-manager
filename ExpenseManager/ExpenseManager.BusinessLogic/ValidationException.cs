using System;
using System.Collections.Generic;

namespace ExpenseManager.BusinessLogic
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string> Erorrs = new Dictionary<string, string>();

        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}