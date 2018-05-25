using Microsoft.AspNetCore.Builder;

namespace api.mapserv.utah.gov.Middleware
{
    public static class MiddlewareExtensions
    {
       public static IApplicationBuilder UseApiKeyAuthorization(this IApplicationBuilder builder)
       {
          return builder.UseMiddleware<AuthorizeApiKeyFromRequest>();
       }
    }
}
