using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;

namespace WebAPI.API.Tests.Commands
{
    public class ParseSingleLineInputCommandTests
    {

        [TestFixture]
        public class ZipCodes
        {
            private ParseSingleLineInputCommand _command;

            [TestFixtureSetUp]
            public void SetupGlobal()
            {
                CacheConfig.BuildCache();
            }

            [SetUp]
            public void Setup()
            {
                _command = new ParseSingleLineInputCommand();
            }

            [TestCase("123 house street 845200703")]
            [TestCase("123 house street 84520-0703")]
            [TestCase("123 house street 845200703 Utah")]
            [TestCase("123 house street 84520-0703 UT")]
            public void ZipCode(string input)
            {
                _command.SetInput(input);
                CommandExecutor.ExecuteCommand(_command);

                Assert.That(_command.Result.Zone, Is.EqualTo("84520"));
                Assert.That(_command.Result.Street, Is.EqualTo("123 house street"));
            }

            [TestCase("123 East Logan Ave Salt Lake City")]
            [TestCase("123 East Logan Ave, Salt Lake City")]
            [TestCase("123 East Logan Ave Salt Lake City Utah")]
            [TestCase("123 East Logan Ave, Salt Lake City Utah")]
            [TestCase("123 East Logan Ave Salt Lake City Ut")]
            [TestCase("123 East Logan Ave Salt Lake City Ut.")]
            public void PlaceName(string input)
            {
                _command.SetInput(input);
                CommandExecutor.ExecuteCommand(_command);

                Assert.That(_command.Result.Zone, Is.EqualTo("salt lake city"));
                Assert.That(_command.Result.Street, Is.EqualTo("123 East Logan Ave"));
            }

            [TestCase("123 East Logan Ave Salt Lake City 84111")]
            [TestCase("123 East Logan Ave, Salt Lake City 84111-1234")]
            [TestCase("123 East Logan Ave Salt Lake City Utah, 84111")]
            [TestCase("123 East Logan Ave, Salt Lake City Utah 84111")]
            [TestCase("123 East Logan Ave Salt Lake City Ut 84111")]
            [TestCase("123 East Logan Ave Salt Lake City Ut. 84111")]
            public void PlaceNameAndZipCode(string input)
            {
                _command.SetInput(input);
                CommandExecutor.ExecuteCommand(_command);

                Assert.That(_command.Result.Zone, Is.EqualTo("salt lake city"));
                Assert.That(_command.Result.Street, Is.EqualTo("123 East Logan Ave"));
            }
        }
    }
}