using Godot;
using System;

public partial class Stick : Object
{
    public override void _Ready()
    {
        stackData = new(itemRes, 5);
    }
}
