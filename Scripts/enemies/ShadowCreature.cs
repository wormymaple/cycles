using Godot;
using System;
using System.Linq;

public partial class ShadowCreature : Enemy
{
    [Export] float moveSpeed;
    [Export] ItemRes[] scaredItems;
    [Export] float scaredDist;
    [Export] private Area2D lightDetectionRange;
    [Export] private AudioStreamPlayer2D attackSound;
    [Export] private AudioStreamPlayer2D detectSound;
  
    private bool inLight;
    
    
    private void CheckExistingOverlaps()
    {
        // Check all areas already overlapping with this one
        var overlappingAreas = lightDetectionRange.GetOverlappingAreas();
        inLight = false;
        foreach (var area in overlappingAreas)
        {
            if (area.GetParent() is PointLight2D light && light.Visible)
            {
                inLight = true;
                break;
            }
            
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!playerInRange) return;
        CheckExistingOverlaps();
        
        Vector2 dirToPlayer = player.Position - GlobalPosition;

        float moveDir = 1f;
        if (inLight)
        {
            moveDir = -1f;
        }

        if (playerInAttackRange && !inLight)
        {
            if(!attackSound.Playing) attackSound.Play();
            player.TakeDamage(damage * (float) delta);
            
        }

        LinearVelocity = dirToPlayer.Normalized() * moveSpeed * moveDir;
    }
}