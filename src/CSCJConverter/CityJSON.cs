using System.Text.Encodings.Web;
using System.Text.Json;

namespace CSCJConverter;

/// <summary>
/// Class to work with CityJSON objects
/// </summary>
public class CityJSON
{
    private CityJSONModel _cityJson;
    private readonly string _outputPath;
    
    /// <summary>
    /// Constructor of the CityJSON class used to work with CityJSON objects.
    /// Automatically deserializes (JSON) string
    /// </summary>
    /// <param name="jsonString">
    ///     A string, result of File.ReadAllText function
    /// </param>
    /// <param name="outPath">
    ///     The output path and filename + file extension (json), example:
    ///     E:\my\path\filename.json
    /// </param>
    public CityJSON(string jsonString, string outPath)
    {
        this._outputPath = outPath;
        this.Deserialize(jsonString);
    }
    
    /// <summary>
    /// Deserialize a JSON file
    /// </summary>
    private void Deserialize(string jsonString)
    {
        // TODO: Check if file was loaded and deserialized correctly?
        this._cityJson = JsonSerializer.Deserialize<CityJSONModel>(jsonString);
    }

    /// <summary>
    /// Serialize a CityJSON object to a JSON file
    /// </summary>
    public void Serialize()
    {
        Console.WriteLine("Warning! UnsafeRelaxedJsonEscaping is being used. \n" +
                          "Do not use the output of this function directly on publicly accessible content, such as \n" +
                          "HTML pages without escaping/encoding it first!");
        
        // TODO: Disable UnsafeRelaxedJsonEscaping?
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        string serializeString = JsonSerializer.Serialize(this._cityJson, options);
        string outFileName = @_outputPath; 
        File.WriteAllText(outFileName, serializeString);
    }

    /// <summary>
    /// Moves the z-axes (height) of CityJSON objects to z=0 by using the h_maaiveld value
    /// </summary>
    public void TranslateHeightMaaiveld()
    {
        // Alleen 'parent' objecten hebben de h_maaiveld attribuut, de objecten staan
        // allemaal in dezelfde lijst, 'children' dus ook. Loop één keer over de lijst en
        // verzamel de h_maaiveld waarde (value) en koppel deze aan een het id van het kind (key)
        Dictionary<string, decimal?> childMaaiveldDict = this.CollectMaaiveldValues();
        
        if (childMaaiveldDict.Count == 0)
            return;

        // Hoekpunten kunnen worden hergebruikt, om te voorkomen dat we ze meerdere keren ophogen
        // houden we bij welke reeds zijn opgehoogd
        HashSet<int> alreadyCorrectedVertices = new HashSet<int>();
        
        // Begin met het loopen over de CityObjecten
        foreach (var cityObject in this._cityJson.CityObjects)
        {
            if (childMaaiveldDict.ContainsKey(cityObject.Key))
            {

                // Bereken de waarde die we bij alle z-waarden moeten doen om bij z=0 te komen
                decimal hCompensationValue = 0 - childMaaiveldDict[cityObject.Key].Value;
                // Schaal de waarde
                int scaledHeightCompensationValue = this.ScaleHeightMetersToCityJSON(hCompensationValue);

                // Loop over de geometry objecten van het huidige CityObject
                foreach (var geometry in cityObject.Value.geometry)
                {
                    // Ondersteunen alleen LOD22 en type Solid
                    // Meer informatie over de opbouw van solid arrays:
                    // - https://www.cityjson.org/dev/geom-arrays/#solid
                    // - https://www.cityjson.org/specs/1.1.0/#arrays-to-represent-boundaries
                    if (geometry.lod == "2.2" && geometry.type == "Solid")
                    {
                        foreach (var boundary in geometry.boundaries)
                        {
                            foreach (var shell in boundary)
                            {
                                foreach (var item in shell)
                                {
                                    foreach (var vertex in item)
                                    {
                                        // Update the vertex height
                                        if (alreadyCorrectedVertices.Contains(vertex) == false)
                                        {
                                            this.ModifyHeight(vertex, scaledHeightCompensationValue);
                                            alreadyCorrectedVertices.Add(vertex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Geometry found which is not LOD22 or type Solid \n" +
                                          "CityObject ID: " + cityObject.Key.ToString() + " \n" +
                                          "Geometry type: " + geometry.type.ToString() + " \n" + 
                                          "Geometry LOD: " + geometry.lod.ToString()); 
                    }
                }
            }

        }
    }

    /// <summary>
    /// Build a dictonary CityObject ID of childeren (key) and the h_maaiveld value (value)
    /// </summary>
    /// <returns>Dictonary containing the h_maaiveld value linked to the CityObject ID</returns>
    private Dictionary<string, decimal?> CollectMaaiveldValues()
    {
        Dictionary<string, decimal?> identificatieMaaiveldDict = new Dictionary<string, decimal?>();
        
        // Zijn er wel objecten?
        if (this._cityJson.CityObjects.Count == 0)
            return identificatieMaaiveldDict;

        foreach (var cityObject in this._cityJson.CityObjects)
        {   
            // Check if the object even has a attribute with h_maaiveld
            if (cityObject.Value.attributes.h_maaiveld != null)
            {
                foreach (string childIdentificatie in cityObject.Value.children)
                {
                    identificatieMaaiveldDict.Add(childIdentificatie, cityObject.Value.attributes.h_maaiveld);
                }
            }
        }
        return identificatieMaaiveldDict;
    }

    /// <summary>
    /// Add a (scaled) value to the height of a vertex.
    /// </summary>
    /// <param name="vertexIndex">The index of the vertex. </param>
    /// <param name="value">The scaled meters value to be added to the current value. </param>
    private void ModifyHeight(int vertexIndex, int value)
    {
        int currentValue = this._cityJson.vertices[vertexIndex][2];
        int newValue = currentValue + value;
        this._cityJson.vertices[vertexIndex][2] = newValue;
    }

    /// <summary>
    /// Scale a decimal height value in meters to a CityJSON value
    /// </summary>
    /// <remarks>
    /// CityJSON stores the coordinates of the vertices of the geometries as integer
    /// values to reduce file size.
    /// </remarks>
    /// <param name="value">The decimal value in meters to be scaled to a CityJSON value. </param>
    /// <returns>An integer representing the value in CityJSON. </returns>
    public int ScaleHeightMetersToCityJSON(decimal value)
    {
        return Decimal.ToInt32(Math.Round(value / this._cityJson.transform.scale[2], 0));
    }
}