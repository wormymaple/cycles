using Godot;
using System;

public partial class Object : Node2D
{
    [Export] protected ItemRes itemRes;
    protected Player player;

    protected Inventory.StackData stackData;
    public void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            player = (Player)body;
        }
    }

    public void OnBodyExited(Node body)
    {
        if (body is Player)
        {
            player = null;
        }
    }

    public override void _Process(double delta)
    {
        if (player != null && Input.IsActionJustPressed("interact"))
        {
            if(player.AddInventoryItem(stackData)) QueueFree();
        }
    }
}
