using System.Text.Encodings.Web;
using System.Text.Json;

namespace CSCJConverter;

/// <summary>
/// Class to work with CityJSON objects
/// </summary>
public class CityJSON
{
    public CityJSONModel CityJson { get; private set; }
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
        this.CityJson = JsonSerializer.Deserialize<CityJSONModel>(jsonString);
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
        string serializeString = JsonSerializer.Serialize(this.CityJson, options);
        string outFileName = @_outputPath; 
        File.WriteAllText(outFileName, serializeString);
    }

    /// <summary>
    /// Moves the z-axes (height) of CityJSON objects to z=0 by using the h_maaiveld value
    /// </summary>
    /// <returns>
    ///     Integer representing the amount of vertices updated. Can be used to check if
    ///     all vertices have been updated. Returns null if CityObjects was empty.
    /// </returns>
    public int? TranslateHeightMaaiveld()
    {
        
        // Controleer of er objecten zijn:
        if (this.CityJson.CityObjects.Count == 0)
            return null;

        // Hoekpunten kunnen worden hergebruikt, om te voorkomen dat we ze meerdere keren ophogen
        // houden we bij welke reeds zijn opgehoogd
        HashSet<int> alreadyCorrectedVertices = new HashSet<int>();
        
        // Begin met het loopen over de CityObjecten
        foreach (var cityObject in this.CityJson.CityObjects)
        {
            // Is dit een 'ouder' object?
            if (cityObject.Value.attributes.h_maaiveld != null)
            {
                // Bereken de waarde die we bij alle z-waarden moeten doen om bij z=0 te komen
                decimal hCompensationValue = (decimal)(0 - cityObject.Value.attributes.h_maaiveld);
                // Schaal de waarde
                int scaledHeightCompensationValue = this.HeightMetersToCityJSON(hCompensationValue);
                
                // Loop over de 'kind' objecten heen
                foreach (string childIdentificatie in cityObject.Value.children)
                {
                    // Loop over de geometry objecten van het 'kind' CityObject
                    foreach (var geometry in this.CityJson.CityObjects[childIdentificatie].geometry)
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
                            throw new NotSupportedException("Geometry with type not equal to " +
                                                            "Solid or LOD not equal to 2.2 not " +
                                                            "supported." +
                                                            "Type: " + geometry.type + " " +
                                                            "LOD: " + geometry.lod + " " +
                                                            "Object key: " + cityObject.Key);
                        }
                    }
                }
            }
        }
        Console.WriteLine("Finished!");
        Console.WriteLine(alreadyCorrectedVertices.Count);
        Console.WriteLine(this.CityJson.vertices.Count);
        return alreadyCorrectedVertices.Count;
    }

    /// <summary>
    /// Build a dictonary CityObject ID of childeren (key) and the h_maaiveld value (value)
    /// </summary>
    /// <remarks>
    /// This function is no longer used.
    /// </remarks>
    /// <returns>Dictonary containing the h_maaiveld value linked to the CityObject ID</returns>
    private Dictionary<string, decimal?> CollectMaaiveldValues()
    {
        Dictionary<string, decimal?> identificatieMaaiveldDict = new Dictionary<string, decimal?>();
        
        // Zijn er wel objecten?
        if (this.CityJson.CityObjects.Count == 0)
            return identificatieMaaiveldDict;

        foreach (var cityObject in this.CityJson.CityObjects)
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
        int currentValue = this.CityJson.vertices[vertexIndex][2];
        int newValue = currentValue + value;
        this.CityJson.vertices[vertexIndex][2] = newValue;
    }

    /// <summary>
    /// Converts a height (in meters, decimal) for the z-axis to a valid CityJSON value (int).
    /// </summary>
    /// <remarks>
    /// To reduce size of CityJSON file coordinates are represented as integers. A 'scale' and 'transform' is applied
    /// to all coordinates. To get the real z-value (height) of a vertex:
    /// v[2] = (vi[2] * ["transform"]["scale"][2]) + ["transform"]["translate"][2] ;
    /// This function does the reverse: convert a real value in meters to a CityJSON integer value.
    /// Refer to the official documentation for more information: <see href="https://www.cityjson.org/specs/1.1.0/#transform-object"/>
    /// </remarks>
    /// <param name="metersValue">
    ///     The value in meters represented as a decimal to be converted to a CityJSON value, an integer.
    /// </param>
    /// <returns>
    ///     The height represented as a CityJSON integer (translated and scaled).
    /// </returns>
    public int HeightMetersToCityJSON(decimal metersValue)
    {
        // De formule moet omgedraaid worden: we willen van echt naar CityJSON.
        // Dus eerst 'translaten' we de waarde:
        decimal translatedHeight = this.TranslateHeightMetersToCityJSON(metersValue);

        // Daarna schalen:
        decimal scaledTranslatedHeight = this.ScaleHeightMetersToCityJSON(translatedHeight);

        // CityJSON verwacht het als int, dus afronden en naar int omzetten:
        return Decimal.ToInt32(Math.Round(scaledTranslatedHeight, 0));
    }
    
    /// <summary>
    /// Translates a decimal height using the CityJSON height translate factor.
    /// </summary>
    /// <param name="heightMeters">A decimal height value in meters.</param>
    /// <returns>Decimal representing a translated height in meters.</returns>
    private decimal TranslateHeightMetersToCityJSON(decimal heightMeters)
    {
        // height (z-axes) = (vi[2] * ["transform"]["scale"][2]) + ["transform"]["translate"][2]
        // This function converts a scaled int to a translated CityJSON value (2nd part of the formula)
        return (heightMeters - this.CityJson.transform.translate[2]);
    }

    /// <summary>
    /// Scales a translated decimal height using the CityJSON height scale factor.
    /// </summary>
    /// <param name="translatedMeters">Translated decimal height value.</param>
    /// <returns>Decimal representing a scaled height in meters.</returns>
    private decimal ScaleHeightMetersToCityJSON(decimal translatedMeters)
    {
        return (translatedMeters / this.CityJson.transform.scale[2]);
    }
}