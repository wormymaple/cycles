using Godot;
using System;

public partial class InteractionRange : Area2D
{
    [Export] private Sprite2D sprite;
    [Export] private Texture2D openSprite, closedSprite;
    private bool playerInRange;

    public override void _Ready()
    {
        openSprite.ResourcePath = "res://assets/chest_opened.tres";
        closedSprite.ResourcePath = "res://assets/chest_closed.tres";
    }

    private void ChangeTexture()
    {
        sprite.Texture = playerInRange ? openSprite : closedSprite;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            playerInRange = true;
            sprite.Texture = openSprite;
        }
    }

    public void OnBodyExited(Node body)
    {
        if (body is Player)
        {
            playerInRange = false;
            sprite.Texture = closedSprite;
        }
    }
}