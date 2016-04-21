using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Executors;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class StandardizeRouteNameCommandTests
    {
        [TestCase("15", Result = "0015")]
        [TestCase("I15", Result = "0015")]
        [TestCase("I 15", Result = "0015")]
        [TestCase("I-15", Result = "0015")]
        [TestCase("0015", Result = "0015")]
        [TestCase("I - 15", Result = "0015")]
        [TestCase("I", Result = null)]
        [TestCase("I - 5", Result = "0005")]
        [TestCase("I - 515", Result = "0515")]
        [TestCase("I - 1515", Result = "1515")]
        [TestCase("I - 15 14", Result = null)]
        public string CleansInterstate(string route)
        {
            var command = new StandardizeRouteNameCommand(route);
            return CommandExecutor.ExecuteCommand(command);
        }
    }
}