using System.Collections.Generic;
using NUnit.Framework;
using WebAPI.Domain.DataStructures;

namespace WebAPI.Domain.Tests.DataStructures
{
    [TestFixture]
    public class TopAndEqualListTests
    {
        [SetUp]
        public void Startup()
        {
            _list = new TopItemList(new ItemComparer());
        }

        private TopItemList _list;

        public class TopItemList : TopAndEqualsList<Item>
        {
            public TopItemList(IComparer<Item> comparer) : base(comparer)
            {
            }
        }

        public class Item
        {
            public Item(double value, string name)
            {
                Value = value;
                Name = name;
            }

            public double Value { get; set; }
            public string Name { get; set; }
        }

        public class ItemComparer : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {
                var compareTo = y.Value.CompareTo(x.Value);

                return compareTo;
            }
        }

        [Test]
        public void ClearsOutEqualsListOnNewTopResult()
        {
            _list.Add(new Item(2, "first"));
            _list.Add(new Item(2, "second"));

            Assert.That(_list.EqualCandidates.Count, Is.EqualTo(1));

            _list.Add(new Item(1, "third"));

            Assert.That(_list.TopResult.Name, Is.EqualTo("third"));
            Assert.That(_list.EqualCandidates, Is.Empty);
        }

        [Test]
        public void AddsEqualsToEqualsList()
        {
            _list.Add(new Item(2, "first"));
            _list.Add(new Item(2, "second"));

            Assert.That(_list.EqualCandidates.Count, Is.EqualTo(1));

            _list.Add(new Item(1, "third"));
            _list.Add(new Item(1, "fourth"));
            _list.Add(new Item(1, "fifth"));

            Assert.That(_list.TopResult.Name, Is.EqualTo("third"));
            Assert.That(_list.EqualCandidates.Count, Is.EqualTo(2));
        }
    }
}