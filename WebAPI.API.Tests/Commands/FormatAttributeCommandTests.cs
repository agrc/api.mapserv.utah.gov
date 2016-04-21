using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Commands.Search;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Tests.Commands
{
    public class FormatAttributeCommandTests
    {
        public static SearchResult Attribute = new SearchResult
            {
                Attributes = new Dictionary<string, object>
                    {
                        {"MIXEDCase", null},
                        {"lowercase", null},
                        {"UPPERCASE", null},
                        {"camelCase", null}
                    }
            };

        [TestFixture]
        public class UpperCase
        {
            [SetUp]
            public void Setup()
            {
                _command = new FormatAttributesCommand(AttributeStyle.Upper, new List<SearchResult>(new[]
                    {
                        Attribute
                    }));
            }

            private FormatAttributesCommand _command;

            [Test]
            public void MakesAttributesUpperCase()
            {
                _command.Run();

                Assert.That(_command.Result.Single().Attributes.Keys,
                            Is.EquivalentTo(new[] {"MIXEDCASE", "LOWERCASE", "UPPERCASE", "CAMELCASE"}));
            }
        }

        [TestFixture]
        public class LowerCase
        {
            [SetUp]
            public void Setup()
            {
                _command = new FormatAttributesCommand(AttributeStyle.Lower, new List<SearchResult>(new[]
                    {
                        Attribute
                    }));
            }

            private FormatAttributesCommand _command;

            [Test]
            public void MakesAttributesUpperCase()
            {
                _command.Run();

                Assert.That(_command.Result.Single().Attributes.Keys,
                            Is.EquivalentTo(new[] { "mixedcase", "lowercase", "uppercase", "camelcase" }));
            }
        }

        [TestFixture]
        public class Identical
        {
            [SetUp]
            public void Setup()
            {
                _command = new FormatAttributesCommand(AttributeStyle.Identical, new List<SearchResult>(new[]
                    {
                        Attribute
                    }));
            }

            private FormatAttributesCommand _command;

            [Test]
            public void MakesAttributesUpperCase()
            {
                _command.Run();

                Assert.That(_command.Result.Single().Attributes, Is.Empty, "app returns attributes as is no need to much with them");
            }
        }

        [TestFixture]
        public class Camel
        {
            [SetUp]
            public void Setup()
            {
                _command = new FormatAttributesCommand(AttributeStyle.Camel, new List<SearchResult>(new[]
                    {
                        Attribute
                    }));
            }

            private FormatAttributesCommand _command;

            [Test]
            public void MakesAttributesUpperCase()
            {
                _command.Run();

                Assert.That(_command.Result.Single().Attributes.Keys,
                            Is.EquivalentTo(new[] { "mixedCase", "lowercase", "uppercase", "camelCase" }));
            }
        }
    }
}