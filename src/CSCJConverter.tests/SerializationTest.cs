using Xunit;

namespace CSCJConverter.tests;

[Collection("JSON Fixture")]
public class SerializationTest
{
    private readonly CityJSON _cityJson;

    public SerializationTest(JSONFixture fixture)
    {
        this._cityJson = fixture.CityJson;
    }
    
    [Fact]
    public void Test_Serialize()
    {
        // Act
        this._cityJson.Serialize();
        
        FileAssert.AreEqual(@"testfixtures/test_shed_clean.json", @"testfixtures/outtest.json");
    }
}