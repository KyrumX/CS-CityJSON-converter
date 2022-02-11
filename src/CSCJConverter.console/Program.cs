// See https://aka.ms/new-console-template for more information

namespace CSCJConverter.console;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Reading file... \n");
        
        string filePath = @"";
        string fileName = "";
        string outPath = @"";
        string outFile = "";
        string jsonString = File.ReadAllText(filePath + fileName);
        
        CityJSON cj = new CityJSON(jsonString, outPath + outFile);
        cj.TranslateHeightMaaiveld();
        cj.Serialize();

        // var root = JsonSerializer.Deserialize<CityJSONModel>(jsonString);
        //
        // // TEMP ALLOW '+' TO GO THROUGH --TEST IF THIS IS NEEDED FOR CJIO
        // var options = new JsonSerializerOptions
        // {
        //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        // };
        //
        // string serializeString = JsonSerializer.Serialize(root, options);
        // string outFileName = "test.json"; 
        // File.WriteAllText(filePath + outFileName, serializeString);
    }

}