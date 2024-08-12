namespace ugrc.api.Services;
/// <summary>
///     List that keeps a sorted amount of N items
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TopNList<T>(int nItems, IComparer<T> comparer) {
    private readonly SortedSet<T> _items = new SortedSet<T>(comparer);

    private int Size { get; } = nItems;

    public virtual void Add(T candidate) {
        _items.Add(candidate);
        if (_items.Count > Size) {
            _items.Remove(_items.Last());
        }
    }

    public virtual IList<T> Get() => _items.ToList();
}
