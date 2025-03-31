using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Control
{
	public class StackData(ItemRes relatedRes, int stackCount)
	{
		public ItemRes RelatedRes = relatedRes;
		public int StackCount = relatedRes.Stackable ? stackCount : 1;
	}

	[Export] Player player;

	[ExportCategory("References")] 
	[Export] PackedScene frame;
	[Export] PackedScene itemIcon, equippedItemIcon;
	[Export] CameraController cameraController;
	[Export] ColorRect inventoryBackground;
	
	[ExportCategory("Inventory Settings")]
	[Export] Vector2I frameLayout;
	[Export] float frameSpacing;
	[Export] Vector2 inventoryOrigin, itemIconOffset, equippedIconOffset;
	[Export] Vector2 inventoryCameraOffset;
	
	[ExportCategory("Item Info")]
	[Export] ItemRes[] items;
	
	bool open;
	InventoryItem liftedItem;
	Control spawnedEquippedIcon;

	public override void _Ready()
	{
		if (frameLayout.X * frameLayout.Y != player.inventorySize)
			GD.PrintErr("INVENTORY LAYOUT AND PLAYER INVENTORY SIZE DO NOT MATCH");
		
		// TORCH TESTING - REMOVE LATER
		StackData testStackData = new(items[0], 0);
		player.AddInventoryItem(testStackData);
		// END
	}

	public override void _Process(double delta)
	{
		if (liftedItem != null) MoveLiftedItem();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("inventory")) ToggleInventory();
	}

	void ToggleInventory()
	{
		open = !open;

		if (open) OpenInventory();
		else CloseInventory();

		inventoryBackground.Visible = !inventoryBackground.Visible;
	}

	void OpenInventory()
	{
		cameraController.SetFocusOffset(inventoryCameraOffset);
		inventoryBackground.Position = new Vector2(inventoryOrigin.X + inventoryBackground.Size.X - 30f, 0f);
		
		Vector2 startPos = -new Vector2(frameLayout.X / 2f, frameLayout.Y / 2f) * frameSpacing + inventoryOrigin;
		
		for (int x = 0; x < frameLayout.X; x += 1)
		{
			for (int y = 0; y < frameLayout.Y; y += 1)
			{
				int inventoryIndex = y * frameLayout.X + x;
				
				Vector2 framePos = startPos + new Vector2(x, y) * frameSpacing;

				InventoryFrame newFrame = frame.Instantiate() as InventoryFrame;
				AddChild(newFrame);
				newFrame.Position = framePos;
				newFrame.inventory = this;
				newFrame.inventoryIndex = inventoryIndex;

				StackData playerItem = player.inventory[inventoryIndex];
				if (playerItem == null) continue;
				
				if (playerItem == player.EquippedItemData) SpawnEquippedIcon(newFrame);
				
				InventoryItem newItemIcon = itemIcon.Instantiate() as InventoryItem;
				AddChild(newItemIcon);
				newItemIcon.Position = framePos + itemIconOffset;
				newItemIcon.SetItem(playerItem);
				newItemIcon.placedFrame = newFrame;

				newFrame.currentItem = newItemIcon;
			}
		}
	}

	void SpawnEquippedIcon(InventoryFrame spawnFrame)
	{
		spawnedEquippedIcon?.QueueFree();

		spawnedEquippedIcon = equippedItemIcon.Instantiate() as Control;
		AddChild(spawnedEquippedIcon);
		spawnedEquippedIcon.Position = spawnFrame.Position + equippedIconOffset;
	}

	void CloseInventory()
	{
		cameraController.SetFocusOffset(Vector2.Zero);
		spawnedEquippedIcon = null;

		if (liftedItem != null)
		{
			player.AddInventoryItem(liftedItem.ItemData);
			liftedItem = null;
		}
		
		foreach (Node child in GetChildren()) child.QueueFree();
	}

	void MoveLiftedItem()
	{
		Vector2 mousePos = GetLocalMousePosition();
		liftedItem.Position = mousePos;
	}

	public void ItemIconClicked(InventoryFrame fromFrame)
	{
		if (!open) return;

		if (liftedItem == null)
		{
			if (fromFrame.currentItem == null) return;
			
			liftedItem = fromFrame.currentItem;
			player.RemoveInventoryItem(liftedItem.ItemData);
		}
		else
		{
			if (fromFrame.currentItem != null && fromFrame.currentItem != liftedItem) return;

			liftedItem.placedFrame.currentItem = null;
			fromFrame.currentItem = liftedItem;
			liftedItem.placedFrame = fromFrame;
			liftedItem.Position = fromFrame.Position + itemIconOffset;
			
			player.SetInventoryItem(liftedItem.ItemData, fromFrame.inventoryIndex);
			if (liftedItem.ItemData == player.EquippedItemData) SpawnEquippedIcon(fromFrame);

			liftedItem = null;
		}
	}

	public void EquipItem(InventoryFrame fromFrame)
	{
		if (liftedItem != null) return;
		if (fromFrame.currentItem == null || !fromFrame.currentItem.ItemData.RelatedRes.Equippable) return;
		
		player.EquipInventoryItem(fromFrame.currentItem.ItemData);
		SpawnEquippedIcon(fromFrame);
	}
}
