using Godot;

public partial class InteractablesSpawner : SpawnerBase
{
    [Export] private PackedScene[] spawnableInteractables;
    
    protected override Node2D CreateScene(int objectIndex)
    {
        GD.Print(spawnableInteractables[objectIndex].GetName());
        Node2D newScene = spawnableInteractables[objectIndex].Instantiate() as Node2D;
        return newScene;
    }
}