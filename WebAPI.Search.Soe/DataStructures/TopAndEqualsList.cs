using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebAPI.Search.Soe.DataStructures
{
    /// <summary>
    /// Keeps the first result and adds equal values to the equals list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TopAndEqualsList<T> where T : class
    {
        private readonly IComparer<T> _comparer;

        protected TopAndEqualsList(IComparer<T> comparer)
        {
            _comparer = comparer;
            Candidates = new Collection<T>();
        }

        public T TopResult { get; set; }

        private ICollection<T> Candidates { get; set; }

        public ICollection<T> EqualCandidates
        {
            get { return Candidates; }
        }

        public void Add(T item)
        {
            if (TopResult == null)
            {
                TopResult = item;
                return;
            }

            var result = _comparer.Compare(TopResult, item);

            if (result == 0)
            {
                Candidates.Add(item);
                return;
            }

            if (result > 0)
            {
                return;
            }

            TopResult = item;
            Candidates.Clear();
        }
    }
}