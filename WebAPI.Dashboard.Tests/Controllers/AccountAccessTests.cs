using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using WebAPI.Common.Authentication.Forms;
using WebAPI.Common.Tests;
using WebAPI.Dashboard.Cache;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels.Users;

namespace WebAPI.Dashboard.Tests.Controllers
{
    [TestFixture]
    public class AccountAccessTests : RavenEmbeddableTest
    {
         public override void SetUp()
        {
            base.SetUp();

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.ApplicationPath)
                       .Returns("/");
            requestMock.SetupGet(x => x.Url)
                       .Returns(new Uri("http://localhost/a", UriKind.Absolute));
            requestMock.SetupGet(x => x.ServerVariables)
                       .Returns(new NameValueCollection());

            var responseMock = new Mock<HttpResponseBase>();
            responseMock.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>()))
                        .Returns((string url) => url);

            var principalMock = new Mock<IPrincipal>();
            principalMock.SetupGet(p => p.Identity)
                         .Returns(new GenericIdentity("testuser"));

            _httpContextMock = new Mock<HttpContextBase>();
            _httpContextMock.Setup(x => x.User)
                            .Returns(principalMock.Object);
            _httpContextMock.Setup(x => x.Request)
                            .Returns(requestMock.Object);
            _httpContextMock.SetupGet(x => x.Response)
                            .Returns(responseMock.Object);

            _routes = new RouteCollection();
            RouteConfig.RegisterRoutes(_routes);
        }

        private Mock<HttpContextBase> _httpContextMock;
        private RouteCollection _routes;

        [Test]
        public void PasswordForRegisterValidatesOnLogin()
        {
            var formsAuthMoq = new Mock<IFormsAuthentication>();
            var requestContext = new RequestContext(_httpContextMock.Object, new RouteData());
            var outputCacheMoq = new Mock<OutputCache>();

            App.OutputCache = outputCacheMoq.Object;

            var loginCredentials = new LoginCredentials
            {
                Email = "email@utah.gov",
                Password = "testpassword"
            };

            var controller = new AccountAccessController(DocumentStore)
            {
                FormsAuthentication = formsAuthMoq.Object,
                Url = new UrlHelper(requestContext, _routes)
            };

            controller.ControllerContext = new ControllerContext(requestContext, controller);

            MvcMockHelpers.Invoke(() => controller.Register(loginCredentials));
            MvcMockHelpers.Invoke(() => controller.Login(loginCredentials, string.Empty));

            Assert.That(controller.ErrorMessage, Is.Null, controller.ErrorMessage);
        }
    }
}