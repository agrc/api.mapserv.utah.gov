using AutoMapper;

namespace WebAPI.Dashboard.Resolvers
{
    public class UsageFormatterResolver : ValueResolver<string, string>
    {
        protected override string ResolveCore(string value)
        {
            return string.Format(value, "##,#0");
        }
    }
}