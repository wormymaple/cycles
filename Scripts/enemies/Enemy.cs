using Godot;
using System;

public partial class Enemy : RigidBody2D
{
    [Export] protected float health, damage;
    [Export] PackedScene deathEffect;
    protected bool playerInRange;
    protected Player player;
    protected bool playerInAttackRange;

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

    void DamageAreaEntered(Node2D body)
    {
        if (body is Player) playerInAttackRange = true;
    }

    void DamageAreaExited(Node2D body)
    {
        if (body is Player) playerInAttackRange = false;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0f) Die();
    }

    void Die()
    {
        Node2D newEffect = deathEffect.Instantiate() as Node2D; 
        GetParent().AddChild(newEffect);
        newEffect.GlobalPosition = GlobalPosition;

        // ADD ITEM DROP
        
        QueueFree();
    }

    public override void _Ready()
    {
        playerInAttackRange = false;
    }
    
    
}
