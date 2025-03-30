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

    [Export] private Sprite2D torch;

    float wiggleT;
    [Export] private float wiggleSpeed, wiggleIntensity;
    [Export] float forcePerspCutoff;

    [Export] Sprite2D hand1, hand2;
    [Export] Vector2 handOffset;
    [Export] private Vector2 handVerticalShift;


    [Export] private Sprite2D eye1, eye2;
    [Export] private Vector2 eyeOffset;
    [Export] private Vector2 eyeVerticalShift;


    [Export] private Sprite2D foot1, foot2;
    [Export] private Vector2 footOffset;
    [Export] private Vector2 footVerticalShift;


    [Export] private Sprite2D mouth;
    [Export] private Vector2 mouthOffset;
    [Export] private Vector2 mouthVerticalShift;


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

    void AnimateBodyPart(Sprite2D bodyP1, Vector2 lookDir, Vector2 perpDir, Vector2 wiggle, Vector2 offset,
        Vector2 verticalShift)
    {
        if (LinearVelocity == Vector2.Zero) wiggle = Vector2.Zero;
        bodyP1.Position = perpDir * offset + wiggle + verticalShift;
        bodyP1.FlipH = lookDir.X > 0;
        bodyP1.ZIndex = perpDir.Y < 0 ? -1 : 1;

        if (Mathf.Abs(lookDir.X) < forcePerspCutoff)
        {
            int order = lookDir.Y < 0 ? -1 : 1;
            bodyP1.ZIndex = order;
        }
    }

    void Animate()
    {
        Vector2 lookDir = GetLocalMousePosition().Normalized();
        Vector2 perpDir = lookDir.Rotated(Mathf.Pi / 2);
        Vector2 wiggle = Mathf.Sin(wiggleT * wiggleSpeed) * wiggleIntensity * lookDir;
        
        AnimateBodyPart(hand1, lookDir, perpDir, wiggle, handOffset, handVerticalShift);
        AnimateBodyPart(hand2, lookDir, -perpDir, -wiggle, handOffset, handVerticalShift);
        
        AnimateBodyPart(eye1, lookDir, perpDir, wiggle, eyeOffset, eyeVerticalShift);
        AnimateBodyPart(eye2, lookDir, -perpDir, -wiggle, eyeOffset, eyeVerticalShift);
        
        AnimateBodyPart(foot1, lookDir, perpDir, wiggle, footOffset, footVerticalShift);
        AnimateBodyPart(foot2, lookDir, -perpDir, -wiggle, footOffset, footVerticalShift);
        
        AnimateBodyPart(torch, lookDir, -perpDir, -wiggle, handOffset, handVerticalShift);


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

    void Move(float delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        wiggleT += delta * inputDir.Length();
        LinearVelocity = inputDir * speed;
    }
}