using Godot;

public partial class LootSpawner : SpawnerBase
{
    [Export] private PackedScene templateScene;
    [Export] private ItemRes[] spawnableLoot;
    [Export] private int[] spawnableLootQtys;

    protected override Node2D CreateScene(int objectIndex)
    {
        Pickup newScene = (Pickup) templateScene.Instantiate();
        Inventory.StackData packagedData = new(
            spawnableLoot[objectIndex], rng.RandiRange(1, spawnableLootQtys[objectIndex]));
            newScene.SetItem(packagedData);
        return newScene;
    }
}