using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CSCJConverter;

/// <summary>
/// Attributes class for attributes found on a CityObject
/// Currently only supports 2 fixed attributes, h_maaiveld and identificatie
/// </summary>
public class Attributes
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal? h_maaiveld { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string identificatie { get; init; }
}

/// <summary>
/// Class to represent geometry of a CityObject
/// Currently only support for Solid type, since 3D BAG only uses solid
/// </summary>
public class GeometrySolid
{
    public List<List<List<List<int>>>> boundaries { get; set; }
    public string lod { get; init; }
    public JsonObject semantics { get; init; }
    public string type { get; init; }
}

/// <summary>
/// Class to represent a CityObject
/// </summary>
public class CityObject
{
    public Attributes attributes { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string> children { get; init; }
    public List<GeometrySolid> geometry { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string> parents { get; init; }
    public string type { get; init; }
}

/// <summary>
/// Class to represent a transform object
/// </summary>
public class Transform
{
    public List<decimal> scale { get; init; }
    public List<decimal> translate { get; init; }
}

/// <summary>
/// Class to represent the 'Metadata' of a CityJSON file
/// </summary>
public class Metadata
{
    public string identifier { get; init; }
    public JsonObject pointOfContact { get; init; }
    public string title { get; init; }
    public string referenceDate { get; init; }
    public double[] geographicalExtent { get; init; }
    public string referenceSystem { get; init; }
    
}

/// <summary>
/// Class to represent the 'root' of a CityJSON file
/// </summary>
public class CityJSONModel
{
    public string type { get; init; }
    public string version { get; init; }
    public Dictionary<string, CityObject> CityObjects { get; init; }
    public List<List<int>> vertices { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonObject extensions { get; init; }
    public Transform transform { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Metadata metadata { get; init; }
    
    [JsonPropertyName("+metadata-extended")]
    public JsonObject MetadataExtended { get; init; }
}