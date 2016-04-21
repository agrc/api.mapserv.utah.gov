using System;
using System.Linq;
using NUnit.Framework;
using WebAPI.Search.Soe.Commands;

namespace WebAPI.Search.Soe.Tests.Commands
{
    public class ExtractGeometryCommandTests
    {
        [TestFixture]
        public class Point
        {
            [Test]
            public void FractionalCoordinatesDeserializesCorrectly()
            {
                var command = new ExtractGeometryCommand("point:[1.1231236, 2.321561]");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POINT"));
                Assert.That(result.Coordinates.Single(), Is.EquivalentTo(new[] {1.1231236, 2.321561}));
            }

            [Test]
            public void NegativeCoordinatesDeserializesCorrectly()
            {
                var command = new ExtractGeometryCommand("point:[1,-2]");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POINT"));
                Assert.That(result.Coordinates.Single(), Is.EquivalentTo(new[] {1d, -2}));
            }

            [Test]
            public void MissingBracketsDeserializesCorrectly()
            {
                var command = new ExtractGeometryCommand("point:1,2");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POINT"));
                Assert.That(result.Coordinates.Single(), Is.EquivalentTo(new[] { 1d, 2 }));
            }

            [Test]
            public void SimpleCoordinatesDeserializesCorrectly()
            {
                var command = new ExtractGeometryCommand("point:[1, 2]");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POINT"));
                Assert.That(result.Coordinates.Single(), Is.EquivalentTo(new[] {1d, 2d}));
            }
        }

        [TestFixture, Explicit("Not Implemented")]
        public class Polyline
        {
            [Test, ExpectedException(typeof(ArgumentException))]
            public void LineThrowsErrorUnlessThereAreTwoOrMorePoints()
            {
                var command = new ExtractGeometryCommand("polyline:[1]");
                command.Run();
            }

            [Test]
            public void SimplePolylineDeserializesCorreclty()
            {
                var command = new ExtractGeometryCommand("polyline:[[1, 2],[2,3]]");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POLYLINE"));
                Assert.That(result.Coordinates, Is.EquivalentTo(new[,] {{1d, 2d}, {2, 3}}));
            }
        }

        [TestFixture, Explicit("Not Implemented")]
        public class Polygon
        {
            [Test, ExpectedException(typeof(ArgumentException))]
            public void PolygonThrowsErrorUnlessThereAreThreeOrMorePoints()
            {
                var command = new ExtractGeometryCommand("polygon:[1, 2]");
                command.Run();
            }

            [Test]
            public void SimplePolygonDeserializesCorreclty()
            {
                var command = new ExtractGeometryCommand("polygon:[[1, 2],[2,3],[1, 2]]");
                command.Run();

                var result = command.Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo("POLYGON"));
                Assert.That(result.Coordinates, Is.EquivalentTo(new[,] { { 1d, 2d }, { 2, 3 } }));
            }
        }
    }
}