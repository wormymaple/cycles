using Godot;
using System.Collections.Generic;

public partial class TreeBuilder : Node2D
{
	[Export] PackedScene branchNode, leafNode;
	[Export] float segmentHeight, segmentVarHeight, branchWidth, minBranchWidth, segmentWidthReduction, segmentColorShift;
	[Export] float splitChance, endChance;
	[Export] int minHeight, maxBranchShiftLayer;
	[Export] float leafVariation;
	[Export] GpuParticles2D fallingLeaves;
	RandomNumberGenerator rng = new();
	List<Vector2> leafPositions = [];
	
	public override void _Ready()
	{
		List<Vector2> branchPoints = [Vector2.Zero];
		List<Vector2> newBranchPoints = [];

		int layer = 0;
		while (branchPoints.Count > 0)
		{
			foreach (Vector2 branchPoint in branchPoints)
			{
				List<Vector2> branchEndpoints = [];
				float branchBehaviour = rng.Randf();
				float continueBehavior = rng.Randf();
				float totalSplitChance = splitChance * layer;
				float totalEndChance = endChance * ((layer * layer) + Mathf.Abs(branchPoint.X) / branchWidth);
				
				branchEndpoints.Add(placeSegment(branchPoint, layer));
				if (branchBehaviour < totalSplitChance) branchEndpoints.Add(placeSegment(branchPoint, layer, true));
				if (continueBehavior > totalEndChance || layer < minHeight) newBranchPoints.AddRange(branchEndpoints);
				else
				{
					foreach (Vector2 endPoint in branchEndpoints) SpawnLeaf(endPoint);
				}
			}

			branchPoints.Clear();
			branchPoints.AddRange(newBranchPoints);
			newBranchPoints.Clear();

			layer += 1;
		}

		if (leafPositions.Count == 0) return;
		
		Vector2 leafPosSum = Vector2.Zero;
		foreach (Vector2 pos in leafPositions) leafPosSum += pos;
		Vector2 leafCenter = leafPosSum / leafPositions.Count;

		float furthestLeaf = 0f;
		foreach (Vector2 pos in leafPositions)
		{
			float leafDist = leafCenter.DistanceTo(pos);
			if (leafDist > furthestLeaf) furthestLeaf = leafDist;
		}
		
		fallingLeaves.Position = leafCenter;
		fallingLeaves.ProcessMaterial.Set("emission_sphere_radius", furthestLeaf);
	}

	Vector2 placeSegment(Vector2 branchPoint, int layer, bool branchSideways = false)
	{
		Line2D newBranch = branchNode.Instantiate() as Line2D;
		AddChild(newBranch);

		newBranch.Position = branchPoint;
		Vector2 endPoint = new(branchSideways ? branchWidth * rng.RandfRange(-1f, 1f) : 0f, -segmentHeight - segmentVarHeight * rng.Randf());
		newBranch.SetPointPosition(1, endPoint);
		newBranch.Width -= layer * segmentWidthReduction;
		if (newBranch.Width < minBranchWidth) newBranch.Width = minBranchWidth;
		
		newBranch.DefaultColor = newBranch.DefaultColor.Lightened(segmentColorShift * (layer < maxBranchShiftLayer ? layer : maxBranchShiftLayer));

		return branchPoint + endPoint;
	}

	void SpawnLeaf(Vector2 point)
	{
		Sprite2D newLeaf = leafNode.Instantiate() as Sprite2D;
		AddChild(newLeaf);

		newLeaf.Position = point;
		newLeaf.Rotation = rng.Randf() * 360f;
		newLeaf.Modulate = newLeaf.Modulate.Darkened(leafVariation * rng.Randf());
		
		leafPositions.Add(point);
	}
}