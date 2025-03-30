using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ChunkLoader : Node2D
{
    public class Chunk
    {
        public string Biome;
        public ChunkGenerator ChunkScript;
        public Vector2I chunkPos;

        public Chunk(string _biome, Vector2I _chunkPos, ChunkGenerator _chunkScript)
        {
            Biome = _biome;
            ChunkScript = _chunkScript;
            chunkPos = _chunkPos;

            ChunkScript.GenerateChunk();
        }
    }
    
    [Export] Node2D updateTarget;
    [Export] PackedScene chunkScene;
    [Export] int chunkDist;
    [Export] float chunkSize;

    List<Chunk> loadedChunks = [];
    
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        Vector2I currentChunkPos = CurrentChunk();

        List<Chunk> chunksToKeep = [];
        List<Vector2I> chunksToLoad = [];
        for (int x = -chunkDist; x < chunkDist + 1; x += 1)
        {
            for (int y = -chunkDist; y < chunkDist + 1; y += 1)
            {
                Vector2I checkPos = new(x + currentChunkPos.X, y + currentChunkPos.Y);

                bool foundChunk = false;
                foreach (Chunk loadedChunk in loadedChunks)
                {
                    if (checkPos != loadedChunk.chunkPos) continue;
                    
                    chunksToKeep.Add(loadedChunk);
                    foundChunk = true;
                        
                    break;
                }
                
                if (!foundChunk) chunksToLoad.Add(checkPos);
            }
        }

        List<Chunk> chunksToUnload = [];
        foreach (Chunk loadedChunk in loadedChunks) 
            if (!chunksToKeep.Contains(loadedChunk)) chunksToUnload.Add(loadedChunk);
        foreach (Chunk chunk in chunksToUnload) UnloadChunk(chunk);

        foreach (Vector2I chunkPos in chunksToLoad) LoadChunk(chunkPos);
    }

    Vector2I CurrentChunk()
    {
        return new Vector2I(Mathf.FloorToInt(updateTarget.GlobalPosition.X / chunkSize),
            Mathf.FloorToInt(updateTarget.GlobalPosition.Y / chunkSize));
    }

    void LoadChunk(Vector2I chunkPos)
    {
        ChunkGenerator newChunkScene = chunkScene.Instantiate() as ChunkGenerator;
        AddChild(newChunkScene);
        newChunkScene.GlobalPosition = (Vector2)chunkPos * chunkSize;

        Chunk newChunk = new("null", chunkPos, newChunkScene);
        loadedChunks.Add(newChunk);
    }

    void UnloadChunk(Chunk chunk)
    {
        if (!loadedChunks.Contains(chunk))
            GD.PrintErr("Attempted to unload unloaded chunk! Pos: " + chunk.chunkPos);
        
        chunk.ChunkScript.QueueFree();
        loadedChunks.Remove(chunk);
    }
}
