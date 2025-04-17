using Godot;
using System;

[GlobalClass]
public partial class ItemRes : Resource
{
    [ExportCategory("rules")]
    [Export] public string Name;
    [Export] public bool Stackable, Equippable;
    [Export] public int[] NumsPerDrop; // array of possible qtys in dropped stack
    
    [ExportCategory("assets and scene")]
    [Export] public Texture2D InventoryTex;

    [Export] public AudioStream PickupNoise;
    [Export] public AudioStream FullInvNoise;
    
    [Export] public PackedScene EquippedScene;
    
    [ExportCategory("spawning stats")]
    [Export] public float probValue;
    [Export] public int numSpawns;
    


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