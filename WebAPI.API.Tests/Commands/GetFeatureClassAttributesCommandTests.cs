using NUnit.Framework;
using WebAPI.API.Commands.Info;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class GetFeatureClassAttributesCommandTests
    {
        [Test]
        public void CanGetAttributesWithSchema()
        {
            var command = new GetFeatureClassAttributesCommand("COUNTIES", "BOUNDARIES", "10");

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result, Is.Not.Empty);
        }

        [Test]
        public void CanGetAttributesFromSgid10()
        {
            var command = new GetFeatureClassAttributesCommand("COUNTIES", null, "10");
            
            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result, Is.Not.Empty);
        }

        [Test]
        public void CanGetAttributesFromSgidImplicitly()
        {
            var command = new GetFeatureClassAttributesCommand("COUNTIES");

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result, Is.Not.Empty);
        }

        [Test]
        public void TableNotFound()
        {
            var command = new GetFeatureClassAttributesCommand("MISSING");

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result, Is.Empty);
        }
    }
}