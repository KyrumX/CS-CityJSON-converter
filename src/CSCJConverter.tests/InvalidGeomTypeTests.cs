using System;
using System.IO;
using Xunit;

namespace CSCJConverter.tests;

public class InvalidGeomFixture
{
    public CityJSON cityJson { get; private set; }
    
    public InvalidGeomFixture()
    {
        string testFile = @"testfixtures/test_invalid_shed.json";
        string jsonString = File.ReadAllText(testFile);
        this.cityJson = new CityJSON(jsonString, "");
    }
}

/// <summary>
/// Tests when a valid CityJSON file is loaded but with invalid geometry type (e.g. not Solid)
/// </summary>
public class InvalidGeomTypeTests : IClassFixture<InvalidGeomFixture>
{
    private readonly CityJSON cityJson;

    public InvalidGeomTypeTests(InvalidGeomFixture fixture)
    {
        this.cityJson = fixture.cityJson;
    }

    /// <summary>
    /// Test the function which modifies vertices when they are of an invalid type/lod.
    /// </summary>
    [Fact]
    public void TranslateHeightMaaiveld_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => this.cityJson.TranslateHeightMaaiveld());
    }
}