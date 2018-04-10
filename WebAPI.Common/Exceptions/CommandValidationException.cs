namespace WebAPI.Common.Exceptions
{
    using System;
    public class CommandValidationException : Exception
    {
        public CommandValidationException()
        {
        }

        public CommandValidationException(string message)
            : base(message) { }
    }
}
