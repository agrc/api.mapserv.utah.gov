using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Address;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ParseAddress3CommandTests
    {
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
                Assert.That(first.GetValue(), Is.EqualTo("123"));

                Assert.That(second.IsFirst, Is.False);
                Assert.That(second.IsLast, Is.False);
                Assert.That(second.Right, Is.EqualTo(result[2]));
                Assert.That(second.Left, Is.EqualTo(result[0]));
                Assert.That(second.GetValue(), Is.EqualTo("north"));

                Assert.That(third.IsFirst, Is.False);
                Assert.That(third.IsLast, Is.True);
                Assert.That(third.Right, Is.Null);
                Assert.That(third.Left, Is.EqualTo(result[1]));
                Assert.That(third.GetValue(), Is.EqualTo("st"));
            }
        }
    }
}