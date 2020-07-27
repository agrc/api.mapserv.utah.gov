using System.Collections.Generic;
using System.Linq;
using AGRC.api.Models.ArcGis;
using Autofac;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Geocoding {
    public interface IFilterSuggestionFactory {
        IFilterSuggestionStrategy GetStrategy(int acceptScore);
    }

    public class FilterSuggestionFactory : IFilterSuggestionFactory {
        private readonly IHttpContextAccessor httpContext;
        public FilterSuggestionFactory(IHttpContextAccessor httpContext) {
            this.httpContext = httpContext;
        }
        public IFilterSuggestionStrategy GetStrategy(int acceptScore) {
            var version = httpContext.HttpContext.Request.RouteValues["version"].ToString();

            if (version == "1") {
                return new FilterStrategyV1();
            }

            return new FilterStrategyV2(acceptScore);
        }
    }

    public interface IFilterSuggestionStrategy {
        IReadOnlyCollection<Candidate> Filter(IReadOnlyCollection<Candidate> candidates);
    }

    public class FilterStrategyV1 : IFilterSuggestionStrategy {
        public IReadOnlyCollection<Candidate> Filter(IReadOnlyCollection<Candidate> candidates)
            => candidates;
    }

    public class FilterStrategyV2 : IFilterSuggestionStrategy {
        private readonly int acceptScore;

        public FilterStrategyV2(int acceptScore) {
            this.acceptScore = acceptScore;
        }

        public IReadOnlyCollection<Candidate> Filter(IReadOnlyCollection<Candidate> candidates)
            => candidates.Where(x => x.Score > acceptScore).ToList();
    }
}
