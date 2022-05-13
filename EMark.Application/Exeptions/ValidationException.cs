using System;

namespace EMark.Application.Exeptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
          
        }
    }
}
