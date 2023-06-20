using AGRC.api.Models.ArcGis;
using Autofac;

namespace AGRC.api.Features.Geocoding;
public interface IFilterSuggestionFactory {
    IFilterSuggestionStrategy GetStrategy(int acceptScore);
}

public class FilterSuggestionFactory(IHttpContextAccessor accessor) : IFilterSuggestionFactory {
    private readonly IHttpContextAccessor _httpContextAccessor = accessor;

    public IFilterSuggestionStrategy GetStrategy(int acceptScore) {
        var version = _httpContextAccessor.HttpContext!.GetRequestedApiVersion();

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

public class FilterStrategyV2(int acceptScore) : IFilterSuggestionStrategy {
    private readonly int _acceptScore = acceptScore;

    public IReadOnlyCollection<Candidate> Filter(IReadOnlyCollection<Candidate> candidates)
        => candidates.Where(x => x.Score > _acceptScore).ToList();
}
