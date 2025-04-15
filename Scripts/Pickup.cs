using Godot;
using System;

public partial class Pickup : Node2D
{
    [Export] Sprite2D pickupSprite;
    [Export] AudioStreamPlayer pickupNoise;
    [Export] AudioStreamPlayer fullNoise;


    private Player player;
    private Inventory.StackData itemData;

    public void SetItem(Inventory.StackData stackData)
    {
        itemData = stackData;
    }
    public void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            player = (Player)body;
        }
    }
    public void OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            player = null;
        }
    }

    public override void _Ready()
    {
        pickupSprite.Texture = itemData.RelatedRes.InventoryTex;
    }
    public override void _Process(double delta)
    {
        if (player != null && Input.IsActionJustPressed("interact"))
        {
            if (player.AddInventoryItem(itemData))
            {
                QueueFree();
                pickupNoise.Play();
            }
            else fullNoise.Play();
        }
    }
}
