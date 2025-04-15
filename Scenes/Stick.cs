using Godot;
using System;

public partial class Stick : Node2D
{
	[Export] PackedScene stick;
	[Export] int stickCount;
	[Export] float spawnAreaSize;
	RandomNumberGenerator rng = new();
	
	public override void _Ready()
	{
		SpawnTrees();
	}

	void SpawnTrees()
	{
		for (int i = 0; i < stickCount; i += 1)
		{

			float x = rng.Randf() * spawnAreaSize;
			float y = rng.Randf() * spawnAreaSize;

			SpawnTree(new Vector2(x,y));
		}
	}

	void SpawnTree(Vector2 pos)
	{
		Inventory.StackData stackData = new(GD.Load<ItemRes>("res://CustomResources/ItemResources/stick.tres"), 5);
		Pickup newTree = stick.Instantiate() as Pickup;
		newTree.SetItem(stackData);
		AddChild(newTree);
		newTree.Position = pos;
	}

}
