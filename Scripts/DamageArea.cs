using Godot;
using System;
using System.Collections.Generic;
using Node2D = Godot.Node2D;

public partial class DamageArea : Area2D
{
    [Export] CollisionShape2D shape;
    float damage;
    float aliveTimeMax, aliveTime;
    List<Node2D> damagedNodes = [];
    PackedScene hitEffect;

    public override void _Process(double delta)
    {
        aliveTime += (float)delta;    
        if (aliveTime >= aliveTimeMax) QueueFree();
    }

    public void SetProperties(float _damage, float areaSize, float _aliveTime, PackedScene _hitEffect = null)
    {
        damage = _damage;
        aliveTimeMax = _aliveTime;
        ((CircleShape2D)shape.Shape).Radius = areaSize;
        hitEffect = _hitEffect;
    }

    void OnBodyEntered(Node2D body)
    {
        if (!body.IsInGroup("Enemy")) return;
        if (damagedNodes.Contains(body)) return;
        
        ((Enemy)body).TakeDamage(damage);
        damagedNodes.Add(body);

        if (hitEffect == null) return;
        
        Node2D newEffect = hitEffect.Instantiate() as Node2D;
        GetTree().GetRoot().AddChild(newEffect);
        newEffect.GlobalPosition = body.GlobalPosition;
    }
}
