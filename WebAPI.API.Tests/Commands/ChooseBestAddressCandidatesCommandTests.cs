using System.Collections.Generic;
using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ChooseBestAddressCandidatesCommandTests
    {
        [Test]
        public void ScoreDifferenceCalculatedWhenSuggestFalse()
        {
            // these are sorted out of the TopNList by the CandidateComparer
            var candidates = new List<Candidate>
            {
                 new Candidate
                {
                    Address = "top match",
                    Score = 100,
                    Weight = 0,
                    AddressGrid = "Grid",
                    Location = new Location(0, 0)
                },
                new Candidate
                {
                    Address = "top suggestion",
                    Score = 90.87,
                    Weight = 0,
                    AddressGrid = "Grid"
                },
                 new Candidate
                {
                    Address = "suggestion",
                    Score = 80.87,
                    Weight = 0,
                    AddressGrid = "Grid"
                },
                new Candidate
                {
                    Address = "another suggestion",
                    Score = 70.87,
                    Weight = 0,
                    AddressGrid = "Grid"
                }
            };

            var options = new GeocodeOptions
            {
                ScoreDifference = true
            };

            var address = new GeocodeAddress(new CleansedAddress("", 0, 0, 0, Direction.None, "", StreetType.None, Direction.None, 0, 0, false, false))
            {
                AddressGrids = new List<GridLinkable>
                {
                    new PlaceGridLink("City", "Grid", 0)
                }
            };

            var command = new ChooseBestAddressCandidateCommand(candidates,
                options,
                "top match",
                "test",
                address);

            command.Run();
            var match = command.Result;

            Assert.That(match.ScoreDifference, Is.EqualTo(100 - 90.87));
        }

        [Test]
        public void ScoreDifferenceCalculatedWhenSuggestFalseAndNoSuggestions()
        {
            var candidates = new List<Candidate>
            {
                new Candidate
                {
                    Address = "top match",
                    Score = 100,
                    Weight = 0,
                    AddressGrid = "Grid",
                    Location = new Location(0, 0)
                }
            };

            var options = new GeocodeOptions
            {
                ScoreDifference = true
            };

            var address = new GeocodeAddress(new CleansedAddress("", 0, 0, 0, Direction.None, "", StreetType.None, Direction.None, 0, 0, false, false))
            {
                AddressGrids = new List<GridLinkable>
                {
                    new PlaceGridLink("City", "Grid", 0)
                }
            };

            var command = new ChooseBestAddressCandidateCommand(candidates,
                options,
                "top match",
                "test",
                address);

            command.Run();
            var match = command.Result;

            Assert.That(match.ScoreDifference, Is.EqualTo(0));
        }

        [Test]
        public void ScoreDifferenceDoesNotCalculatedWhenSuggestIsGreatherThanZero()
        {
            var candidates = new List<Candidate>
            {
                new Candidate
                {
                    Address = "top match",
                    Score = 100,
                    Weight = 0,
                    AddressGrid = "Grid",
                    Location = new Location(0, 0)
                },
                new Candidate
                {
                    Address = "top suggestion",
                    Score = 90.87,
                    Weight = 0,
                    AddressGrid = "Grid"
                }
            };

            var options = new GeocodeOptions
            {
                SuggestCount = 1
            };

            var address = new GeocodeAddress(new CleansedAddress("", 0, 0, 0, Direction.None, "", StreetType.None, Direction.None, 0, 0, false, false))
            {
                AddressGrids = new List<GridLinkable>
                {
                    new PlaceGridLink("City", "Grid", 0)
                }
            };

            var command = new ChooseBestAddressCandidateCommand(candidates,
                options,
                "top match",
                "test",
                address);

            command.Run();
            var match = command.Result;

            Assert.That(match.Score, Is.EqualTo(100));
            Assert.That(match.Candidates.Length, Is.EqualTo(1));
        }
    }
}