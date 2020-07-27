using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;

namespace api.mapserv.utah.gov.Features.Searching {
    public class KeyFormatting {
        public class Pipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : SqlQuery.Command
            where TResponse : IReadOnlyCollection<SearchResponseContract> {

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
                var response = await next();

                if (!response.Any()) {
                    return response;
                }

                Func<string, string> formatterFunction = x => x;
                switch (request.Styling) {
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
