using Godot;
using System;

[GlobalClass]
public partial class ItemRes : Resource
{
    [ExportCategory("rules")]
    [Export] public string Name;
    [Export] public bool Stackable, Equippable;
    [Export] public int MaxStack;
    
    [ExportCategory("assets and scene")]
    [Export] public Texture2D InventoryTex;
    [Export] public AudioStream PickupNoise;
    [Export] public AudioStream FullInvNoise;
    [Export] public PackedScene EquippedScene;
    [Export] public bool Edible;
    [Export] public float hungerRegen;
    public ItemRes() : this("none", true, false, null, null)
    {
    }

    public ItemRes(string name, bool stackable, bool equippable, Texture2D inventoryTex, PackedScene equippedScene)
    {
        Name = name;
        Stackable = stackable;
        Equippable = equippable;
        InventoryTex = inventoryTex;
        EquippedScene = equippedScene;
    }
}