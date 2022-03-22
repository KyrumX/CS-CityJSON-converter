namespace CSCJConverter.tileset;

public interface ITileset
{
   public TilesetModel GenerateTileset();
   public void AddTile(double[] geographicalExtent, string uri);
}