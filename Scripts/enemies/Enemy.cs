using Godot;
using System;

public partial class Enemy : RigidBody2D
{
    [Export] float health, damage;
    [Export] PackedScene deathEffect;
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0f) Die();
    }

    void Die()
    {
        GD.Print(deathEffect);
        Node2D newEffect = deathEffect.Instantiate() as Node2D; 
        GetParent().AddChild(newEffect);
        newEffect.GlobalPosition = GlobalPosition;

        // ADD ITEM DROP
        
        QueueFree();
    }
    
    void BodyCollision(Node body)
    {
        if (!body.IsInGroup("Player")) return;
        
        ((Player)body).TakeDamage(damage);
    }
}
