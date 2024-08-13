namespace ugrc.api.Services;
/// <summary>
///     List that keeps a sorted amount of N items
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TopNList<T>(int nItems, IComparer<T> comparer) {
    private readonly SortedSet<T> _items = new(comparer);

    private int Size { get; } = nItems;

    public virtual void Add(T candidate) {
        if (_items.Count < Size) {
            _items.Add(candidate);
        } else if (comparer.Compare(candidate, _items.Max) < 0) {
            var max = _items.Max;

            if (max is not null) {
                _items.Remove(max);
            }
            _items.Add(candidate);
        }
    }

    public virtual IList<T> Get() => [.. _items];
}
