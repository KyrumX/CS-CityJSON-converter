using System.Text.Json.Serialization;

namespace CSCJConverter.tileset;


/// <summary>
/// Tileset model for a 3D Tiles tileset.json file.
/// Does not fully implement the spec (e.g. not all options are allowed!)
/// </summary>
public class BoxVolume
{
    public decimal[] box { get; init; }
}

public class Content
{
    public string uri { get; init; }
}

public class Tile
{
    public BoxVolume boundingVolume { get; init; }
    public Content content { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Tile> children { get; init; }
    public decimal geometricError { get; init; }
}

public class Asset
{
    public string version { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string tilesetVersion { get; init; }
    public string gltfUpAxis { get; init; }
}

public class Root
{
    public BoxVolume boundingVolume { get; init; }
    public int[] transform { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<Tile> children { get; init; }
    public decimal geometricError { get; init; }
    public string refine { get; init; }
}

public class TilesetModel
{
    public Asset asset { get; init; }
    public Root root { get; init; }
    public decimal geometricError { get; init; }
}