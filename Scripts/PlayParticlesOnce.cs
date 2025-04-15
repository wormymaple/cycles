using Godot;
using System;

public partial class PlayParticlesOnce : GpuParticles2D
{
    public override void _Ready()
    {
        Emitting = true;
    }
}
