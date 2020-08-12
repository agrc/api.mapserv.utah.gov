using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Executors;
using WebAPI.Domain;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class StandardizeRouteNameCommandTests
    {
        [TestCase("15", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("I15", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("I 15", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("I-15", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("0015", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("I - 15", SideDelineation.P, ExpectedResult = "0015PM")]
        [TestCase("I", SideDelineation.P, ExpectedResult = null)]
        [TestCase("I - 5", SideDelineation.P, ExpectedResult = "0005PM")]
        [TestCase("I - 515", SideDelineation.P, ExpectedResult = "0515PM")]
        [TestCase("I - 1515", SideDelineation.P, ExpectedResult = "1515PM")]
        [TestCase("I - 15 14", SideDelineation.P, ExpectedResult = null)]
        public string CleansInterstate(string route, SideDelineation side)
        {
            var command = new StandardizeRouteNameCommand(route, side);
            return CommandExecutor.ExecuteCommand(command);
        }
    }
}