using Godot;
using System;

public partial class CameraController : Camera2D
{
    [Export] Node2D target;
    [Export] float speed;
    [Export] float zoomMoveSpeed;
    [Export] public float targetZoom;
    Vector2 offset;

    public override void _Process(double delta)
    {
        Vector2 dir = target.GlobalPosition + offset - GlobalPosition;
        GlobalPosition += dir * (speed * (float)delta);

        Zoom += (targetZoom - Zoom.X) * Vector2.One * ((float)delta * zoomMoveSpeed);
    }

    public void SetFocusOffset(Vector2 newOffset) => offset = newOffset;
}
