using Godot;

public partial class ItemSpawner : Node2D
{
    [Export] private float spawnRadius;
    [Export] private PackedScene templateScene;
    [Export] private ItemRes[] currChunkItems;
    private RandomNumberGenerator rng = new();

    private Pickup CreateItemScene(ItemRes itemData)
    {
        Pickup genScene = (Pickup) templateScene.Instantiate();
        Inventory.StackData packagedData = new(
            itemData, itemData.NumsPerDrop[rng.Randi() % itemData.NumsPerDrop.Length]);
        genScene.SetItem(packagedData);
        return genScene;
    }
    private void SpawnItem(ItemRes currItem)
    {
        for (int i = 0; i < currItem.numSpawns; i++)
        {
            if (rng.RandfRange(0f, 1f) < currItem.probValue)
            {
                float x = rng.RandfRange(0, spawnRadius);
                float y = rng.RandfRange(0, spawnRadius);

                Pickup newItemScene = CreateItemScene(currItem);
                AddChild(newItemScene);
                newItemScene.Position = new Vector2(x, y);
                
                
            }
        }
    }
    public override void _Ready()
    {
        for (int i = 0; i < currChunkItems.Length; i++)
        {
            GD.Print($"Item {currChunkItems[i].Name}");
            SpawnItem(currChunkItems[i]);
        }
    }
}