using AGRC.api.Models.ArcGis;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace AGRC.api.Features.Geocoding;
public interface IFilterSuggestionFactory {
    IFilterSuggestionStrategy GetStrategy(int acceptScore);
}

public class FilterSuggestionFactory : IFilterSuggestionFactory {
    private readonly HttpContext _httpContext;
    public FilterSuggestionFactory(IHttpContextAccessor httpContext) {
        _httpContext = httpContext.HttpContext!;
    }
    public IFilterSuggestionStrategy GetStrategy(int acceptScore) {
        var version = _httpContext.GetRequestedApiVersion();

        return version?.MajorVersion switch {
            1 => new FilterStrategyV1(),
            2 => new FilterStrategyV2(acceptScore),
            _ => new FilterStrategyV1()
        };
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
