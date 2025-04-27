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
	[Export] PackedScene itemListing;
	[Export] PackedScene equippedIcon;
	[Export] VBoxContainer listingsContainer;
	[Export] CameraController cameraController;
	
	[ExportCategory("Inventory Settings")]
	[Export] Vector2 inventoryCameraOffset;
	[Export] Vector2 equippedIconOffset;
	
	[ExportCategory("Item Info")]
	[Export] ItemRes[] items;
	
	bool open;
	Control spawnedEquippedIcon;
	List<InventoryItem> listings = [];

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		if (open)
		{
			CloseInventory();
			OpenInventory();
		}
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

		Visible = !Visible;
	}

	void OpenInventory()
	{
		cameraController.SetFocusOffset(inventoryCameraOffset);
		SpawnListings();
	}

	void SpawnListings()
	{
		foreach (StackData item in player.Inventory)
		{
			InventoryItem newListing = itemListing.Instantiate() as InventoryItem;
			listingsContainer.AddChild(newListing);
			newListing.SetItem(item, this);
			
			if (item == player.EquippedItemData) SpawnEquippedIcon(newListing);
			
			listings.Add(newListing);
		}
	}

	public void EquipItem(InventoryItem item)
	{
		if (player.EquippedItemData == item.ItemData) return; // Unequip?
		if (item.ItemData.RelatedRes.Edible)
		{
			player.ConsumeInventoryItem(item.ItemData.RelatedRes, 1);
			player.ModifyHunger(-item.ItemData.RelatedRes.hungerRegen);
		}
		player.EquipInventoryItem(item.ItemData);
		SpawnEquippedIcon(item);
	}

	void SpawnEquippedIcon(InventoryItem item)
	{
		spawnedEquippedIcon?.QueueFree();

		spawnedEquippedIcon = equippedIcon.Instantiate() as Control;
		item.AddChild(spawnedEquippedIcon);
		spawnedEquippedIcon.Position = equippedIconOffset;
	}

	void CloseInventory()
	{
		cameraController.SetFocusOffset(Vector2.Zero);
		spawnedEquippedIcon = null;
		
		foreach (InventoryItem listing in listings) listing.QueueFree();
		listings.Clear();
	}

	void MoveLiftedItem()
	{
		Vector2 mousePos = GetLocalMousePosition();
	}
}
