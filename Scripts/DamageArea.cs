using Godot;
using System;
using System.Collections.Generic;

public partial class DamageArea : Area2D
{
    [Export] CollisionShape2D shape;
    float damage;
    float aliveTimeMax, aliveTime;
    List<Node2D> damagedNodes = [];

    public override void _Process(double delta)
    {
        aliveTime += (float)delta;
        if (aliveTime >= aliveTimeMax) QueueFree();
    }

    public void SetProperties(float _damage, float areaSize, float _aliveTime)
    {
        damage = _damage;
        aliveTimeMax = _aliveTime;
        ((CircleShape2D)shape.Shape).Radius = areaSize;
    }

    void OnBodyEntered(Node2D body)
    {
        if (!body.IsInGroup("Enemy")) return;
        if (damagedNodes.Contains(body)) return;
        
        ((Enemy)body).TakeDamage(damage);
        damagedNodes.Add(body);
    }
}
