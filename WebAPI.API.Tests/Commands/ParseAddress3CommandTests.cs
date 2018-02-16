using System;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Domain;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ParseAddress3CommandTests
    {
        [SetUp]
        public void BuildCache()
        {
            App.UnitAbbreviations = CacheConfig.CacheUnitAbbreviations();
            App.RegularExpressions = CacheConfig.CacheRegularExpressions();
            App.StreetTypeAbbreviations = CacheConfig.CacheStreetTypeAbbreviations();
        }

        [TestCase("123 north st.")]
        [TestCase("123 north-west st")]
        [TestCase("1:23 north-west st")]
        public void RemoveExtraniousCharacters(string street)
        {
            var command = new ParseAddress3Command(street);

            var result = command.RemoveExtraniousCharacters(street);

            Console.WriteLine(result);
            Assert.That(result.Contains("."), Is.False);
            Assert.That(result.Contains("-"), Is.False);
            Assert.That(result.Contains(":"), Is.False);
        }

        [TestFixture]
        public class GetStreetParts
        {
            [TestCase("123 north st", 3)]
            [TestCase("123 northwest st", 3)]
            public void ReturnsCorrectCount(string street, int parts)
            {
                var command = new ParseAddress3Command(street);

                var result = command.CreateStreetParts(street);

                Assert.That(result.Count(), Is.EqualTo(parts));
            }

            [Test]
            public void CreatesLinkedList()
            {
                const string street = "123 north st";
                var command = new ParseAddress3Command(street);

                var result = command.CreateStreetParts(street).ToArray();

                var first = result.Single(x => x.Index == 0);
                var second = result.Single(x => x.Index == 1);
                var third = result.Single(x => x.Index == 2);

                Assert.That(first.IsFirst, Is.True);
                Assert.That(first.IsLast, Is.False);
                Assert.That(first.Right, Is.EqualTo(result[1]));
                Assert.That(first.Left, Is.Null);

                Assert.That(second.IsFirst, Is.False);
                Assert.That(second.IsLast, Is.False);
                Assert.That(second.Right, Is.EqualTo(result[2]));
                Assert.That(second.Left, Is.EqualTo(result[0]));

                Assert.That(third.IsFirst, Is.False);
                Assert.That(third.IsLast, Is.True);
                Assert.That(third.Right, Is.Null);
                Assert.That(third.Left, Is.EqualTo(result[1]));
            }
        }

        [TestFixture]
        public class StreetParts
        {
            [SetUp]
            public void BuildCache()
            {
                App.UnitAbbreviations = CacheConfig.CacheUnitAbbreviations();
                App.RegularExpressions = CacheConfig.CacheRegularExpressions();
                App.StreetTypeAbbreviations = CacheConfig.CacheStreetTypeAbbreviations();
            }

            [TestCase("", false)]
            [TestCase("1", true)]
            [TestCase("123", true)]
            [TestCase("abc", false)]
            [TestCase("a1b2", false)]
            public void IsNumber(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsNumber, Is.EqualTo(expectation));
            }

            [TestCase("", false)]
            [TestCase("n", true)]
            [TestCase("no", true)]
            [TestCase("north", true)]
            [TestCase("northwind", false)]
            [TestCase("1", false)]
            [TestCase("abc", false)]
            public void IsDirection(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsDirection, Is.EqualTo(expectation));
            }

            [TestCase("", false)]
            [TestCase("14th", true)]
            [TestCase("3rd", true)]
            [TestCase("22nd", true)]
            [TestCase("1", false)]
            [TestCase("abc", false)]
            public void IsOrdinal(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsOrdinal, Is.EqualTo(expectation));
            }

            [TestCase("", false)]
            [TestCase("HWY", true)]
            [TestCase("highway", true)]
            [TestCase("SR", true)]
            [TestCase("stateroute", true)]
            [TestCase("US", true)]
            [TestCase("U.S.", true)]
            [TestCase("U?s.", false)]
            [TestCase("U.T", false)]
            public void IsHighway(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsHighway, Is.EqualTo(expectation));
            }

            [TestCase("", false)]
            [TestCase("HWY", false)]
            [TestCase("highway", false)]
            [TestCase("HWY68", true)]
            [TestCase("SR", false)]
            [TestCase("stateroute", false)]
            [TestCase("SR68", true)]
            [TestCase("US", false)]
            [TestCase("US89", true)]
            [TestCase("123west", true)]
            [TestCase("456south", true)]
            [TestCase("U.S.", false)]
            [TestCase("U?s.", false)]
            [TestCase("U.T", false)]
            public void NeedsSplit(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.NeedsSplit, Is.EqualTo(expectation));
            }

            [TestCase("st", StreetType.Street)]
            [TestCase("drive", StreetType.Drive)]
            [TestCase("", StreetType.None)]
            public void GetStreetType(string value, object expectation)
            {
                var output = ParseAddress3Command.StreetPart.GetStreetType(value);
                Assert.That(output, Is.EqualTo(expectation));
            }
        }
    }
}