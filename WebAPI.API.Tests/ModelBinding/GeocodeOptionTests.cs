using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.ModelBinding;
using Moq;
using NUnit.Framework;
using WebAPI.API.ModelBindings;
using WebAPI.Domain;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Tests.ModelBinding
{
    [TestFixture]
    public class GeocodeOptionTests
    {
        [Test]
        public void DefaultOptions()
        {
            var binder = new GeocodeOptionsModelBinding();
            var httpControllerContext = new HttpControllerContext
                {
                    Request =
                        new HttpRequestMessage(HttpMethod.Get,
                                               "http://webapi/api/v1/geocode/address/zone")
                };
            var httpActionContext = new HttpActionContext {ControllerContext = httpControllerContext};
            var moc = new Mock<ModelMetadataProvider>();
            var modelBindingContext = new ModelBindingContext
                {
                    ModelMetadata =
                        new ModelMetadata(moc.Object, null, null, typeof (GeocodeOptions), null)
                };

            var successful = binder.BindModel(httpActionContext, modelBindingContext);

            Assert.That(successful, Is.True);

            var model = modelBindingContext.Model as GeocodeOptions;

            Assert.That(model, Is.Not.Null);

            Assert.That(model.AcceptScore, Is.EqualTo(70));
            Assert.That(model.WkId, Is.EqualTo(26912));
            Assert.That(model.SuggestCount, Is.EqualTo(0));
            Assert.That(model.PoBox, Is.False);
            Assert.That(model.Locators, Is.EqualTo(LocatorType.All));
        }

        [Test]
        public void AllOptions()
        {
            var binder = new GeocodeOptionsModelBinding();
            var httpControllerContext = new HttpControllerContext
                {
                    Request =
                        new HttpRequestMessage(HttpMethod.Get,
                                               "http://webapi/api/v1/geocode/address/zone?spatialReference=111&format=geojson&callback=p&acceptScore=80&suggest=1&locators=roadCenterlines&pobox=tRue&apiKey=AGRC-ApiExplorer")
                };
            var httpActionContext = new HttpActionContext {ControllerContext = httpControllerContext};
            var moc = new Mock<ModelMetadataProvider>();
            var modelBindingContext = new ModelBindingContext
                {
                    ModelMetadata =
                        new ModelMetadata(moc.Object, null, null, typeof (GeocodeOptions), null)
                };

            var successful = binder.BindModel(httpActionContext, modelBindingContext);

            Assert.That(successful, Is.True);

            var model = modelBindingContext.Model as GeocodeOptions;
            
            Assert.That(model, Is.Not.Null);

            Assert.That(model.AcceptScore, Is.EqualTo(80));
            Assert.That(model.WkId, Is.EqualTo(111));
            Assert.That(model.SuggestCount, Is.EqualTo(1));
            Assert.That(model.PoBox, Is.True);
            Assert.That(model.Locators, Is.EqualTo(LocatorType.RoadCenterlines));
        }
    }
}