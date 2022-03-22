using System.IO;
using System.Text.Json;
using CSCJConverter.tileset;

namespace CSCJConverter.tests;

public class GenerateFreshGridTilesetFixture
{
    public double[] geographicalExtent1122;
    public double[] geographicalExtent6228;
    public double[] geographicalExtent3370;
    public string contentUri1122;
    public string contentUri6228;
    public string contentUri3370;

    public GenerateFreshGridTilesetFixture()
    {
        string json1122 = File.ReadAllText(@"testfixtures/test_moved_1122.json");
        this.geographicalExtent1122 = JsonSerializer.Deserialize<CityJSONModel>(json1122).metadata.geographicalExtent;
        this.contentUri1122 = "1122.b3dm";
        string json6228 = File.ReadAllText(@"testfixtures/test_moved_6228.json");
        this.geographicalExtent6228 = JsonSerializer.Deserialize<CityJSONModel>(json6228).metadata.geographicalExtent;
        this.contentUri6228 = "6228.b3dm";
        string json3370 = File.ReadAllText(@"testfixtures/test_moved_3370.json");
        this.geographicalExtent3370 = JsonSerializer.Deserialize<CityJSONModel>(json3370).metadata.geographicalExtent;
        this.contentUri3370 = "3370.b3dm";
    }
}

public class AppendToGridTilesetFixture : GenerateFreshGridTilesetFixture
{
    public TilesetModel model;
    public double[] geographicalExtent3369;
    public string contentUri3369;
    
    public AppendToGridTilesetFixture() : base()
    {
        string json3369 = File.ReadAllText(@"testfixtures/test_moved_3369.json");
        geographicalExtent3369 = JsonSerializer.Deserialize<CityJSONModel>(json3369).metadata.geographicalExtent;
        contentUri3369 = "3369.b3dm";
        AbstractTileset tileset = new GridTileset();
        tileset.AddTile(geographicalExtent6228, contentUri6228);
        tileset.AddTile(geographicalExtent3370, contentUri3370);
        tileset.AddTile(geographicalExtent1122, contentUri1122);

        model = tileset.GenerateTileset();
    }
}