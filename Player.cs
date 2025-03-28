using Godot;
using System;

public partial class Player : RigidBody2D
{
	[Export] float speed;
	
	// helpers
	private Object RayCast(Vector2 start, Vector2 end){
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(start, end);
		var result = spaceState.IntersectRay(query);
		if (result.Count > 0){
			return result["collider"];
		}

		return null;
	}
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		Move();
	}
	public void _input(InputEvent @event){
		if (@event.IsActionPressed("hit"))
		{
			GD.Print("hit");
		}
	}

	void Move()
	{
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		LinearVelocity = inputDir * speed;
	}
}
