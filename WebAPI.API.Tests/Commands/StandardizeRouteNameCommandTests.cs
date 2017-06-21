using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Executors;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class StandardizeRouteNameCommandTests
    {
        [TestCase("15", ExpectedResult = "0015")]
        [TestCase("I15", ExpectedResult = "0015")]
        [TestCase("I 15", ExpectedResult = "0015")]
        [TestCase("I-15", ExpectedResult = "0015")]
        [TestCase("0015", ExpectedResult = "0015")]
        [TestCase("I - 15", ExpectedResult = "0015")]
        [TestCase("I", ExpectedResult = null)]
        [TestCase("I - 5", ExpectedResult = "0005")]
        [TestCase("I - 515", ExpectedResult = "0515")]
        [TestCase("I - 1515", ExpectedResult = "1515")]
        [TestCase("I - 15 14", ExpectedResult = null)]
        public string CleansInterstate(string route)
        {
            var command = new StandardizeRouteNameCommand(route);
            return CommandExecutor.ExecuteCommand(command);
        }
    }
}