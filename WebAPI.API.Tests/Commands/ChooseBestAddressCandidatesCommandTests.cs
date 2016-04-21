using System.Linq;
using NUnit.Framework;
using WebAPI.API.Comparers;
using WebAPI.API.DataStructures;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.Commands
{
    [TestFixture]
    public class ChooseBestAddressCandidatesCommandTests
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
    }
}