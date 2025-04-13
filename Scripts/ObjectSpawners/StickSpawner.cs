using Godot;
using System;


public partial class StickSpawner : Node2D
{
	[Export] PackedScene stick;
	[Export] int stickCount;
	[Export] float spawnAreaSize;
	RandomNumberGenerator rng = new();
	
	public override void _Ready()
	{
		SpawnSticks();
	}

	void SpawnSticks()
	{
		for (int i = 0; i < stickCount; i += 1)
		{
			float x = rng.Randf() * spawnAreaSize;
			float y = rng.Randf() * spawnAreaSize;

			SpawnStick(new Vector2(x,y));
		}
	}

	void SpawnStick(Vector2 pos)
	{
		Node2D newStick = stick.Instantiate() as Node2D;
		AddChild(newStick);
		newStick.Position = pos;
		newStick.Scale *= new Vector2(Mathf.Sign(rng.Randf() - 0.5f), 1f);
	}
}