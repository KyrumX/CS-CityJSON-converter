using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CSCJConverter.tests;

public class CityJSONFixture
{
    public CityJSON cityJson { get; private set; }
    
    public CityJSONFixture()
    {
        string testFile = @"testfixtures/test_shed.json";
        string jsonString = File.ReadAllText(testFile);
        this.cityJson = new CityJSON(jsonString, "testfixtures/test.json");
    }
}

public class MaaiveldConversionTests : IClassFixture<CityJSONFixture>
{
    private readonly CityJSON cityJson;
    
    public MaaiveldConversionTests(CityJSONFixture fixture)
    {
        this.cityJson = fixture.cityJson;
    }
    
    /// <summary>
    /// Test the function which converts the height in meters from a decimal value to an integer
    /// applying the scaling as defined in the CityJSON file
    /// </summary>
    [Fact]
    public void TestScaleHeightMetersToCityJSON()
    {
        const decimal inputValue = 12.3456789m;
        const int expectedValue = 12346;

        int result = this.cityJson.ScaleHeightMetersToCityJSON(inputValue);
        
        Assert.Equal(expectedValue, result);
    }

    /// <summary>
    /// Test the function which modifies vertices.
    /// It will use the h_maaiveld value, convert it to CityJSON value (by applying scale and translate)
    /// and then modify all vertices belonging to the building.
    /// </summary>
    [Fact]
    public void TestModifyHeight()
    {
        // Arrange
        List<int> expectedVertexZero = new List<int>() {-38707, 156058, 0};
        List<int> expectedVertexTwo = new List<int>() {-36549, 156471, 0};
        List<int> expectedVertexSeventeen = new List<int>() { -35747, 154248, 2371 };
        List<int> expectedVertexNineteen = new List<int>() {-35724, 154355, 2364};
        // Control vertex (index 20) doesn't belong to any building, thus shouldn't have been modified
        List<int> expectedVertexTwenty = new List<int>() {155380, 287061, 30234};
        
        // Act
        this.cityJson.TranslateHeightMaaiveld();
        
        // Assert
        Assert.Equal(expectedVertexZero, this.cityJson.CityJson.vertices[0]);
        Assert.Equal(expectedVertexTwo, this.cityJson.CityJson.vertices[2]);
        Assert.Equal(expectedVertexSeventeen, this.cityJson.CityJson.vertices[17]);
        Assert.Equal(expectedVertexNineteen, this.cityJson.CityJson.vertices[19]);
        Assert.Equal(expectedVertexTwenty, this.cityJson.CityJson.vertices[20]);
    }
}