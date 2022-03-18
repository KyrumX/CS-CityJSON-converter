// See https://aka.ms/new-console-template for more information

using System.Text.Json;

namespace CSCJConverter.console;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Reading file... \n");
        
        // string filePath = @"";
        // string fileName = "";
        // string outPath = @"";
        // string outFile = "";
        // string jsonString = File.ReadAllText(filePath + fileName);
        //
        // CityJSON cj = new CityJSON(jsonString, outPath + outFile);
        // cj.TranslateHeightMaaiveld();
        // cj.Serialize();
        Tileset tileset = new Tileset();
        
        string json1122 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_1122_nullbound.json");
        double[] geo1122 = JsonSerializer.Deserialize<CityJSONModel>(json1122).metadata.geographicalExtent;
        tileset.AddTile(geo1122, "1122zup.b3dm");
        
        string json3369 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_3369_nullbound.json");
        double[] geo3369 = JsonSerializer.Deserialize<CityJSONModel>(json3369).metadata.geographicalExtent;
        tileset.AddTile(geo3369, "3369zup.b3dm");
        
        string json3370 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_3370_nullbound.json");
        double[] geo3370 = JsonSerializer.Deserialize<CityJSONModel>(json3370).metadata.geographicalExtent;
        tileset.AddTile(geo3370, "3370zup.b3dm");
        
        string json6227 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_6227_nullbound.json");
        double[] geo6227 = JsonSerializer.Deserialize<CityJSONModel>(json6227).metadata.geographicalExtent;
        tileset.AddTile(geo6227, "6227zup.b3dm");
        
        string json6228 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_6228_nullbound.json");
        double[] geo6228 = JsonSerializer.Deserialize<CityJSONModel>(json6228).metadata.geographicalExtent;
        tileset.AddTile(geo6228, "6228zup.b3dm");
        
        string json6229 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_6229_nullbound.json");
        double[] geo6229 = JsonSerializer.Deserialize<CityJSONModel>(json6229).metadata.geographicalExtent;
        tileset.AddTile(geo6229, "6229zup.b3dm");
        
        string json6230 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_6230_nullbound.json");
        double[] geo6230 = JsonSerializer.Deserialize<CityJSONModel>(json6230).metadata.geographicalExtent;
        tileset.AddTile(geo6230, "6230zup.b3dm");
        
        string json3371 =
            File.ReadAllText(
                @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\3dbag_translated_3371_nullbound.json");
        double[] geo3371 = JsonSerializer.Deserialize<CityJSONModel>(json3371).metadata.geographicalExtent;
        tileset.AddTile(geo3371, "3371zup.b3dm");

        TilesetModel model = tileset.GenerateTileSet();
        
        string serializeString = JsonSerializer.Serialize<TilesetModel>(model); 
        File.WriteAllText(@"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CS-CityJSON-converter\test.json", serializeString);
        Console.WriteLine();

    }

}