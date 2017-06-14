using NUnit.Framework;
using WebAPI.API.Commands.Geocode.Flags;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Tests.Commands
{
    public class ExceptionsCommandTests
    {
        [TestFixture]
        public class DoubleAvenuesTests
        {
            [TestCase("7th", 84047, Direction.West)]
            [TestCase("7 th", 84047, Direction.West)]
            [TestCase("seventh", 84047, Direction.West)]
            [TestCase("7", 84047, Direction.West)]
            [TestCase("seventh heaven", 84047, Direction.None)]
            [TestCase("7", 11111, Direction.None)]
            [TestCase("7th", 11111, Direction.None)]
            [TestCase("7th", null, Direction.None)]
            public void MidvaleAvenuesAddsWestIfNotSupplied(string streetname, int zipcode, Direction direction)
            {
                var address = new GeocodeAddress(new CleansedAddress("", 0, 0, 0, Direction.None, streetname,
                    StreetType.Avenue, Direction.None, 0, zipcode, false, false));

                var command = new DoubleAvenuesExceptionCommand(address, "");
                command.Run();

                Assert.That(command.GetResult().PrefixDirection, Is.EqualTo(direction));
            }

            [TestCase("7th", "MIDVale", Direction.West)]
            [TestCase("7 th", "midvale", Direction.West)]
            [TestCase("seventh", " midvale ", Direction.West)]
            [TestCase("7", "  midvale", Direction.West)]
            [TestCase("7", "not midvale", Direction.None)]
            [TestCase("7th", "not midvale", Direction.None)]
            [TestCase("7th", null, Direction.None)]
            public void MidvaleAvenuesAddsWestIfNotSuppliedForCity(string streetname, string city, Direction direction)
            {
                var address = new GeocodeAddress(new CleansedAddress("", 0, 0, 0, Direction.None, streetname,
                    StreetType.Avenue, Direction.None, 0, null, false, false));

                var command = new DoubleAvenuesExceptionCommand(address, city);
                command.Run();

                Assert.That(command.GetResult().PrefixDirection, Is.EqualTo(direction));
            }
        }
    }
}