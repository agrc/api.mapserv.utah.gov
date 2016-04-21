using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ParseZoneCommandTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            CacheConfig.BuildCache();
        }

        [Test]
        public void ZipPlusFour()
        {
            var zone = "845200703";
            var addressBase =
                    CommandExecutor.ExecuteCommand(new ParseZoneCommand(zone, new GeocodeAddress(new CleansedAddress())));

            Assert.That(addressBase.Zip5, Is.EqualTo(84520));
            Assert.That(addressBase.Zip4, Is.EqualTo(0703));

            zone = "84520-0703";
            addressBase =
                    CommandExecutor.ExecuteCommand(new ParseZoneCommand(zone, new GeocodeAddress(new CleansedAddress())));

            Assert.That(addressBase.Zip5, Is.EqualTo(84520));
            Assert.That(addressBase.Zip4, Is.EqualTo(0703));
        }
    }
}