using Godot;
using System;

public partial class Chest : InteractableBase
{
    [Export] private ItemRes[] itemDrops;
    [Export] private PackedScene templateScene;
    [Export] private Sprite2D[] sprites;
    private RandomNumberGenerator rng = new();

    protected override void startInteraction()
    {
        interactable = false;
        sprites[1].Visible = true;
        var droppedItem = itemDrops[rng.RandiRange(0, itemDrops.Length-1)];
        Pickup newScene = (Pickup) templateScene.Instantiate();
        Inventory.StackData packagedData = new(
            droppedItem, 1);
        newScene.SetItem(packagedData);
        newScene.Position = new Vector2(0, 20);
        AddChild(newScene);
        

    }
    
}
