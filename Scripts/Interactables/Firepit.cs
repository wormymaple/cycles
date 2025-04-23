using Godot;
using System;

/*
TO DOS
Prevent starting fire when already going
Damage player if on top?
Add extinguished noise
Fine tune stat boosts
*/
public partial class Firepit : Node2D
{
	[Export] Sprite2D[] sprites;
	[Export] AudioStreamPlayer burningSound;
	[Export] AudioStreamPlayer extinguishedSound;
	[Export] PointLight2D light;
	[Export] GpuParticles2D particles;
	[Export] ItemRes neededRes;
	[Export] double burnMaxTime;
	[Export] float regenRate;

	[ExportCategory("spawning logic")] 
	[Export] private float probValue;
	[Export] private int numSpawns;
	
	

	private Player player;
	private double burnTime;
	private bool isBurning;

	void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			player = (Player)body;
		}
	}
	void OnBodyExited(Node body)
	{

		if (body is Player)
		{
			player = null;
		}
	}
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
	public override void _Process(double delta)
	{
		if (player != null && Input.IsActionJustPressed("interact"))
		{
			if (player.ConsumeInventoryItem(neededRes, 7))
			{
				isBurning = true;
				ToggleFire(true);
			}
		}
		if (isBurning)
		{
			burnTime += delta;
			if (player != null)
			{
				player.IncrementStats(regenRate, ref player.health, player.maxHealth, (float)delta);
				player.IncrementStats(regenRate, ref player.temp, player.maxTemp, (float)delta);
			}

			if (burnTime > burnMaxTime)
			{
				burnTime = 0;
				isBurning = false;
				ToggleFire(false);
			}
		}
	}

}
