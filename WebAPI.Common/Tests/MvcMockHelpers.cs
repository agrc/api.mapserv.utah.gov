using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace WebAPI.Common.Tests
{
    public static class MvcMockHelpers
    {
        public static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            return context.Object;
        }

        public static HttpContextBase FakeHttpContext(string url)
        {
            var context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);
            return context;
        }

        public static void SetFakeControllerContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }

        private static string GetUrlFileName(string url)
        {
            return url.Contains("?") ? url.Substring(0, url.IndexOf("?", StringComparison.Ordinal)) : url;
        }

        private static NameValueCollection GetQueryStringParameters(string url)
        {
            if (!url.Contains("?"))
            {
                return null;
            }

            var parameters = new NameValueCollection();

            var parts = url.Split("?".ToCharArray());
            var keys = parts[1].Split("&".ToCharArray());

            foreach (var key in keys)
            {
                var part = key.Split("=".ToCharArray());
                parameters.Add(part[0], part[1]);
            }

            return parameters;
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (!url.StartsWith("~/"))
            {
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");
            }

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));
            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
        }

        public static T Invoke<T>(Expression<Func<T>> exp) where T : ActionResult
        {
            var methodCall = (MethodCallExpression) exp.Body;
            var method = methodCall.Method;
            var memberExpression = (MemberExpression) methodCall.Object;

            var getCallerExpression = Expression.Lambda<Func<Object>>(memberExpression);
            var getCaller = getCallerExpression.Compile();
            var ctrlr = (Controller) getCaller();

            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(ctrlr.GetType());
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(method, method.Name, controllerDescriptor);

            // OnActionExecuting

            var rc = new RequestContext();
            ctrlr.ControllerContext = new ControllerContext(rc, ctrlr);
            var ctx1 = new ActionExecutingContext(ctrlr.ControllerContext, actionDescriptor,
                                                  new Dictionary<string, object>());
            var onActionExecuting = ctrlr.GetType()
                                         .GetMethod("OnActionExecuting", BindingFlags.Instance | BindingFlags.NonPublic);
            onActionExecuting.Invoke(ctrlr, new object[]
            {
                ctx1
            });

            // call controller method

            var result = exp.Compile()();

            // OnActionExecuted

            var ctx2 = new ActionExecutedContext(ctrlr.ControllerContext, actionDescriptor, false, null)
            {
                Result = result
            };
            var onActionExecuted = ctrlr.GetType()
                                        .GetMethod("OnActionExecuted", BindingFlags.Instance | BindingFlags.NonPublic);
            onActionExecuted.Invoke(ctrlr, new object[]
            {
                ctx2
            });

            return (T) ctx2.Result;
        }
    }

    public class TestHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage,
            CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public TestHandler()
        {
            _handlerFunc = (r, c) => Return200();
        }

        public TestHandler(Func<HttpRequestMessage,
            CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }

        public static Task<HttpResponseMessage> Return200()
        {
            return Task.Factory.StartNew(
                () => new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}