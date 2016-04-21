using System.Diagnostics;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;

namespace WebAPI.API.Tests.Speed
{
    [TestFixture]
    public class ParseAddressSpeedTests
    {
        private readonly string[] _strings = new[]
        {
            "326 e south temple", "160 w main st", "80 w 100 s", "64 S Main St", "1303 North Main Street",
            "500 Foothill Drive", "306 West River Bend Lane", "50 North Medical Drive", "1000 North Main Street",
            "3460 Pioneer Parkway", "630 East Medical Drive", "64 East 100 North Gunnison"
        };

        [TestFixtureSetUp]
        public void Setup()
        {
            CacheConfig.BuildCache();
        }

        [Test]
        public void New()
        {
            var timer = new Stopwatch();

            timer.Start();

            foreach (var address in _strings)
            {
                var addressBase = CommandExecutor.ExecuteCommand(new ParseAddressCommand(address));
            }

            timer.Stop();

            Debug.Print(timer.ElapsedMilliseconds.ToString());
        }
    }
}