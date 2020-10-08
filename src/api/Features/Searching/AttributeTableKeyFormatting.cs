using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using MediatR;
using Serilog;

namespace AGRC.api.Features.Searching {
    public class AttributeTableKeyFormatting {
        public class Decorator : IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> {

            private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _decorated;
            private readonly ILogger _log;

            public Decorator(IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> decorated, ILogger log) {
                _log = log?.ForContext<AttributeTableKeyFormatting>();
                _decorated = decorated;
            }
            public async Task<IReadOnlyCollection<SearchResponseContract>> Handle(SqlQuery.Computation computation, CancellationToken cancellationToken) {
                var response = await _decorated.Handle(computation, cancellationToken);

                if (!response.Any()) {
                    return response;
                }

                Func<string, string> formatterFunction = x => x;
                switch (computation.Styling) {
                    case AttributeStyle.Lower: {
                            formatterFunction = x => x.ToLowerInvariant();
                            break;
                        }
                    case AttributeStyle.Upper: {
                            formatterFunction = x => x.ToUpperInvariant();
                            break;
                        }
                    case AttributeStyle.Input: {
                            return response;
                        }
                }

                foreach (var item in response) {
                    item.Attributes = item.Attributes.ToDictionary(x => formatterFunction(x.Key), y => y.Value);
                }

                return response;
            }
        }
    }
}
