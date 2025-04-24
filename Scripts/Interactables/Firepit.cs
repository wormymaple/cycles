using Godot;
using System;

/*
TO DOS
Prevent starting fire when already going
Damage player if on top?
Add extinguished noise
Fine tune stat boosts
*/
public partial class Firepit : InteractableBase
{
   
    [Export] Sprite2D[] sprites;
    [Export] AudioStreamPlayer burningSound;
    [Export] AudioStreamPlayer extinguishedSound;
    [Export] PointLight2D light;
    [Export] GpuParticles2D particles;

    [Export] ItemRes neededResource;
    [Export] double burnMaxTime;
    [Export] float regenRate;
    [Export] private int neededPlayerResources;

    
    private double burnTime;
    private bool isBurning;

    private void ToggleFire(bool state)
    {
        sprites[0].Visible = state;
        sprites[1].Visible = !state;
        light.Visible = state;
        particles.Visible = state;
        burningSound.Playing = state;
    }

    public override void _Ready()
    {
        ToggleFire(false);
        burnTime = 0;
    }

    private bool ConsumePlayerResources()
    {
        if (!player.ConsumeInventoryItem(neededResource, neededPlayerResources)) return false;
        return true;
    }

    protected override void startInteraction()
    {
        if (!ConsumePlayerResources()) return;
        
        isBurning = true;
        ToggleFire(true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!isBurning) return;

        burnTime += delta;
        if (burnTime > burnMaxTime)
        {
            burnTime = 0;
            isBurning = false;
            ToggleFire(false);
        }

        if (player == null) return;
        
        player.IncrementStats(regenRate, ref player.health, player.maxHealth, (float)delta);
        player.IncrementStats(regenRate, ref player.temp, player.maxTemp, (float)delta);
    }
}