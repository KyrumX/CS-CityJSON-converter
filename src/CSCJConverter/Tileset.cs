using CSCJConverter.structures;

namespace CSCJConverter;

public class Tileset
{
    private decimal? _maxX;
    private decimal? _maxY;
    private decimal? _maxZ;
    private decimal? _minX;
    private decimal? _minY;
    private decimal? _minZ;

    private string _version;
    private string _gltfUpAxis;

    private decimal _tilesetGeometricError;
    private decimal _rootGeometricError;
    private decimal _tileGeometricError;

    public enum RefineMethods
    {
        ADD,
        REPLACE
    }

    private string _rootRefineMethod;
    
    public enum StructureTypes
    {
        GRID,    
    }
    private IStructure _structureGenerator;

    private List<Tile> _tiles;

    /// <summary>
    /// Constructor for the Tileset class used to build a tileset.
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
    public Tileset(decimal tilesetGeometricError = 260,
        decimal rootGeometricError = 4.5398975185470771m,
        decimal tileGeometricError = 2.3232m,
        string version = "1.0",
        string gltfUpAxis = "z",
        RefineMethods refineMethod = RefineMethods.REPLACE,
        StructureTypes structureType = StructureTypes.GRID)
    {
        this._version = version;
        this._gltfUpAxis = gltfUpAxis;
        this._rootRefineMethod = refineMethod == RefineMethods.ADD ? "ADD" : "REPLACE";
        this._tilesetGeometricError = tilesetGeometricError;
        this._rootGeometricError = rootGeometricError;
        this._tileGeometricError = tileGeometricError;

        if (structureType == StructureTypes.GRID)
        {
            this._structureGenerator = new GridStructure();
        }

        this._tiles = new List<Tile>();
    }

    /// <summary>
    /// Generate a TilesetModel which can be serialized.
    /// </summary>
    /// <returns>TilesetModel used for serialization.</returns>
    public TilesetModel GenerateTileSet()
    {
        if (!_tiles.Any()) return null;

        Asset asset = new Asset()
        {
            version = this._version,
            gltfUpAxis = this._gltfUpAxis

        };
        
        decimal rootCenterX = this.CalculateCenter((decimal)_maxX, (decimal)_minX);
        decimal rootCenterY = this.CalculateCenter((decimal)_maxY, (decimal)_minY);
        decimal rootCenterZ = this.CalculateCenter((decimal)_maxZ, (decimal)_minZ);
        decimal rootHalfX = this.CalculateHalfLength((decimal)_maxX, (decimal)_minX);
        decimal rootHalfY = this.CalculateHalfLength((decimal)_maxY, (decimal)_minY);
        decimal rootHalfZ = this.CalculateHalfLength((decimal)_maxZ, (decimal)_minZ);
        BoxVolume rootBox = new BoxVolume()
        {
            box = new decimal[12]
            {
                rootCenterX, rootCenterY, rootCenterZ,
                rootHalfX, 0, 0,
                0, rootHalfY, 0,
                0, 0, rootHalfZ
            }
        };

        var children = this._structureGenerator.generateStructure(this._tiles);

        Root root = new Root()
        {
            boundingVolume = rootBox,
            transform = new int[16]
            {
                1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1
            },
            children = children,
            geometricError = this._rootGeometricError,
            refine = this._rootRefineMethod
        };
        
        return new TilesetModel()
        {
            asset = asset,
            root = root,
            geometricError = _tilesetGeometricError
        };
    }
    
    /// <summary>
    /// Add a tile to the tileset.
    /// </summary>
    /// <param name="geographicalExtent">An array representing the geographical extent: [minx, miny, minz, maxx, maxy, maxz]</param>
    /// <param name="uri">The content uri relative to the tileset file.</param>
    public void AddTile(
        double[] geographicalExtent,
        string uri)
    {
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
        Console.WriteLine(tileCenterX);
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
        
        this._tiles.Add(tile);

        // Ensure the max and min values are set, if needed
        this._updateMinMaxX(tileMaxX, tileMinX);
        this._updateMinMaxY(tileMaxY, tileMinY);
        this._updateMinMaxZ(tileMaxZ, tileMinZ);
    }

    /// <summary>
    /// Update (if required) the max / min x coords of our tileset.
    /// </summary>
    /// <param name="tileMaxX">Tile max x.</param>
    /// <param name="tileMinX">Tile min x.</param>
    private void _updateMinMaxX(decimal tileMaxX, decimal tileMinX)
    {
        if (this._maxX == null) this._maxX = tileMaxX;
        else if (tileMaxX > this._maxX) this._maxX = tileMaxX;
                
        if (this._minX == null) this._minX = tileMinX;
        else if (tileMinX < this._minX) this._minX = tileMinX;
    }
    
    /// <summary>
    /// Update (if required) the max / min y coords of our tileset.
    /// </summary>
    /// <param name="tileMaxY">Tile max y.</param>
    /// <param name="tileMinY">Tile min y.</param>
    private void _updateMinMaxY(decimal tileMaxY, decimal tileMinY)
    {
        if (this._maxY == null) this._maxY = tileMaxY;
        else if (tileMaxY > this._maxY) this._maxY = tileMaxY;
        if (this._minY == null) this._minY = tileMinY;
        else if (tileMinY < this._minY) this._minY = tileMinY;
        
    }
    
    /// <summary>
    /// Update (if required) the max / min z coords of our tileset.
    /// </summary>
    /// <param name="tileMaxZ">Tile max z.</param>
    /// <param name="tileMinZ">Tile min z.</param>
    private void _updateMinMaxZ(decimal tileMaxZ, decimal tileMinZ)
    {
        if (this._maxZ == null) this._maxZ = tileMaxZ;
        else if (tileMaxZ > this._maxZ) this._maxZ = tileMaxZ;
        if (this._minZ == null) this._minZ = tileMinZ;
        else if (tileMinZ < this._minZ) this._minZ = tileMinZ;
    }

    /// <summary>
    /// Calculate the center based on two coords.
    /// </summary>
    /// <param name="maxCoordPoint">The max coord.</param>
    /// <param name="minCoordPoint">The min coord.</param>
    /// <returns>The center of both coords as decimal.</returns>
    private decimal CalculateCenter(decimal maxCoordPoint, decimal minCoordPoint)
    {
        Console.WriteLine(this.CalculateHalfLength(maxCoordPoint, minCoordPoint));
        return this.CalculateHalfLength(maxCoordPoint, minCoordPoint) + minCoordPoint;
    }

    /// <summary>
    /// Calculate a half value of two coords.
    /// </summary>
    /// <param name="maxCoordPoint">The max coord.</param>
    /// <param name="minCoordPoint">The min coord.</param>
    /// <returns>The half value of two coords, as decimal.</returns>
    public decimal CalculateHalfLength(decimal maxCoordPoint, decimal minCoordPoint)
    {
        return (maxCoordPoint - minCoordPoint) / 2;
    }
    
    
}