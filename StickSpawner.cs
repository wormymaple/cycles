using Godot;
using System;

public partial class StickSpawner : Node2D
{
    [Export] int numSticks;
    RandomNumberGenerator rng;
    public override void _Ready(){
        rng = new();
        for(int i = 0; i<numSticks; i++)
        {
            Inventory.StackData stackData = new(GD.Load<ItemRes>("res://CustomResources/ItemResources/stick.tres"), 5);
            PackedScene stick = GD.Load<PackedScene>("res://Scenes/Items/Pickup.tscn");
            
            Pickup pickup = stick.Instantiate() as Pickup;
            pickup.SetItem(stackData);
            float x = rng.Randf() * 256f;
            float y = rng.Randf() * 256f;
            pickup.Position = new Vector2 (x, y);
            AddChild(pickup);

        }
    }
}
