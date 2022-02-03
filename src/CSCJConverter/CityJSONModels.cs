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
    public decimal? h_maaiveld { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string identificatie { get; set; }
}

/// <summary>
/// Class to represent geometry of a CityObject
/// Currently only support for Solid type, since 3D BAG only uses solid
/// </summary>
public class GeometrySolid
{
    public List<List<List<List<int>>>> boundaries { get; set; }
    public string lod { get; set; }
    public JsonObject semantics { get; set; }
    public string type { get; set; }
}

/// <summary>
/// Class to represent a CityObject
/// </summary>
public class CityObject
{
    public Attributes attributes { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string> children { get; set; }
    public List<GeometrySolid> geometry { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string> parents { get; set; }
    public string type { get; set; }
}

/// <summary>
/// Class to represent a transform object
/// </summary>
public class Transform
{
    public List<decimal> scale { get; set; }
    public List<decimal> translate { get; set; }
}

/// <summary>
/// Class to represent the 'root' of a CityJSON file
/// </summary>
public class CityJSONModel
{
    public string type { get; set; }
    public string version { get; set; }
    public Dictionary<string, CityObject> CityObjects { get; set; }
    public List<List<int>> vertices { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonObject extensions { get; set; }
    public Transform transform { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonObject metadata { get; set; }
    
    [JsonPropertyName("+metadata-extended")]
    public JsonObject MetadataExtended { get; set; }
}