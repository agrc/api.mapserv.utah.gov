using AutoMapper;
using WebAPI.Common.Executors;
using WebAPI.Dashboard.Commands.Time;

namespace WebAPI.Dashboard.Resolvers
{
    /// <summary>
    ///     https://github.com/AutoMapper/AutoMapper/wiki/Custom-value-resolvers
    /// </summary>
    public class CalculateTimeAgoResolver : ValueResolver<long, string>
    {
        protected override string ResolveCore(long source)
        {
            return CommandExecutor.ExecuteCommand(new CalculateTimeAgoCommand(source));
        }
    }
}