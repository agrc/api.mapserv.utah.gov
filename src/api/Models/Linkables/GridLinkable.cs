namespace ugrc.api.Models.Linkables;
public abstract class GridLinkable : IWeighable {
    protected GridLinkable(string grid, int weight) {
        Grid = grid;
        Weight = weight;
    }

    public abstract string Key { get; }
    public string Grid { get; set; }
    public int Weight { get; set; }

    public override string ToString() => $"{Grid},{Weight}";
}
