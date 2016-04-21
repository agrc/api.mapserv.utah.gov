using NUnit.Framework;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Tests.Extensions
{
    [TestFixture]
    public class ObjectExtensionTests
    {
         [Test]
         public void ConvertToqueryString()
         {
             var args = new QueryArgs("test", new[] {"a", "b"}, "me");
             Assert.That(args.ToQueryString(), Is.EqualTo("featureClass=test&returnValues=a%2Cb&predicate=me"));
         }
    }
}