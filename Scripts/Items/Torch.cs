using Godot;
using System;

public partial class Torch : Sprite2D, Item
{
    [Export] PackedScene damageArea;
    [Export] float damage, damageAreaDist, damageAreaSize, damageAreaTime;
    [Export] PackedScene hitEffect;
    
    public void Use(Vector2 attackDir)
    {
        DamageArea newDamageArea = damageArea.Instantiate() as DamageArea;
        GetTree().GetRoot().AddChild(newDamageArea);
        
        newDamageArea.SetProperties(damage, damageAreaSize, damageAreaTime, hitEffect);
        newDamageArea.GlobalPosition = GlobalPosition + attackDir * damageAreaDist;
    }
}
