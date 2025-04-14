using Godot;
using System;

public partial class Firepit : Node2D
{
	[Export] Sprite2D[] sprites;
	[Export] PointLight2D light;
	[Export] GpuParticles2D particles;
	[Export] ItemRes neededRes;
	[Export] double burnMaxTime;
	[Export] float regenRate;

	private Player player;
	private double burnTime;
	private bool isBurning;

	void OnBodyEntered(Node body)
	{
		GD.Print("entered-firepit");
		if (body is Player)
		{
			GD.Print("entered-firepit");
			player = (Player)body;
		}
	}
	void OnBodyExited(Node body)
	{

		if (body is Player)
		{
			GD.Print("exited-firepit");
			player = null;
		}
	}
	private void ToggleFire(bool state)
	{
		sprites[0].Visible = state;
		sprites[1].Visible = !state;
		light.Visible = state;
		particles.Visible = state;
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
			GD.Print("togglefire");
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
