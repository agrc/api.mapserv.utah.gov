// #region License
// 
// // 
// // Copyright (C) 2013 AGRC
// // Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// // and associated documentation files (the "Software"), to deal in the Software without restriction, 
// // including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// // and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// // subject to the following conditions:
// // 
// // The above copyright notice and this permission notice shall be included in all copies or substantial 
// // portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// // NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// // IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// // SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// // 
// 
// #endregion

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.API.Handlers.Delegating
{
    public class CorsHandler : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return AllowCors(request) ?
                ProcessCorsRequest(request, ref cancellationToken) :
                base.SendAsync(request, cancellationToken);
        }

        private static bool AllowCors(HttpRequestMessage request)
        {
            return request.Headers.Contains(Origin);
        }

        private Task<HttpResponseMessage> ProcessCorsRequest(HttpRequestMessage request, ref CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                return Task.Factory.StartNew(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    AddCorsResponseHeaders(request, response);
                    return response;
                }, cancellationToken);
            }
            
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var resp = task.Result;
                resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                return resp;
            });
        }

        private static void AddCorsResponseHeaders(HttpRequestMessage request, HttpResponseMessage response)
        {
            response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

            var accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
            if (accessControlRequestMethod != null)
            {
                response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
            }

            var requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
            if (!string.IsNullOrEmpty(requestedHeaders))
            {
                response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
            }
        }
    }
}