using System;

namespace EMark.Application.Exeptions
{
    internal class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
          
        }
    }
}
