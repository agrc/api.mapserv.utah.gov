using MinimumEditDistance;
using NUnit.Framework;

namespace WebAPI.API.Tests
{
    [TestFixture]
    public class LevenshteinTests
    {
        /// <summary>
        ///     Less than zero     x is less than y.
        ///     Zero     x equals y.
        ///     Greater than zero     x is greater than y.
        /// </summary>
        [Test]
        public void WhatDoTheNumbersMean()
        {
            const string input = "339 E 200 S";
            const string candidate1 = "339 E 200 S";
            const string candidate2 = "339 E 200 N";

            var xDistance = Levenshtein.CalculateDistance(input, candidate1, 1);
            var yDistance = Levenshtein.CalculateDistance(input, candidate2, 1);

            var comparison = xDistance.CompareTo(yDistance);

            // since smaller is better
            Assert.That(comparison, Is.EqualTo(-1));
        }
    }
}