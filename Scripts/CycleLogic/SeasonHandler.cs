using Godot;
using System;

public partial class SeasonHandler : Node2D
{
	public enum Season
	{
		Winter,
		Spring,
		Summer,
		Fall
	}

	[Export] Season season;
	[Export] float seasonTimeMax;
}
