using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    [Export] Vector2 spawningRange;
    [Export] int spawnCountAtNight;
    [Export] PackedScene enemy;
    RandomNumberGenerator rng = new();
    

    void SpawnOnNight()
    {
        for (int i = 0; i < spawnCountAtNight; i += 1)
        {
            Vector2 spawnPos = GlobalPosition +
                               new Vector2(rng.RandfRange(0f, spawningRange.X), rng.RandfRange(0f, spawningRange.Y));
            Node2D newEnemy = enemy.Instantiate() as Node2D;
            AddChild(newEnemy);
            newEnemy.GlobalPosition = spawnPos;
        }
    }
}