using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public partial class MusicHandler : AudioStreamPlayer
{
    [Export] AudioStream[] tracks;
    [Export] float trackDelay;
    float delayTime;
    RandomNumberGenerator rng = new();

    public override void _Process(double delta)
    {
        if (Playing) return;

        if (delayTime < trackDelay) delayTime += (float)delta;
        else StartMusic();
    }
    
    void StartMusic()
    {
        if (tracks == null) return;
        
        List<AudioStream> possibleSelections = [..tracks];
        possibleSelections.Remove(Stream);

        Stream = possibleSelections[rng.RandiRange(0, possibleSelections.Count - 1)];
        delayTime = 0;
        
        Play();
    }
}
