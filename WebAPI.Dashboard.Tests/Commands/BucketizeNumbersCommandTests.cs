using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebAPI.Common.Extensions;

namespace WebAPI.Dashboard.Tests.Commands
{
    [TestFixture]
    public class BucketizeNumbersCommandTests
    {
        [Test, Explicit]
        public void Test()
        {
            decimal geocodeCalls = 100,
                    query = 5,
                    info = 0,
                    route = 0;

            var list = new List<decimal>
            {
                geocodeCalls,
                query,
                info,
                route
            }.AsEnumerable();

            var buckets = list.Bucketize(2);
        }
    }
}