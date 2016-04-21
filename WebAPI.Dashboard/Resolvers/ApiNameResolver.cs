using System;
using System.Linq;
using AutoMapper;
using WebAPI.Common.Extensions;
using WebAPI.Common.Models.Enums;

namespace WebAPI.Dashboard.Resolvers
{
    public class ApiNameResolver : ValueResolver<string, string>
    {
        protected override string ResolveCore(string source)
        {
            return Enum.Parse(typeof (ApiNames), source.Split(',')
                                                       .First()
                                                       .Split('.')
                                                       .Last()
                                                       .Replace("Usage", ""))
                       .ToString()
                       .DeCamel();
        }
    }
}