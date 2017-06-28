using NUnit.Framework;
using WebAPI.API.Commands.Sgid;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class CachePoBoxLocationsCommandTests
    {
        [Test]
        public void CanOpenFc()
        {
            var command = new CachePoBoxLocationsCommand();
            command.Run();

            Assert.That(command.Result, Is.Not.Empty);
        }
    }
}