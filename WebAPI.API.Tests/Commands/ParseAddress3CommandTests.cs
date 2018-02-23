using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;
using WebAPI.Domain;

namespace WebAPI.API.Tests.Commands
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void BuildCache()
        {
            App.UnitAbbreviations = CacheConfig.CacheUnitAbbreviations();
            App.StreetTypeAbbreviations = CacheConfig.CacheStreetTypeAbbreviations();
            App.RegularExpressions = CacheConfig.CacheRegularExpressions();
        }
    }

    [TestFixture]
    public class ParseAddress3CommandTests
    {
        [TestCase("123 northwest st.")]
        [TestCase("(123 northwest st)")]
        [TestCase("123 north-west st")]
        [TestCase("1:23 north-west st")]
        public void RemoveExtraniousCharacters(string street)
        {
            var command = new ParseAddress3Command(street);

            var result = command.RemoveExtraniousCharacters(street);

            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo("123 northwest st"));
        }

        public class GetStreetParts
        {
            [TestCase("123 north st", 3)]
            [TestCase("123 northwest st", 3)]
            public void ReturnsCorrectCount(string street, int parts)
            {
                var command = new ParseAddress3Command(street);

                var result = command.CreateStreetParts(street);

                Assert.That(result.Count, Is.EqualTo(parts));
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

        public class StreetParts
        {
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
            [TestCase("and", true)]
            [TestCase("at", true)]
            [TestCase("@", true)]
            [TestCase("|", true)]
            [TestCase("1", false)]
            [TestCase("abc", false)]
            public void IsIntersection(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsIntersection, Is.EqualTo(expectation));
            }

            [TestCase("", false)]
            [TestCase("pobox", true)]
            [TestCase("pob", true)]
            [TestCase("po", true)]
            [TestCase("box", true)]
            [TestCase("pbox", true)]
            [TestCase("1", false)]
            [TestCase("abc", false)]
            [TestCase("pobbox", false)]
            [TestCase("pb", true)]
            public void IsPoBox(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);

                Assert.That(part.IsPoBox, Is.EqualTo(expectation));
            }

            [TestCase("bsmt", true)]
            [TestCase("PENTHOUSE", true)]
            [TestCase("unit", true)]
            [TestCase("lot", true)]
            [TestCase("TRLR", true)]
            [TestCase("bldg", true)]
            [TestCase("ste", true)]
            [TestCase("#", true)]
            public void IsSecondary(string value, bool expectation)
            {
                var part = new ParseAddress3Command.StreetPart(value, 0);
                Assert.That(part.IsSecondary, Is.EqualTo(expectation));
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

        public class StreetAnalysis
        {
            [TestCase("See Listing See Listing", false)]
            [TestCase("326 east south temple st", true)]
            [TestCase("150 East", true)]
            [TestCase("150", false)]
            [TestCase("150 150", false)]
            [TestCase("APT 201 714 N 900 W", true)]
            [TestCase("APT 201", false)]
            public void PossibleAddress(string street, bool expectation)
            {
                var command = new ParseAddress3Command(street);

                var parts = command.CreateStreetParts(street);
                var result = new ParseAddress3Command.StreetAnalysis(parts);

                Assert.That(result.PossibleAddress, Is.EqualTo(expectation));
            }
        }
    }
}