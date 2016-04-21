using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetAddressSystemFromZipCodeCommandTests
    {
        [SetUp]
        public void Setup()
        {
            CacheConfig.BuildCache();
        }

        [Test]
        public void GetsGridFromZipCode()
        {
            var command = new GetAddressSystemFromZipCodeCommand(84111);
            
            command.Run();

            var result = command.Result;

            Assert.That(result.Count, Is.EqualTo(1));

            var first = result.FirstOrDefault();

            Assert.That(first.Grid, Is.EqualTo("SALT LAKE CITY"));
            Assert.That(first.Weight, Is.EqualTo(0));
        }

        [Test]
        public void GetGridForZipThatDoesNotExist()
        {
            var command = new GetAddressSystemFromZipCodeCommand(90210);

            command.Run();

            var result = command.Result;

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetsMultipleItemsForZipCode()
        {
            var command = new GetAddressSystemFromZipCodeCommand(84032);
            
            command.Run();

            var result = command.Result;

            Assert.That(result.Count, Is.EqualTo(3));

            Assert.That(result.OrderByDescending(x=>x.Weight).First().Grid, Is.EqualTo("HEBER CITY"));
        }
    }
}