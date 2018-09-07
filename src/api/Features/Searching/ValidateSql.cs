using System.Text.RegularExpressions;
using MediatR;

namespace api.mapserv.utah.gov.Features.Searching {
    public class ValidateSql {
        public class Command : IRequest<bool> {
            public Command(string sql) {
                Sql = sql;
            }

            internal string Sql { get; }
        }

        public class Handler : RequestHandler<Command, bool> {
            protected override bool Handle(Command request) {
                var badChars = new Regex(@";|--|/\*|\*/", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                return badChars.IsMatch(request.Sql);
            }
        }
    }
}
