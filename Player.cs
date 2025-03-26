using Godot;
using System;

public partial class Player : RigidBody2D
{
    [Export] float speed;
    
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        Move();    
    }

    void Move()
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        
        LinearVelocity = inputDir * speed;
    }
}
