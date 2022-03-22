using System.Collections.Generic;
using System.Linq;
using CSCJConverter.tileset;
using Xunit;

namespace CSCJConverter.tests;

public class GridTilesetTestAddTile : IClassFixture<GenerateFreshGridTilesetFixture>
{

    private readonly GenerateFreshGridTilesetFixture _tilsetFixture;
    
    public GridTilesetTestAddTile(GenerateFreshGridTilesetFixture tilsetFixture)
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
        IEnumerable<decimal> expectedBox = new decimal[12] {  2490.8535m, 1266.6625m, 29.8125m, 2490.8535m, 0, 0, 0,1266.6625m, 0, 0, 0, 30.8125m };
        IEnumerable<int> expectedTransform = new int[16] { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1 };
        decimal expectedRootError = 4.5398975185470771m;
        string expectedRefineMethod = "REPLACE";
        IEnumerable<decimal> expectedTileBox = new decimal[12] { 2795.215m, 958.4555m, 13.8475m, 316.03m, 0, 0, 0, 330.6365m, 0, 0, 0, 13.8475m };
        string expectedContentUri = this._tilsetFixture.contentUri6228;
        decimal expectedTileError = 2.3232m;
        
        // Act -- Voeg drie tegels toe, de volgorde maakt hier uit (voor de _updateMinMax{} functies)
        AbstractTileset tileset = new GridTileset();
        tileset.AddTile(_tilsetFixture.geographicalExtent6228, _tilsetFixture.contentUri6228);
        tileset.AddTile(_tilsetFixture.geographicalExtent3370, _tilsetFixture.contentUri3370);
        tileset.AddTile(_tilsetFixture.geographicalExtent1122, _tilsetFixture.contentUri1122);
        TilesetModel model = tileset.GenerateTileset();
        
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