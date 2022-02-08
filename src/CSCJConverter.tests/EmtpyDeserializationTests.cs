using System.IO;
using Xunit;

namespace CSCJConverter.tests;

public class EmptyCityObjectsFixture
{
    public CityJSON cityJson { get; private set; }
    
    public EmptyCityObjectsFixture()
    {
        string testFile = @"testfixtures/test_empty_objects.json";
        string jsonString = File.ReadAllText(testFile);
        this.cityJson = new CityJSON(jsonString, "");
    }
}

/// <summary>
/// Tests when a valid CityJSON file is loaded but with no CityObjects
/// </summary>
public class EmtpyDeserializationTests : IClassFixture<EmptyCityObjectsFixture>
{
    private readonly CityJSON cityJson;

    public EmtpyDeserializationTests(EmptyCityObjectsFixture fixture)
    {
        this.cityJson = fixture.cityJson;
    }

    /// <summary>
    /// Test the function which modifies vertices when there are no CityObjects.
    /// Should return Null.
    /// </summary>
    [Fact]
    public void TranslateHeightMaaiveld_ReturnsNull()
    {
        var res = this.cityJson.TranslateHeightMaaiveld();
        
        Assert.Null(res);
    }
}