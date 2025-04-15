using Godot;
using System;

public partial class DieAfterSeconds : Node2D
{
    [Export] float aliveTimeMax;
    float aliveTime;

    public override void _Process(double delta)
    {
        aliveTime += (float)delta;
        if (aliveTime >= aliveTimeMax) QueueFree();
    }
}
