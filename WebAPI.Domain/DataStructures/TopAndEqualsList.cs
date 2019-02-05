using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebAPI.Domain.DataStructures
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
        }

        public T TopResult { get; set; }

        public ICollection<T> CandidatesNearby { get; set; } = new Collection<T>();

        public ICollection<T> EqualCandidates { get; set; } = new Collection<T>();

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
                EqualCandidates.Add(item);
                return;
            }

            if (result > 0)
            {
                CandidatesNearby.Add(item);
                return;
            }

            TopResult = item;
            foreach (var candidate in EqualCandidates)
            {
              CandidatesNearby.Add(candidate);  
            }

            EqualCandidates.Clear();
        }
    }
}