using System;

namespace WebAPI.API.Exceptions
{
    public class GeocodingException : Exception
    {
        public GeocodingException(string message, Exception innerException)
            : base(message, innerException) {}

        public GeocodingException(string message)
            : base(message) {}
    }
}