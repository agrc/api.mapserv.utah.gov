using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Infrastructure;

namespace AGRC.api.Features.Searching {
    public class ValidateSql {
        public class Computation : IComputation<bool> {
            public Computation(string sql) {
                Sql = sql;
            }

            internal string Sql { get; }
        }

        public class Handler : IComputationHandler<Computation, bool> {
            public Task<bool> Handle(Computation request, CancellationToken cancellationToken) {
                var badChars = new Regex(@";|--|/\*|\*/", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                return Task.FromResult(badChars.IsMatch(request.Sql));
            }
        }
    }
}
