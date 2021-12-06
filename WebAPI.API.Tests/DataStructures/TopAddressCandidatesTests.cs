using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.API.Comparers;
using WebAPI.API.DataStructures;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.DataStructures
{
    [TestFixture]
    public class TopAddressCandidatesTests
    {
        [Test]
        public void GridWeightsMatter()
        {
            const int topItemCount = 3;
            const string tieBreakerInput = "GOLD";
            var topCandidates = new TopAddressCandidates(topItemCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate
            {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "GOLDS",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate
            {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate
            {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            var items = topCandidates.GetTopItems();

            const int addOneForWinnerWhichIsRemoved = 1;

            Assert.That(items.Count(), Is.EqualTo(topItemCount + addOneForWinnerWhichIsRemoved));
            var candidate = items.First();
            Assert.That(candidate.Score, Is.EqualTo(5));
            Assert.That(candidate.Address, Is.EqualTo("GOLD"));
        }

        [Test]
        public void SizeIsTwoWhenSuggestIsZeroForScoreDifferenceCalculating()
        {
            const int suggestCount = 0;
            const string tieBreakerInput = "";
            var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate
            {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "GOLDS",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate
            {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate
            {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            Assert.That(topCandidates.GetTopItems().ToList().Count, Is.EqualTo(2));
        }

        [Test]
        public void SizeIsOneLargerThanInputToGetExactSuggestionCount()
        {
            const int suggestCount = 2;
            const string tieBreakerInput = "";
            var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(tieBreakerInput));

            topCandidates.Add(new Candidate
            {
                Address = "GOLD",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "GOLDS",
                Score = 5,
                Weight = 100
            });

            topCandidates.Add(new Candidate
            {
                Address = "BRONZE",
                Score = 5,
                Weight = 1
            });

            topCandidates.Add(new Candidate
            {
                Address = "SILVER",
                Score = 5,
                Weight = 50
            });

            topCandidates.Add(new Candidate
            {
                Address = "Runner up",
                Score = 5,
                Weight = 0
            });

            Assert.That(topCandidates.GetTopItems().ToList().Count, Is.EqualTo(suggestCount + 1));
        }

        [Test]
        public void TiesTakeHighRank()
        {
            const int suggestCount = 2;
            const string address = "669 3rd ave";
            var topCandidates = new TopAddressCandidates(suggestCount, new CandidateComparer(address.ToUpperInvariant()));

            var streetCandidates = new List<Candidate>
            {
                new Candidate
                {
                    Address = "669 W 3RD AVE",
                    Score = 90.87,
                    Weight = 1
                },
                new Candidate
                {
                    Address = "669 E 3RD AVE",
                    Score = 90.87,
                    Weight = 1
                },
                new Candidate
                {
                    Address = "670 W 3RD AVE",
                    Score = 69.87,
                    Weight = 1
                },
                new Candidate
                {
                    Address = "670 E 3RD AVE",
                    Score = 69.87,
                    Weight = 1
                }
            };

            var addressPointCandidates = new List<Candidate>
            {
                new Candidate
                {
                    Address = "669 W THIRD AVE",
                    Score = 90.87,
                    Weight = 2
                },
                new Candidate
                {
                    Address = "669 E 3RD AVE",
                    Score = 90.87,
                    Weight = 2
                }
            };

            addressPointCandidates.ForEach(topCandidates.Add);
            streetCandidates.ForEach(topCandidates.Add);

            var items = topCandidates.GetTopItems();

            const int addOneForWinnerWhichIsRemoved = 1;

            Assert.That(items.Count(), Is.EqualTo(suggestCount + addOneForWinnerWhichIsRemoved));
            var topItems = topCandidates.GetTopItems().ToList();
            topItems.ForEach(x => Assert.That(x.Score, Is.EqualTo(10)));
            Assert.That(topCandidates.GetTopItems().First().Address, Is.EqualTo("101 e 3rd ave"));
        }
    }
}