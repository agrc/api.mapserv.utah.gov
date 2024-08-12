namespace ugrc.api.Models.Linkables;
public abstract class GridLinkable(string grid, int weight) : IWeighable {
    public abstract string Key { get; }
    public string Grid { get; set; } = grid;
    public int Weight { get; set; } = weight;

    public override string ToString() => $"{Grid},{Weight}";
}
