using Godot;
using System;

public partial class StatsContainer : GridContainer
{
    [Export] Player player;
    [Export] ColorRect healthBar;
    [Export] ColorRect hungerBar;
    [Export] ColorRect tempBar;

    [Export] float healthBarSize;
 
    [Export] float hungerBarSize;
    [Export] float tempBarSize;
    
    public override void _Process(double delta)
    {
        float healthPerc = player.GetHealthPerc();
        float hungerPerc = player.GetHungerPerc();
        float tempPerc = player.GetTempPerc();
        
        healthBar.Size = new Vector2 (healthBarSize * healthPerc, healthBar.Size.Y);
        hungerBar.Size = new Vector2 (hungerBarSize * hungerPerc, hungerBar.Size.Y);
        tempBar.Size = new Vector2 (tempBarSize * tempPerc, tempBar.Size.Y);
    }
}
