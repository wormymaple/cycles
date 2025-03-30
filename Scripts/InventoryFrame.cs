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
		if (!@event.IsActionPressed("hit") || !hovering) return;

		inventory.ItemIconClicked(this);
	}

	void OnMouseEntered() {
		hovering = true;
	}

	void OnMouseExited() {
		hovering = false;
	}
}
