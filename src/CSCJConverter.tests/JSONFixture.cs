using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace CSCJConverter.tests;

/// <summary>
/// Create a fixture for the deserialization of ou CityJSON
/// Since we need it for every test, we use this fixture class
/// </summary>
public class JSONFixture : IDisposable
{
    public CityJSON CityJson { get; private set; }
    private string outFile = @"testfixtures/outtest.json";
    public JSONFixture()
    {
        string testFile = @"testfixtures/test_shed.json";
        string jsonString = File.ReadAllText(testFile);
        CityJSON cityJson = new CityJSON(jsonString, outFile);
        this.CityJson = cityJson;
    }

    public void Dispose()
    {
        File.Delete(outFile);
    }
}

[CollectionDefinition("JSON Fixture")]
public class JSONFixtureCollection : ICollectionFixture<JSONFixture>
{
}