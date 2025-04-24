using Godot;
using System;

public partial class DayNightCycle : Node2D
{
    public enum DayState
    {
        Daytime,
        Nighttime
    }
    
    [Signal] public delegate void DayStartedEventHandler();
    [Signal] public delegate void NightStartedEventHandler();
    [Export] Timer timer;
    [Export] DirectionalLight2D worldLight;
    [Export] Curve energyCurve;
    [Export] CurveXyzTexture colorCurve;
    [Export] float cutoffEnergy;
    bool isDay = true, isNight;
    public static DayState State = DayState.Daytime;
    public void Timeout()
    {

    }
    public override void _Ready()
    {
        worldLight.Energy = 1f;
        isDay = true;
        isNight = false;
    }
    
    public override void _Process(double delta)
    {   
        float curveInput = (float) (timer.TimeLeft/ timer.WaitTime);
        float energy = energyCurve.Sample(curveInput);

        float red = colorCurve.CurveX.Sample(curveInput);
        float green = colorCurve.CurveY.Sample(curveInput);
        float blue = colorCurve.CurveZ.Sample(curveInput);

        worldLight.Energy = energy;
        worldLight.Color = new Color(red, green, blue);
        
        if (energy < cutoffEnergy && isDay)
        {
            State = DayState.Nighttime;
            EmitSignal(SignalName.NightStarted);
            isDay = false;
            isNight = true;
        }
        else if (energy > cutoffEnergy && isNight)
        {
            State = DayState.Daytime;
            EmitSignal(SignalName.DayStarted);
            isDay = true;
            isNight = false;
        }
    }
}
