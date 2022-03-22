# CS-CityJSON-converter [![dotnet package](https://github.com/KyrumX/CS-CityJSON-converter/actions/workflows/workflow.yml/badge.svg)](https://github.com/KyrumX/CS-CityJSON-converter/actions/workflows/workflow.yml)

  C# applicatie voor het bewerken van CityJSON 1.1 3D BAG bestanden.
  
  **Functionaliteit**
  - Aanpassen van hoekpunten aan de hand van maaiveldhoogte (vereist h_maaiveld attribuut op ouder object!);
  - Genereren van een GRID tegelset JSON bestand op basis van CityJSON 1.1;
  - Toevoegen van tegels (CityJSON 1.1) aan een GRID tegelset JSON bestand.

# Gebruik

## Maaiveld

```cs
string filePath = @"C:\path\to\cityjson\";
string inFile = "samplefile.json";
string jsonString = File.ReadAllText(filePath + inFile);
        
CityJSON cj = new CityJSON(jsonString, filePath + "myOutputFile.json");
cj.TranslateHeightMaaiveld();
cj.Serialize();
```

### Opmerking

Huidige versie doet geen controles. Bijvoorbeeld checken of alle hoekpunten zijn bijgewerkt of dat er een valide bestand wordt ingeladen.

## Tegelset genereren

```cs
GridTileset tileset = new GridTileset();
        
string cj1 = File.ReadAllText(@"C:\path\to\cityjson\cj1.json");
double[] cj1geo = JsonSerializer.Deserialize<CityJSONModel>(cj1).metadata.geographicalExtent;
tileset.AddTile(cj1, "cj1.b3dm");

string cj2 = File.ReadAllText(@"C:\path\to\cityjson\cj2.json");
double[] cj2geo = JsonSerializer.Deserialize<CityJSONModel>(cj2).metadata.geographicalExtent;
tileset.AddTile(cj1, "cj2.b3dm");

TilesetModel model = tileset.GenerateTileset();
        
string serializeString = JsonSerializer.Serialize<TilesetModel>(model); 
File.WriteAllText(@"C:\output\dir\tileset.json", serializeString);
```

## CityJSON toevoegen aan bestaand tegelset bestand

```cs
string tilesetString = File.ReadAllText(@"C:\output\dir\tileset.json");
TilesetModel tilesetModel = JsonSerializer.Deserialize<TilesetModel>(tilesetString);
GridTileset gridTileset = new GridTileset(tilesetModel);

string cj3 = File.ReadAllText(@"C:\path\to\cityjson\cj3.json");
double[] cj3geo = JsonSerializer.Deserialize<CityJSONModel>(cj3).metadata.geographicalExtent;
tileset.AddTile(cj1, "cj3.b3dm");

TilesetModel newModel = tileset.GenerateTileset();
        
string serializeString = JsonSerializer.Serialize<TilesetModel>(newModel); 
File.WriteAllText(@"C:\output\dir\newtileset.json", serializeString);
```

# Tegelset ondersteuning

Onderstaande tabellen laten zien met welke eigenschappenen onze tegelset generator kan werken. Eigenschappen welke niet in het overzicht staan zullen worden verwijderd bij het inlezen van een bestaand tegelset bestand.

## Tileset

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**asset**|`object`|Metadata over de gehele tegelset.|:white_check_mark: Ja|
|**geometricError**|`nummer`|De fout in meters, geïntroduceerd wanneer de tegelset niet getekend wordt. Tijdens runtime wordt de geometrische fout gebruikt om de screen space error (SSE) te berekenen, ofwel de fout uitgedrukt in pixels.|:white_check_mark: Ja|
|**root**|`object`|De hoofd tegel van de tegelset.| :white_check_mark: Ja|

## Asset

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**version**|`string`|De versie van de 3D Tiles specificatie.|:white_check_mark: Ja|
|**tilesetVersion**|`string`|De applicatie-specifieke versie van het tegelset bestand.|Nee|
|**gltfUpAxis**|`string`|Welke as de hoogte vertegenwoordigt in de glTF modellen.|Nee|

## Root Tile

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**boundingVolume**|`string`|Een volume waarin de content van de tegel valt. Enkel ondersteuning voor een BoxVolume.|:white_check_mark: Ja|
|**geometricError**|`nummer`|De fout, in meters, geïntroduceerd als deze tegel wordt weergegeven en zijn kinderen niet. Tijdens runtime wordt de geometrische fout gebruikt om de screen space error (SSE) te berekenen, ofwel de fout uitgedrukt in pixels.|:white_check_mark: Ja|
|**refine**|`string`|Geeft aan of additive of replacement verfijning wordt gebruikt bij het doorlopen van de tegelset. Gebruikt voor het weergeven van tegel content.|Nee, wel vereist voor de root tegel|
|**transform**|`nummer[16]`|Een 4x4 transformatiematrix, opgeslagen in kolom-hoofdvolgorde, die de inhoud van de tegel transformeert. Zie specificatie.|Nee, standaard identitiy matrix|
|**content**|`object`|Metadata van de tegel en een link naar de content.|Nee|
|**children**|`tile[]`|Lijst van kinder tegels.|Nee|

## Tile (kinderen)

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**boundingVolume**|`string`|Een volume waarin de content van de tegel valt. Enkel ondersteuning voor een BOX.|:white_check_mark: Ja|
|**geometricError**|`nummer`|De fout, in meters, geïntroduceerd als deze tegel wordt weergegeven en zijn kinderen niet. Tijdens runtime wordt de geometrische fout gebruikt om de screen space error (SSE) te berekenen, ofwel de fout uitgedrukt in pixels.|:white_check_mark: Ja|
|**content**|`object`|Metadata van de tegel en een link naar de content.|Nee|
|**children**|`tile[]`|Lijst van kinder tegels.|Nee|

## Content

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**uri**|`string`|Een uri welke verwijst naar de content van de tegel.|:white_check_mark: Ja|

## Bouding volume

|   |Type|Beschrijving|Vereist volgens officiële specificatie|
|---|----|-----------|--------|
|**box**|`nummer[12]`|Een array van 12 nummers. De eerste drie elementen beschrijven de x, y en z waarden voor het center van de box. De volgende drie elementen (met indices 3, 4, en 5) beschrijven de x-as richting en een halve lengte. De volgende drie elementen (met indices 6, 7, en 8) beschrijven de y-as richting en een halve lengte. De laatste drie elementen (met indices 9, 10 en 11) beschrijven de z-as richting en een halve lengte.|Nee|
