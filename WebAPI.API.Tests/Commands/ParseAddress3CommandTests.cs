using System;
using System.Collections.Generic;
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

        [Test]
        public void SplitsProperly()
        {
            

//            var partOne = parts.Single(x => x.Index == 0);
//            Assert.That(partOne.GetValue(), Is.EqualTo(123));
//
//            var partTwo = parts.Single(x => x.Index == 1);
//            Assert.That(partTwo.GetValue(), Is.EqualTo(Direction.West));
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
            [SetUp]
            public void BuildCache()
            {
                App.UnitAbbreviations = CacheConfig.CacheUnitAbbreviations();
                App.RegularExpressions = CacheConfig.CacheRegularExpressions();
                App.StreetTypeAbbreviations = CacheConfig.CacheStreetTypeAbbreviations();
            }

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
                var command = new ParseAddress3Command(null);

                var result = command.CreateStreetParts(street);

                var node = result.First;

                Assert.That(node.Value.Index, Is.EqualTo(0));

                node = node.Next;

                Assert.That(node.Value.Index, Is.EqualTo(1));

                node = node.Next;

                Assert.That(node.Value.Index, Is.EqualTo(2));
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
            [TestCase("123north", false)]
            [TestCase("north123", false)]
            [TestCase("123north123", false)]
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
            [TestCase("123hwy", false)]
            [TestCase("123hwy123", false)]
            [TestCase("hwyx", false)]
            [TestCase("xhwyx", false)]
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

            [TestCase("st", StreetType.Street)]
            [TestCase("drive", StreetType.Drive)]
            [TestCase("", StreetType.None)]
            [TestCase("123west", StreetType.None)]
            public void GetStreetType(string value, object expectation)
            {
                var output = ParseAddress3Command.StreetPart.GetStreetType(value);
                Assert.That(output, Is.EqualTo(expectation));
            }

            [TestCase("st", true)]
            [TestCase("drive", true)]
            [TestCase("", false)]
            [TestCase("xst", false)]
            [TestCase("123west", false)]
            public void IsStreetType(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);
                Assert.That(part.IsStreetType, Is.EqualTo(expectation));
            }

            [TestCase("", new string[]{})]
            [TestCase("HWY68", new[] { "HWY", "68" })]
            [TestCase("SR68", new[] { "SR", "68" })]
            [TestCase("US89", new[] { "US", "89" })]
            [TestCase("123west", new[] { "123", "west" })]
            [TestCase("south456", new[] { "south", "456" })]
            public void Split(string value, string[] parts)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);
                var splits = ParseAddress3Command.StreetPart.Split(part);

                Assert.That(splits, Is.EqualTo(parts));
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

            [Test]
            public void HandleSimpleSplit()
            {
                var list = new LinkedList<ParseAddress3Command.StreetPart>();
                list.AddFirst(new ParseAddress3Command.StreetPart("123west", 0));

                var command = new ParseAddress3Command(null);
                var actual = command.HandleSplits(list);

                Assert.That(actual.Count, Is.EqualTo(2));     
                Assert.That(actual.First.Value.GetValue(), Is.EqualTo(123));
                Assert.That(actual.Last.Value.GetValue(), Is.EqualTo(Direction.West));
            }

            [Test]
            public void HandleSplit()
            {
                var list = new LinkedList<ParseAddress3Command.StreetPart>();
                list.AddFirst(new ParseAddress3Command.StreetPart("123west", 0));
                list.AddLast(new ParseAddress3Command.StreetPart("st", 1));

                var command = new ParseAddress3Command(null);
                var actual = command.HandleSplits(list);

                Assert.That(actual.Count, Is.EqualTo(3));

                var node = actual.First;
                Assert.That(node.Value.GetValue(), Is.EqualTo(123));

                node = node.Next;
                Assert.That(node.Value.GetValue(), Is.EqualTo(Direction.West));

                node = node.Next;
                Assert.That(node.Value.GetValue(), Is.EqualTo(StreetType.Street));
            }
        }
    }
}