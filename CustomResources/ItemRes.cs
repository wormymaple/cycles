using Godot;
using System;

[GlobalClass]
public partial class ItemRes : Resource
{
	[Export] public string Name;
	[Export] public bool Stackable, Equippable;
	[Export] public Texture2D InventoryTex;
	[Export] public PackedScene EquippedScene;

	public ItemRes() : this("none", true, false, null, null) {}
	public ItemRes(string name, bool stackable, bool equippable, Texture2D inventoryTex, PackedScene equippedScene)
	{
		Name = name;
		Stackable = stackable;
		Equippable = equippable;
		InventoryTex = inventoryTex;
		EquippedScene = equippedScene;
	}
}
