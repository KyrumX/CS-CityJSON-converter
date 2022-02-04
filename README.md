# CS-CityJSON-converter ![Package (test, build)](https://github.com/KyrumX/CS-CityJSON-converter/actions/workflows/workflow.yml/badge.svg)

  C# applicatie voor het bewerken van CityJSON 1.1 3D BAG bestanden.
  
  **Functionaliteit**
  - Aanpassen van hoekpunten aan de hand van maaiveldhoogte.

# Gebruik

```cs
string filePath = @"C:\path\to\cityjson\";
string inFile = "samplefile.json";
string jsonString = File.ReadAllText(filePath + inFile);
        
CityJSON cj = new CityJSON(jsonString, filePath + "myOutputFile.json");
cj.TranslateHeightMaaiveld();
cj.Serialize();
```

## Opmerkingen

Huidige versie doet geen controles. Bijvoorbeeld checken of alle hoekpunten zijn bijgewerkt of dat er een valide bestand wordt ingeladen.
