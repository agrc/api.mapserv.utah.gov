using AGRC.api.Infrastructure;

namespace AGRC.api.Features.Searching;
public partial class ValidateSql {
    public class Computation : IComputation<bool> {
        public Computation(string sql) {
            Sql = sql;
        }

        internal readonly string Sql;
    }

    public partial class Handler : IComputationHandler<Computation, bool> {
        public Task<bool> Handle(Computation request, CancellationToken cancellationToken) {
            var badChars = BadChars();

            return Task.FromResult(badChars.IsMatch(request.Sql));
        }

        [GeneratedRegex(";|--|/\\*|\\*/", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
        private static partial Regex BadChars();
    }
}
