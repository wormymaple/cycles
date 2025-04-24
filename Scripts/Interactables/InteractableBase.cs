using Godot;
using System;

public abstract partial class InteractableBase : Node2D
{
    protected Player player;
    protected bool interactable = true;

    private void OnBodyEntered(Node2D body)
    {
       
        if (body is Player)
        {
            player = (Player) body;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Player) player = null;
    }

    protected abstract void startInteraction();
    public override void _Process(double delta)
    {
        if (player != null && Input.IsActionJustPressed(("interact")) && interactable)
        {
            GD.Print("y");
            startInteraction();
        }
    }
}
