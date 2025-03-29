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
            Move();
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
        LinearVelocity = Input.GetVector("left", "right", "up", "down") * speed;
    }
}