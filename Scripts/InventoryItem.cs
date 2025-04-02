using Godot;
using System;
using System.Globalization;

public partial class InventoryItem : TextureRect
{
	[Export] RichTextLabel itemLabel;
	public Inventory.StackData ItemData;
	Inventory inventory;

	bool hovering;

	public void SetItem(Inventory.StackData itemData, Inventory fromInventory)
	{
		ItemData = itemData;
		Texture = itemData.RelatedRes.InventoryTex;
		string correctedName = (char)(itemData.RelatedRes.Name[0] - 32) + itemData.RelatedRes.Name.Substring(1);
		itemLabel.Text = correctedName + (itemData.StackCount > 1 ? ": " + itemData.StackCount : "");

		inventory = fromInventory;
	}

	public override void _Input(InputEvent @event)
	{
		if (!@event.IsActionPressed("hit")) return;
		if (hovering) inventory.EquipItem(this);
	}

	void OnMouseEnter()
	{
		hovering = true;
	}

	void OnMouseExit()
	{
		hovering = false;
	}
}
