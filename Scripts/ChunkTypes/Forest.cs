using Godot;
using System;

public partial class Forest : ChunkGenerator
{
    [Export] int chunkSize;
    [Export] int biomeSetSource;
    RandomNumberGenerator rng = new();
    
    public override void GenerateChunk()
    {   
        TileSetSource tileSetSource = GetTileSet().GetSource(biomeSetSource);
        int biomeTileCount = tileSetSource.GetTilesCount();
        Vector2I[] tiles = new Vector2I[biomeTileCount];
        for (int i = 0; i < biomeTileCount; i += 1)
            tiles[i] = tileSetSource.GetTileId(i);
        
        Clear();

        for (int x = 0; x < chunkSize; x += 1)
        {
            for (int y = 0; y < chunkSize; y += 1)
            {
                Vector2I tile = new(x, y);
                SetCell(tile, biomeSetSource, tiles[rng.RandiRange(0, biomeTileCount - 1)]);
            }
        }
    }
}
