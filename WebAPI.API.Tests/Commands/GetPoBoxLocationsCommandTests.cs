using NUnit.Framework;
using WebAPI.API.Commands.Sgid;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetPoBoxLocationsCommandTests
    {
        [Test]
        public void CanOpenFc()
        {
            var command = new GetPoBoxLocationsCommand();
            command.Run();

            Assert.That(command.Result, Is.Not.Empty);
        }
    }
}