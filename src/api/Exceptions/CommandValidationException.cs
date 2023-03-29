namespace AGRC.api.Exceptions;
public class CommandValidationException : Exception {
    public CommandValidationException(string message)
        : base(message) {
    }
}
