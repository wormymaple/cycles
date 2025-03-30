using Godot;
using System;

public partial class InventoryItem : TextureRect
{
	[Export] RichTextLabel stackCount;
	public Inventory.StackData ItemData;
	public InventoryFrame placedFrame;

	public void SetItem(Inventory.StackData itemData)
	{
		ItemData = itemData;
		Texture = itemData.RelatedRes.InventoryTex;
		stackCount.Text = itemData.StackCount > 1 ? itemData.StackCount.ToString() : "";
	}
}
