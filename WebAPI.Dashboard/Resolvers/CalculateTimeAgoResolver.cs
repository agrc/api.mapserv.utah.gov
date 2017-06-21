using System;
using AutoMapper;
using WebAPI.Common.Executors;
using WebAPI.Dashboard.Commands.Time;

namespace WebAPI.Dashboard.Resolvers
{
    /// <summary>
    ///     https://github.com/AutoMapper/AutoMapper/wiki/Custom-value-resolvers
    /// </summary>
    public class CalculateTimeAgoResolver : IMemberValueResolver<object, object, long, string>
    {
        public string Resolve(object source, object destination, long srcMember, string destMember, ResolutionContext context)
        {
            return CommandExecutor.ExecuteCommand(new CalculateTimeAgoCommand(srcMember));
        }
    }
}