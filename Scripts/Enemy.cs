using Godot;
using System;

public partial class Enemy : RigidBody2D
{
    [Export] float health;
    protected bool playerInRange;
    protected Player player;

    void OnBodyEntered(Node2D body)
    {
        if (!body.IsInGroup("Player")) return;
        
        playerInRange = true;
        player = body as Player;
    }
    
    void OnBodyExited(Node2D body)
    {
        if (body.IsInGroup("Player")) playerInRange = false;
    }
}
