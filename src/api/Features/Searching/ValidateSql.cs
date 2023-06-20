using AGRC.api.Infrastructure;

namespace AGRC.api.Features.Searching;
public partial class ValidateSql {
    public class Computation(string sql) : IComputation<bool> {
        internal readonly string _sql = sql;
    }

    public partial class Handler : IComputationHandler<Computation, bool> {
        public Task<bool> Handle(Computation request, CancellationToken cancellationToken) {
            var badChars = BadChars();

            return Task.FromResult(badChars.IsMatch(request._sql));
        }

        [GeneratedRegex(";|--|/\\*|\\*/", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
        private static partial Regex BadChars();
    }
}
