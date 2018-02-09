using System;

namespace WebAPI.Common.Exceptions
{
    public class GeocodingException : Exception
    {
        public GeocodingException()
        {
            
        }

        public GeocodingException(string message, Exception innerException)
            : base(message, innerException) {}

        public GeocodingException(string message)
            : base(message) {}
    }
}