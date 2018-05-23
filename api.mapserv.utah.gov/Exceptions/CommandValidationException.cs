using System;

namespace api.mapserv.utah.gov.Exceptions
{
  public class CommandValidationException : Exception
  {
      public CommandValidationException()
      {
      }

      public CommandValidationException(string message)
          : base(message) { }
  }
}
