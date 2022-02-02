using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace CSCJConverter.tests;

/// <summary>
/// Create a fixture for the deserialization of ou CityJSON
/// Since we need it for every test, we use this fixture class
/// </summary>
public class JSONFixture
{
    public CityJSONModel SerializedCJ { get; private set; }
    public JSONFixture()
    {
        string testFile = @"testfixtures/test_shed.json";
        string jsonString = File.ReadAllText(testFile);
        this.SerializedCJ = JsonSerializer.Deserialize<CityJSONModel>(jsonString);
    }
}

public class DeserializationTests : IClassFixture<JSONFixture>
{
    private readonly JSONFixture _fixture;

    public DeserializationTests(JSONFixture fixture)
    {
        this._fixture = fixture;
    }
    
    /// <summary>
    /// Test whether deserialization of 'type' was done correctly
    /// </summary>
    [Fact]
    public void GetType_InputIsCityJson_ReturnTrue()
    {
        const string expectedType = "CityJSON";

        Assert.Equal(expectedType, this._fixture.SerializedCJ.type);
    }
    
    /// <summary>
    /// Test whether deserialization of 'version' was done correctly
    /// </summary>
    [Fact]
    public void GetVersion_InputIsOneOne_ReturnTrue()
    {
        const string expectedVersion = "1.1";
        
        Assert.Equal(expectedVersion, this._fixture.SerializedCJ.version);
    }

    /// <summary>
    /// Test whether deserialization of the 'scale' of the x-axis was done correctly
    /// </summary>
    [Fact]
    public void GetScaleX_ReturnsSameValueAsFile()
    {
        const decimal expectedScaleX = 0.001m;
        
        Assert.Equal(expectedScaleX, this._fixture.SerializedCJ.transform.scale[0]);
    }
    
    /// <summary>
    /// Test whether deserialization of the 'scale' of the y-axis was done correctly
    /// </summary>
    [Fact]
    public void GetScaleY_ReturnsSameValueAsFile()
    {
        const decimal expectedScaleY = 0.001m;
        
        Assert.Equal(expectedScaleY, this._fixture.SerializedCJ.transform.scale[1]);
    }
    
    /// <summary>
    /// Test whether deserialization of the 'scale' of the z-axis was done correctly
    /// </summary>
    [Fact]
    public void GetScaleZ_ReturnsSameValueAsFile()
    {
        const decimal expectedScaleZ = 0.001m;
        
        Assert.Equal(expectedScaleZ, this._fixture.SerializedCJ.transform.scale[2]);
    }

    /// <summary>
    /// Test whether deserialization of the 'translate' of the x-axis was done correctly
    /// </summary>
    [Fact] public void GetTranslateX_ReturnsSameValueAsFile()
    {
        const decimal expectedTranslateX = 97854.99m;
        
        Assert.Equal(expectedTranslateX, this._fixture.SerializedCJ.transform.translate[0]);
    }
    
    /// <summary>
    /// Test whether deserialization of the 'translate' of the y-axis was done correctly
    /// </summary>
    [Fact]
    public void GetTranslateY_ReturnsSameValueAsFile()
    {
        const decimal expectedTranslateY = 438577.88m;
        
        Assert.Equal(expectedTranslateY, this._fixture.SerializedCJ.transform.translate[1]);
    }
    
    /// <summary>
    /// Test whether deserialization of the 'translate' of the z-axis was done correctly
    /// </summary>
    [Fact]
    public void GetTranslateZ_ReturnsSameValueAsFile()
    {
        const decimal expectedTranslateZ = 0.0m;
        
        Assert.Equal(expectedTranslateZ, this._fixture.SerializedCJ.transform.translate[2]);
    }
    
    /// <summary>
    /// Test whether deserialization of the parent 'CityObject' was done correctly
    /// </summary>
    [Fact]
    public void GetParentCityObject_ReturnsSameValuesAsFile()
    {
        // Arrange
        const string parentObjectID = "NL.IMBAG.Pand.0599100010014793";

        const decimal expectedMaaiveld = -5.083000183105469m;
        const string expectedIdentificatie = "NL.IMBAG.Pand.0599100010014793";
        List<string> expectedChildren = new List<string> { "NL.IMBAG.Pand.0599100010014793-0" };
        List<GeometrySolid> expectedEmptyGeomList = new List<GeometrySolid> { };
        const string expectedType = "Building";
        
        // Act
        CityObject cityObject = this._fixture.SerializedCJ.CityObjects[parentObjectID];
        
        // Asserts
        Assert.Equal(expectedMaaiveld, cityObject.attributes.h_maaiveld);
        Assert.Equal(expectedIdentificatie, cityObject.attributes.identificatie);
        Assert.Equal(expectedChildren, cityObject.children);
        Assert.Null(cityObject.parents);
        Assert.Equal(expectedEmptyGeomList, cityObject.geometry);
        Assert.Equal(expectedType, cityObject.type);
    }
    
    /// <summary>
    /// Test whether deserialization of the child 'CityObject' was done correctly
    /// </summary>
    [Fact]
    public void GetChildCityObject_ReturnsSameValuesAsFile()
    {
        // Arrange
        const string childObjectID = "NL.IMBAG.Pand.0599100010014793-0";
        
        List<string> expectedParents = new List<string> { "NL.IMBAG.Pand.0599100010014793" };
        const string expectedType = "BuildingPart";

        // Act
        CityObject cityObject = this._fixture.SerializedCJ.CityObjects[childObjectID];
        
        // Asserts
        Assert.Null(cityObject.attributes.h_maaiveld);
        Assert.Null(cityObject.attributes.identificatie);
        Assert.Null(cityObject.children);
        Assert.Equal(expectedParents, cityObject.parents);
        Assert.IsType<List<GeometrySolid>>(cityObject.geometry);
        Assert.Equal(expectedType, cityObject.type);
    }
    
    /// <summary>
    /// Test whether deserialization of the 'geometry' object on the child 'CityObject' was done correctly
    /// </summary>
    [Fact]
    public void GetChildCityObjectGeometry_ReturnsSameValuesAsFile()
    {
        // Arrange
        const string childObjectID = "NL.IMBAG.Pand.0599100010014793-0";

        const string expectedLod = "2.2";
        const string expectedType = "Solid";
        List<int> firstShapeList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> lastShapeList = new List<int>() { 12, 13, 14, 17, 18, 19, 10, 11, 15, 16 };
        
        // Act
        List<GeometrySolid> geometries = this._fixture.SerializedCJ.CityObjects[childObjectID].geometry;
        GeometrySolid geometry = geometries[0];
        
        // Asserts
        Assert.Single(geometries);
        Assert.Equal(expectedLod, geometry.lod);
        Assert.IsType<JsonObject>(geometry.semantics);
        Assert.Equal(expectedType, geometry.type);
        Assert.Single(geometry.boundaries);
        Assert.Equal(12, geometry.boundaries[0].Count);
        Assert.Single(geometry.boundaries[0][0]);
        Assert.Equal(10, geometry.boundaries[0][0][0].Count);
        Assert.Equal(firstShapeList, geometry.boundaries[0][0][0]);
        Assert.Equal(lastShapeList, geometry.boundaries[0][11][0]);
    }
}