using Godot;
using System;
using System.Linq;

public partial class ShadowCreature : Enemy
{
    [Export] float moveSpeed;
    [Export] ItemRes[] scaredItems;
    [Export] float scaredDist;
    
    public override void _PhysicsProcess(double delta)
    {
        if (!playerInRange) return;

        Vector2 dirToPlayer = player.Position - GlobalPosition;

        float moveDir = 1f;
        if (player.EquippedItemData != null && scaredItems.Contains(player.EquippedItemData.RelatedRes)
            && dirToPlayer.Length() < scaredDist)
        {
            moveDir = -1f;
        }
        LinearVelocity = dirToPlayer.Normalized() * moveSpeed * moveDir;
    }
}
