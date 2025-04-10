using Godot;
using System;

public partial class Stick : Node2D
{
    [Export] ItemRes itemRes;
    private Player player;

    private Inventory.StackData stackData;
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

    public override void _Ready()
    {
        stackData = new(itemRes, 5);
    }
    public override void _Process(double delta)
    {
        if (player != null && Input.IsActionJustPressed("interact"))
        {
            player.AddInventoryItem(stackData);
            QueueFree();
        }
    }
}
