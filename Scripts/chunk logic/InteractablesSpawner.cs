using Godot;

public partial class InteractablesSpawner : Node2D
{
    [Export] private float spawnRadius;
    [Export] private PackedScene[] currChunkInteractables;
    private RandomNumberGenerator rng = new();
    
    private void SpawnItem(PackedScene currInteractable)
    {
        Node2D instance = currInteractable.Instantiate() as Node2D;
        int numSpawns = (int) instance.Get("numSpawns");
        float probValue = (float)instance.Get("probValue");
        instance.QueueFree();
        for (int i = 0; i < numSpawns; i++)
        {
            Node2D instanceToSpawn = currInteractable.Instantiate() as Node2D;
            if (rng.RandfRange(0f, 1f) < probValue)
            {
                float x = rng.RandfRange(0, spawnRadius);
                float y = rng.RandfRange(0, spawnRadius);
                AddChild(instanceToSpawn);
                instanceToSpawn.Position = new Vector2(x, y);
                
                
            }
            else
            {
                instanceToSpawn.QueueFree();
            }
        }
    }
    public override void _Ready()
    {
        for (int i = 0; i < currChunkInteractables.Length; i++)
        {
            SpawnItem(currChunkInteractables[i]);
        }
    }
}