using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.Common.Commands.Spatial;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ReprojectPointsCommandTests
    {
        /// <summary>
        /// {"geometries": [{
        ///  "x": -111.62445941538023,
        ///  "y": 40.590304872099466
        /// }]}
        /// </summary>
        [Test]
        public void CanReprojectSimplePoint()
        {
            var command =
                new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, 4326,
                                                                                            new List<double>
                                                                                                {
                                                                                                    447158,
                                                                                                    4493466
                                                                                                }));

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result.IsSuccessful, Is.True);
            Assert.That(Math.Round(command.Result.Geometries.First().X, 2), Is.EqualTo(-111.62));
            Assert.That(Math.Round(command.Result.Geometries.First().Y, 2), Is.EqualTo(40.59));
        }

        [Test]
        public void HandlesBadInput()
        {
            var command =
                new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, 0,
                                                                                            new List<double>
                                                                                                {
                                                                                                    447158,
                                                                                                    4493466
                                                                                                }));

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result.IsSuccessful, Is.False);
            Assert.That(command.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void CanReprojectMultiplePoint()
        {
            var command =
                new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, 4326,
                                                                                            new List<double>
                                                                                                {
                                                                                                    447158,
                                                                                                    4493466,
                                                                                                    418473.370852653, 
                                                                                                    4563753.59168596
                                                                                                }));

            command.Run();

            Assert.That(command.Result, Is.Not.Null);
            Assert.That(command.Result.IsSuccessful, Is.True);
            Assert.That(command.Result.Geometries.First().X, Is.EqualTo(-111.62447107405954d));
            Assert.That(command.Result.Geometries.First().Y, Is.EqualTo(40.590310911817163d));
            Assert.That(command.Result.Geometries.Skip(1).First().X, Is.EqualTo(-111.97264595627512d));
            Assert.That(command.Result.Geometries.Skip(1).First().Y, Is.EqualTo(41.221068629003206d));
        }
    }
}