using Godot;
using System;

public partial class Pickup : Node2D
{
    [Export] Sprite2D pickupSprite;
    [Export] AudioStreamPlayer pickupNoise;
    [Export] AudioStreamPlayer fullNoise;


    private Player player;
    private Inventory.StackData itemData;
    private bool queueDelete = false;

    public void SetItem(Inventory.StackData stackData)
    {
        itemData = stackData;
        pickupSprite.Texture = itemData.RelatedRes.InventoryTex;
        pickupNoise.SetStream(itemData.RelatedRes.PickupNoise);
        fullNoise.SetStream(itemData.RelatedRes.FullInvNoise);
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


    public override void _Process(double delta)
    {
        if (queueDelete && !pickupNoise.Playing) QueueFree();
        
        if (player != null && Input.IsActionJustPressed("interact"))
        {
            if (player.AddInventoryItem(itemData))
            {
                pickupNoise.Play();
                pickupSprite.Visible = false;
                queueDelete = true;
            }
            else fullNoise.Play();
        }
    }
}
