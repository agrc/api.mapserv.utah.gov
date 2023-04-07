namespace AGRC.api.Services;
/// <summary>
///     List that keeps a sorted amount of N items
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TopNList<T> {
    private readonly SortedSet<T> _items;

    protected TopNList(int nItems, IComparer<T> comparer) {
        Size = nItems;
        _items = new SortedSet<T>(comparer);
    }

    private int Size { get; }

    public virtual void Add(T candidate) {
        _items.Add(candidate);
        if (_items.Count > Size) {
            _items.Remove(_items.Last());
        }
    }

    public virtual IList<T> Get() => _items.ToList();
}
