using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CSCJConverter.tileset;
using Xunit;

namespace CSCJConverter.tests;

public class GridTilsetFixture
{
    public AbstractTileset tileset;
    public double[] geographicalExtent1122;
    public double[] geographicalExtent6228;
    public double[] geographicalExtent3370;
    public string contentUri1122;
    public string contentUri6228;
    public string contentUri3370;

    public GridTilsetFixture()
    {
        this.tileset = new GridTileset();
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
public class GridTilesetTestAddTile : IClassFixture<GridTilsetFixture>
{

    private readonly GridTilsetFixture _tilsetFixture;
    
    public GridTilesetTestAddTile(GridTilsetFixture tilsetFixture)
    {
        this._tilsetFixture = tilsetFixture;
    }

    /// <summary>
    /// Test if a TilesetModel is generated correctly based on 3 tiles.
    /// </summary>
    [Fact]
    public void TestGeneratedModel()
    {
        // Arrange
        string expectedVersion = "1.0";
        string expectedGLTFUpAxis = "z";
        decimal expectedTilesetError = 260m;
        IEnumerable<decimal> expectedBox = new decimal[12] {  2490.8535m, 1266.6625m, 30.3125m, 2490.8535m, 0, 0, 0,1266.6625m, 0, 0, 0, 30.3125m };
        IEnumerable<int> expectedTransform = new int[16] { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1 };
        decimal expectedRootError = 4.5398975185470771m;
        string expectedRefineMethod = "REPLACE";
        IEnumerable<decimal> expectedTileBox = new decimal[12] { 2795.215m, 958.4555m, 13.8475m, 316.03m, 0, 0, 0, 330.6365m, 0, 0, 0, 13.8475m };
        string expectedContentUri = this._tilsetFixture.contentUri6228;
        decimal expectedTileError = 2.3232m;
        
        // Act
        _tilsetFixture.tileset.AddTile(_tilsetFixture.geographicalExtent6228, _tilsetFixture.contentUri6228);
        _tilsetFixture.tileset.AddTile(_tilsetFixture.geographicalExtent3370, _tilsetFixture.contentUri3370);
        _tilsetFixture.tileset.AddTile(_tilsetFixture.geographicalExtent1122, _tilsetFixture.contentUri1122);
        TilesetModel model = _tilsetFixture.tileset.GenerateTileset();
        
        // Assert
        Assert.Equal(expectedVersion, model.asset.version);
        Assert.Equal(expectedGLTFUpAxis, model.asset.gltfUpAxis);
        Assert.Equal(expectedTilesetError, model.geometricError);
        Assert.Equal(expectedBox, model.root.boundingVolume.box);
        Assert.Equal(expectedTransform, model.root.transform);
        Assert.Equal(expectedRootError, model.root.geometricError);
        Assert.Equal(expectedRefineMethod, model.root.refine);
        Assert.Equal(3, model.root.children.Count());
        var firstChild = model.root.children.First();
        Assert.Equal(expectedTileBox ,firstChild.boundingVolume.box);
        Assert.Equal(expectedContentUri ,firstChild.content.uri);
        Assert.Equal(expectedTileError, firstChild.geometricError);
        Assert.Null(firstChild.children);
    }
}