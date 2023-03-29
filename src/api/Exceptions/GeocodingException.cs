namespace AGRC.api.Exceptions;
public class GeocodingException : Exception {
    public GeocodingException(string message, Exception innerException)
        : base(message, innerException) {
    }

    public GeocodingException(string message)
        : base(message) {
    }
}
