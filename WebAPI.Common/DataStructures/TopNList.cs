using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Common.DataStructures
{
    /// <summary>
    ///     List that keeps a sorted amount of N items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TopNList<T>
    {
        private readonly SortedSet<T> _items;

        protected TopNList(int nItems, IComparer<T> comparer)
        {
            Size = nItems;
            _items = new SortedSet<T>(comparer);
        }

        private int Size { get; set; }

        public virtual void Add(T candidate)
        {
            _items.Add(candidate);
            if (_items.Count > Size)
            {
                _items.Remove(_items.Last());
            }
        }

        public virtual IEnumerable<T> GetTopItems()
        {
            return _items.AsEnumerable();
        }
    }
}