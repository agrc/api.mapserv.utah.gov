using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ParseZoneCommandTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CacheConfig.BuildCache();
        }

        [TestCase("845200703")]
        [TestCase("84520-0703")]
        public void ZipPlusFour(string zone)
        {
            var addressBase = CommandExecutor.ExecuteCommand(new ParseZoneCommand(zone, new GeocodeAddress(new CleansedAddress())));

            Assert.That(addressBase.Zip5, Is.EqualTo(84520));
            Assert.That(addressBase.Zip4, Is.EqualTo(0703));
        }

        [TestCase("City of Alta")]
        [TestCase("Town of Alta")]
        [TestCase("Alta")]
        public void CityOfTownOfZones(string zone)
        {
            var addressBase = CommandExecutor.ExecuteCommand(new ParseZoneCommand(zone, new GeocodeAddress(new CleansedAddress())));

            Assert.That(addressBase.AddressGrids.Count, Is.EqualTo(1));
            Assert.That(addressBase.AddressGrids.First().Grid, Is.EqualTo("SALT LAKE CITY"));
        }
    }
}