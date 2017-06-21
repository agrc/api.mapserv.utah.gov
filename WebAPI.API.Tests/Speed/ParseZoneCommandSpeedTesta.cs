using System.Diagnostics;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Tests.Speed
{
    [TestFixture]
    public class ParseZoneCommandSpeedTesta
    {
        private readonly string[] _strings = new[]
        {
            "salt lake city", "84109", "84111", "orem", "PROVO", "84109-1906", "84114", "RichField", "84113"
        };

        [OneTimeSetUp]
        public void Setup()
        {
            CacheConfig.BuildCache();
        }

        [Test]
        public void New()
        {
            var timer = new Stopwatch();

            timer.Start();

            foreach (var zone in _strings)
            {
                var addressBase =
                    CommandExecutor.ExecuteCommand(new ParseZoneCommand(zone, new GeocodeAddress(new CleansedAddress())));
            }

            timer.Stop();

            Debug.Print(timer.ElapsedMilliseconds.ToString());
        }
    }
}