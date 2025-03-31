using Godot;
using System;

public partial class InventoryFrame : TextureRect
{
	public Inventory inventory;
	public InventoryItem currentItem;
	public int inventoryIndex;

	bool hovering;

	public override void _Input(InputEvent @event)
	{
		if (!hovering) return;

		if (@event.IsActionPressed("hit"))
		{
			inventory.ItemIconClicked(this);
		}
		else if (@event.IsActionPressed("right_click"))
		{
			inventory.EquipItem(this);
		}
	}

	void OnMouseEntered() {
		hovering = true;
	}

	void OnMouseExited() {
		hovering = false;
	}
}
