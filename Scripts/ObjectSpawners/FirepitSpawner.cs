using Godot;
using System;

public partial class FirepitSpawner : Node2D
{
    [Export] PackedScene firepit;
	[Export] float spawnAreaSize;
	RandomNumberGenerator rng = new();
	
	public override void _Ready()
	{
        if(rng.RandiRange(1, 10) == 1){
            float x = rng.Randf() * spawnAreaSize;
			float y = rng.Randf() * spawnAreaSize;
            SpawnFirepit(new Vector2(x,y));
        } 
		
	}

	void SpawnFirepit(Vector2 pos)
	{
		Node2D newFirepit = firepit.Instantiate() as Node2D;
		AddChild(newFirepit);
		newFirepit.Position = pos;
		newFirepit.Scale *= new Vector2(Mathf.Sign(rng.Randf() - 0.5f), 1f);
	}
}
