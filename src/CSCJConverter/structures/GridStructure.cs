namespace CSCJConverter.structures;

public class GridStructure : IStructure
{
    // In a grid all tiles are equal, meaning a tile doesn't have children
    public List<Tile> generateStructure(List<Tile> tiles)
    {
        return tiles;
    }
}