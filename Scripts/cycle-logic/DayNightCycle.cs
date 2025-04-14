using Godot;
using System;

public partial class DayNightCycle : Node2D
{
    [Signal] public delegate void DayStartedEventHandler();
    [Signal] public delegate void NightStartedEventHandler();
    [Export] Timer timer;
    [Export] DirectionalLight2D worldLight;
    [Export] Curve energyCurve;
    bool isDay, isNight;
    public void Timeout(){

    }
    public override void _Ready()
    {
        worldLight.Energy = 1f;
        isDay = true;
        isNight = false;
    }
    public override void _Process(double delta)
    {
        float energy = energyCurve.Sample((float) (timer.TimeLeft / timer.WaitTime));
        worldLight.Energy = energy;
        if (energy < .3f && isDay) 
        {
            EmitSignal(SignalName.NightStarted);
            isDay = false;
            isNight = true;
        }
        else if (energy > .3f && isNight)
        {
            EmitSignal(SignalName.DayStarted);
            isDay = true;
            isNight = false;
        }
    }
}
