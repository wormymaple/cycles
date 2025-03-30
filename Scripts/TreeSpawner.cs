using Godot;
using System;
using System.Threading.Tasks;

public partial class TreeSpawner : Node2D
{
	[Export] PackedScene tree;
	[Export] int treeCount;
	[Export] float spawnAreaSize;
	RandomNumberGenerator rng = new();
	
	public override void _Ready()
	{
		SpawnTrees();
	}

	void SpawnTrees()
	{
		for (int i = 0; i < treeCount; i += 1)
		{
			float x = rng.Randf() * spawnAreaSize;
			float y = rng.Randf() * spawnAreaSize;

			SpawnTree(new Vector2(x,y));
		}
	}

	void SpawnTree(Vector2 pos)
	{
		Node2D newTree = tree.Instantiate() as Node2D;
		AddChild(newTree);
		newTree.Position = pos;
		newTree.Scale *= new Vector2(Mathf.Sign(rng.Randf() - 0.5f), 1f);
	}
}
