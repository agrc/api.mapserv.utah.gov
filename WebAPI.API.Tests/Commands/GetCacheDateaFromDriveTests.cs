using NUnit.Framework;
using WebAPI.API.Commands.Drive;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetCacheDateaFromDriveTests
    {
        [Test]
        public void CanAuthenticate()
        {
            var command = new GetCachedDataFromDriveCommand();
            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result.ZipCodesGrids, Is.Not.Empty);
            Assert.That(command.Result.PlaceGrids, Is.Not.Empty);
            Assert.That(command.Result.UspsDeliveryPoints, Is.Not.Empty);
            Assert.That(command.Result.PoBoxExclusions, Is.Not.Empty);
        }
    }
}
