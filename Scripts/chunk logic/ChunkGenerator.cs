using Godot;
using System.Collections.Generic;

public abstract partial class ChunkGenerator : TileMapLayer
{
    public abstract void GenerateChunk();
}
