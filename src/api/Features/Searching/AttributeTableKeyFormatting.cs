using ugrc.api.Infrastructure;
using ugrc.api.Models.Constants;

namespace ugrc.api.Features.Searching;
public static class AttributeTableKeyFormatting {
    public class Decorator(IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?> decorated) : IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?> _decorated = decorated;

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(SqlQuery.Computation computation, CancellationToken cancellationToken) {
            var response = await _decorated.Handle(computation, cancellationToken);

            if (response is null) {
                return null;
            }

            if (!response.Any()) {
                return response;
            }

            Func<string, string> formatterFunction = x => x;
            switch (computation.SearchOptions.AttributeStyle) {
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
                if (item is null) {
                    continue;
                }

                item.Attributes = item.Attributes.ToDictionary(x => formatterFunction(x.Key), y => y.Value);
            }

            return response;
        }
    }
}
