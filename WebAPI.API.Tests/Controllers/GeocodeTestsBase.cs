using NUnit.Framework;

namespace WebAPI.API.Tests.Controllers
{
    public class GeocodeTestsBase
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            CacheConfig.BuildCache();
        }
    }
}