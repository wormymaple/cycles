using Godot;
using System;

public partial class Player : RigidBody2D
{
    [Export] private float speed;
    [Export] private int health;
    [Export] private float hunger;
    [Export] private float hungerRate;
    [Export] private int attackDamage;
    [Export] private Curve dashCurve;
    [Export] private float dashPower;
    [Export] private float dashTimeMax;
	[Export] Camera2D mainCamera;
	[Export] Sprite2D hand1, hand2;
	[Export] Vector2 handOffset;
	[Export] float handWiggleIntensity, handWiggleSpeed;
	float handWiggleT;
	[Export] float forceHandPerspCutoff;

    private Vector2 dashDir;
    private bool isDashing;
    private float dashTime;


    // helpers
    private Object RayCast(Vector2 start, Vector2 end)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(start, end);
        var result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            return result["collider"];
        }

        return null;
    }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
       
        hunger += (float)delta * hungerRate;
        
        if (isDashing)
        {
            dashTime += (float)delta;
            if (dashTime > dashTimeMax)
            {
                dashTime = dashTimeMax;
                isDashing = false;
            }

            float targetVel = dashCurve.Sample(dashTime / dashTimeMax) * dashPower;
            LinearVelocity = dashDir * targetVel;
        }
        else
        {
            Move((float)delta);
        }
		
		Animate();
    }
    
    void Animate()
    {
		Vector2 mousePos = mainCamera.GetGlobalMousePosition();
		Vector2 lookDir = (mousePos - GlobalPosition).Normalized();

		Vector2 perpDir = lookDir.Rotated(Mathf.Pi / 2);
		Vector2 handWiggle = Mathf.Sin(handWiggleT * handWiggleSpeed) * handWiggleIntensity * lookDir;
		hand1.Position = perpDir * handOffset + handWiggle;
		hand2.Position = -perpDir * handOffset - handWiggle;

		hand1.ZIndex = perpDir.Y < 0 ? -1 : 1;
		hand2.ZIndex = -perpDir.Y < 0 ? -1 : 1;

		if (Mathf.Abs(lookDir.X) < forceHandPerspCutoff)
		{
		int order = lookDir.Y < 0 ? -1 : 1;
		hand1.ZIndex = order;
		hand2.ZIndex = order;
      }
  	}

    public void _input(InputEvent @event)
    {
        if (@event.IsActionPressed("hit"))
        {
            GD.Print("hit");
        }

        if (@event.IsActionPressed("dash"))
        {
            isDashing = true;
            dashTime = 0;
            dashDir = GetLocalMousePosition().Normalized();
            GD.Print(dashDir);
        }

    }

    void Move()
    {		
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		handWiggleT += delta * inputDir.Length();
		LinearVelocity = inputDir * speed;
    }
}
