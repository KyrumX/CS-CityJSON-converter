using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CSCJConverter.tileset;
using Xunit;

namespace CSCJConverter.tests;

public class AppendToGridTilesetTests : IClassFixture<AppendToGridTilesetFixture>
{
    private readonly AppendToGridTilesetFixture _tilsetFixture;
    
    public AppendToGridTilesetTests(AppendToGridTilesetFixture tilsetFixture)
    {
        this._tilsetFixture = tilsetFixture;
    }

    [Fact]
    public void TestAppendToTileset()
    {
        // Arrange
        string expectedVersion = "2.0";
        string expectedTilesetVersion = "appendedTilesetV2";
        string expectedGLTFUpAxis = "z";
        decimal expectedTilesetError = 260m;
        IEnumerable<decimal> expectedBox = new decimal[12] { 2490.8535m, 1267.0025m, 40.502m, 2490.8535m, 0, 0, 0, 1267.0025m, 0, 0, 0, 41.502m };
        IEnumerable<int> expectedTransform = new int[16] { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1 };
        decimal expectedRootError = 4.5398975185470771m;
        string expectedRefineMethod = "REPLACE";
        IEnumerable<decimal> expectedTileBox = new decimal[12] { 3103.069m, 1901.6975m, 41.002m, 648.136m, 0, 0, 0, 632.3075m, 0, 0, 0, 41.002m };
        string expectedContentUri = _tilsetFixture.contentUri3369;
        decimal expectedTileError = 2.3232m;
        
        // Act
        AbstractTileset tileset = new GridTileset(_tilsetFixture.model, version:"2.0", tilesetVersion:"appendedTilesetV2");
        tileset.AddTile(_tilsetFixture.geographicalExtent3369, _tilsetFixture.contentUri3369);
        TilesetModel model = tileset.GenerateTileset();
        
        // Assert
        Assert.Equal(expectedVersion, model.asset.version);
        Assert.Equal(expectedTilesetVersion, model.asset.tilesetVersion);
        Assert.Equal(expectedGLTFUpAxis, model.asset.gltfUpAxis);
        Assert.Equal(expectedTilesetError, model.geometricError);
        Assert.Equal(expectedBox, model.root.boundingVolume.box);
        Assert.Equal(expectedTransform, model.root.transform);
        Assert.Equal(expectedRootError, model.root.geometricError);
        Assert.Equal(expectedRefineMethod, model.root.refine);
        Assert.Equal(4, model.root.children.Count());
        var newlyAppendedTile = model.root.children.Last();
        Assert.Equal(expectedTileBox ,newlyAppendedTile.boundingVolume.box);
        Assert.Equal(expectedContentUri ,newlyAppendedTile.content.uri);
        Assert.Equal(expectedTileError, newlyAppendedTile.geometricError);
        Assert.Null(newlyAppendedTile.children);
    }

    [Fact]
    public void TestCountMethod()
    {
        // Arrange
        int expectedCount = 3;

        // Act
        AbstractTileset tileset = new GridTileset(_tilsetFixture.model);
        int res = tileset.CountTiles();

        // Assert
        Assert.Equal(expectedCount, res);
    }
}