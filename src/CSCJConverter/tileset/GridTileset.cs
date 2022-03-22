namespace CSCJConverter.tileset;

public class GridTileset : AbstractTileset
{
    private List<Tile> _tiles;

    /// <summary>
    /// Constructor for a grid based Tileset.
    /// </summary>
    /// <param name="tilesetGeometricError">
    ///     The error, in meters, introduced if this tileset is not rendered. At runtime, the geometric error is used
    ///     to compute screen space error (SSE), i.e., the error measured in pixels. Default: 260
    /// </param>
    /// <param name="rootGeometricError">
    ///     Nonnegative number that defines the error, in meters. Is used at runtime to determine the SSE at which the
    ///     root tile's children are rendered. Default: 4.5398975185470771
    /// </param>
    /// <param name="tileGeometricError">
    ///     The error, in meters, introduced if this tile is rendered and its children are not. At runtime,
    ///     the geometric error is used to compute screen space error (SSE), i.e., the error measured in pixels.
    ///     Default: 2.3232
    /// </param>
    /// <param name="version">Default: 1.0</param>
    /// <param name="gltfUpAxis">Default: z</param>
    /// <param name="refineMethod">Default: REPLACE</param>
    /// <param name="structureType">Default: GRID</param>
    public GridTileset(
       decimal tilesetGeometricError = 260,
       decimal rootGeometricError = 4.5398975185470771m,
       decimal tileGeometricError = 2.3232m,
       string version = "1.0",
       string gltfUpAxis = "z",
       RefineMethods refineMethod = RefineMethods.REPLACE) : base(tilesetGeometricError, rootGeometricError, tileGeometricError, version, gltfUpAxis, refineMethod)
    {
        // Een grid is niets anders dan een lijst van tegels, een tegel heeft dan geen eigen kinder tegels:
        this._tiles = new List<Tile>();
    }

    public GridTileset(TilesetModel model, string version = "1.0") : base(model, version)
    {
        this._tiles = model.root.children.ToList();
    }

    /// <summary>
    /// Generate a TilesetModel, which can be serialized to json. Children will use a grid setup.
    /// </summary>
    /// <returns>TilesetModel, can be serialized to json for a tileset.json file.</returns>
    public override TilesetModel GenerateTileset()
    {
        var children = this._tiles;
        return this.BuildTileSet(children);
    }

    /// <summary>
    /// Add a tile to the tileset.
    /// </summary>
    /// <param name="geographicalExtent">An array representing the geographical extent: [minx, miny, minz, maxx, maxy, maxz]</param>
    /// <param name="uri">The content uri relative to the tileset file.</param>
    public override void AddTile(
        double[] geographicalExtent,
        string uri)
    {
        // Haal eerst de benodigde x, y en z waarden uit een CityJSON geographicalExtent:
        decimal tileMinX = (decimal)geographicalExtent[0];
        decimal tileMaxX = (decimal)geographicalExtent[3];
        decimal tileMinY = (decimal)geographicalExtent[1];
        decimal tileMaxY = (decimal)geographicalExtent[4];
        decimal tileMinZ = (decimal)geographicalExtent[2];
        decimal tileMaxZ = (decimal)geographicalExtent[5];
        decimal tileCenterX = this.CalculateCenter(tileMaxX, tileMinX);
        decimal tileCenterY = this.CalculateCenter(tileMaxY, tileMinY);
        decimal tileCenterZ = this.CalculateCenter(tileMaxZ, tileMinZ);
        decimal tileHalfX = this.CalculateHalfLength(tileMaxX, tileMinX);
        decimal tileHalfY = this.CalculateHalfLength(tileMaxY, tileMinY);
        decimal tileHalfZ = this.CalculateHalfLength(tileMaxZ, tileMinZ);

        // BoxVolume maken (https://github.com/CesiumGS/3d-tiles/tree/main/specification#box):
        BoxVolume boxVolume = new BoxVolume()
        {
            box = new decimal[12]
            {
                tileCenterX, tileCenterY, tileCenterZ,
                tileHalfX, 0, 0,
                0, tileHalfY, 0,
                0, 0, tileHalfZ
            }
        };

        Content content = new Content()
        {
            uri = uri
        };
        
        Tile tile = new Tile()
        {
            boundingVolume = boxVolume,
            content = content,
            geometricError = this._tileGeometricError
        };
        
        // Is een simpele grid: een tegel heeft geen kinder tegels
        this._tiles.Add(tile);

        // Update x, y en z --> zodat de root tegel (die alle tegels moet omvatten) de juiste x, y en z waarden heeft
        this._updateMinMaxX(tileMaxX, tileMinX);
        this._updateMinMaxY(tileMaxY, tileMinY);
        this._updateMinMaxZ(tileMaxZ, tileMinZ);
    }
}