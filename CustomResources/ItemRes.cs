using Godot;
using System;

[GlobalClass]
public partial class ItemRes : Resource
{
	[Export] public string Name;
	[Export] public bool Stackable;
	[Export] public Texture2D InventoryTex;
	[Export] PackedScene EquippedScene;

	public ItemRes() : this("none", true, null, null) {}
	public ItemRes(string name, bool stackable, Texture2D inventoryTex, PackedScene equippedScene)
	{
		Name = name;
		Stackable = stackable;
		InventoryTex = inventoryTex;
		EquippedScene = equippedScene;
	}
}
