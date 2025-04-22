using Godot;
using System;

/*
 Basic RNG spawner
 
 each subclass is responsible for spawning an array of different item types
 for example, the loot spawner spawns loot that the player can pickup
 the interactable spawner spawns things that the player can interact with (chest, fireplace)
 
 there are 3 arrays
 a collection of unique objects we want a chunk to possibly contain
 a collection of the max amount of spawns that a unique object can have in a single chunk
 a collection of the chances (float representing percentage out of 100%) for each unique object to actually spawn

we run through each unique object 
for each unique object we attempt to spawn it max amount of spawns times
for each spawn attempt we use its probability to spawn to check if we should spawn it
 */
public abstract partial class SpawnerBase : Node2D
{ 
    [Export] protected float spawnRadius;
    [Export] protected float[] probValues;
    [Export] protected int[] maxSpawnsPerChunk;
    
    
    protected RandomNumberGenerator rng = new();
    private void Spawn()
    {
        for (int i = 0; i < probValues.Length; i++) // probValues length should = the amount of items we are going to run spawns on
        {
            for (int j = 0; j < maxSpawnsPerChunk[i]; j++)
            {
                if (rng.RandfRange(0f, 1f) < probValues[i])
                {
                    float x = rng.RandfRange(-spawnRadius, spawnRadius);
                    float y = rng.RandfRange(-spawnRadius, spawnRadius);

                    Node2D newScene = CreateScene(i);
                    if (newScene != null)
                    {
                        newScene.Position = new Vector2(x, y);
                        newScene.ZIndex = 10;
                        AddChild(newScene);
                    }
                    else
                    {
                        GD.Print($"Null value returned instead of Scene in {Name} spawning logic");
                    }
                    

                }
            }
        }
    }

    protected abstract Node2D CreateScene(int objectIndex); // this is defined in sub classes to handle unique logic for creating a scene

    public override void _Ready()
    {
        if (probValues.Length != maxSpawnsPerChunk.Length)
        {
            GD.Print("probValues and maxSpawnsPerChunk are of different length: Check Export variable config");
        }
        Spawn();
    }
}
