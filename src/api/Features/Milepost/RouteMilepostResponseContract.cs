using AGRC.api.Models;

namespace AGRC.api.Features.Milepost;
public record RouteMilepostResponseContract(string Source, Point Location, string MatchRoute);
