using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : RigidBody2D
{
    [Export] private float speed;
    [Export] private int health;
    [Export] private float hunger;
    [Export] private float hungerRate;
    [Export] private int attackDamage;
    [Export] public int inventorySize;
    
    [Export] private Curve dashCurve;
    [Export] private float dashPower;
    [Export] private float dashTimeMax, dashRegenTimeMax;
	
    [Export] Camera2D mainCamera;
	
	float wiggleT;
	[Export] private float wiggleSpeed, wiggleIntensity;
	[Export] float forcePerspCutoff;

	[Export] Sprite2D hand1, hand2;
	[Export] Vector2 handOffset;
	[Export] private Vector2 handVerticalShift;
	
	[Export] private Sprite2D eye1, eye2;
	[Export] private Vector2 eyeOffset;
	[Export] private Vector2 eyeVerticalShift;
	
	[Export] private Sprite2D foot1, foot2;
	[Export] private Vector2 footOffset;
	[Export] private Vector2 footVerticalShift;

	public Inventory.StackData[] inventory = new Inventory.StackData[16];
	Node2D equippedItem;
	public Inventory.StackData EquippedItemData;

	Vector2 dashDir;
	bool isDashing;
	float dashTime, dashRegenTime;


    // helpers
    Object RayCast(Vector2 start, Vector2 end)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(start, end);
        var result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            return result["collider"];
        }

        return null;
    }

    public override void _Ready()
    {
	    dashRegenTime = dashRegenTimeMax;
	    // inventory = new List<Inventory.StackData>(inventorySize); REPLACE AFTER TESTING
    }

    public override void _Process(double delta)
    {
        hunger += (float)delta * hungerRate;
        
        if (isDashing)
        {
            dashTime += (float)delta;
            if (dashTime > dashTimeMax)
            {
                dashTime = dashTimeMax;
                isDashing = false;
            }

            float targetVel = dashCurve.Sample(dashTime / dashTimeMax) * dashPower;
            LinearVelocity = dashDir * targetVel;
        }
        else
        {
            Move((float)delta);
            dashRegenTime += (float)delta;
        }

        Animate();
    }

    void AnimateBodyPart(Sprite2D bodyP, Vector2 lookDir, Vector2 perpDir, Vector2 wiggle, Vector2 offset,
        Vector2 verticalShift)
    {
        if (LinearVelocity == Vector2.Zero) wiggle = Vector2.Zero;
        bodyP.Position = perpDir * offset + wiggle + verticalShift;
        bodyP.FlipH = lookDir.X > 0;
        bodyP.ZIndex = perpDir.Y < 0 ? -1 : 1;

        if (Mathf.Abs(lookDir.X) < forcePerspCutoff)
        {
            int order = lookDir.Y < 0 ? -1 : 1;
            bodyP.ZIndex = order;
        }
    }

    void Animate()
    {
        Vector2 lookDir = GetLocalMousePosition().Normalized();
        Vector2 perpDir = lookDir.Rotated(Mathf.Pi / 2);
        Vector2 wiggle = Mathf.Sin(wiggleT * wiggleSpeed) * wiggleIntensity * lookDir;
        
        AnimateBodyPart(hand1, lookDir, perpDir, wiggle, handOffset, handVerticalShift);
        AnimateBodyPart(hand2, lookDir, -perpDir, -wiggle, handOffset, handVerticalShift);
        
        AnimateBodyPart(eye1, lookDir, perpDir, wiggle, eyeOffset, eyeVerticalShift);
        AnimateBodyPart(eye2, lookDir, -perpDir, -wiggle, eyeOffset, eyeVerticalShift);
        
        AnimateBodyPart(foot1, lookDir, perpDir, wiggle, footOffset, footVerticalShift);
        AnimateBodyPart(foot2, lookDir, -perpDir, -wiggle, footOffset, footVerticalShift);

    }


    public void _input(InputEvent @event)
    {
        if (@event.IsActionPressed("hit"))
        {
        }

        if (@event.IsActionPressed("dash") && dashRegenTime >= dashRegenTimeMax)
        {
            isDashing = true;
            dashTime = 0;
            dashRegenTime = 0;
            dashDir = GetLocalMousePosition().Normalized();
        }
    }

    void Move(float delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        wiggleT += delta * inputDir.Length();
        LinearVelocity = inputDir * speed;
    }

    int GetInventoryUsage()
    {
	    int itemCount = inventory.Count(item => item != null);
	    return itemCount;
    }
    
    public void AddInventoryItem(Inventory.StackData item)
    {
	    if (GetInventoryUsage() >= inventorySize) return;

	    for (int i = 0; i < inventory.Length; i += 1)
	    {
		    if (inventory[i] != null) continue;

		    inventory[i] = item;
		    return;
	    }
    }

    public void RemoveInventoryItem(Inventory.StackData item)
    {
	    for (int i = 0; i < inventory.Length; i += 1)
	    {
		    if (inventory[i] != item) continue;
		    
		    inventory[i] = null;
		    return;
	    }
	    
	    GD.PrintErr("Tried to remove item not in inventory");
    }

    public void SetInventoryItem(Inventory.StackData item, int index)
    {
	    if (inventory[index] != null)
	    {
		    GD.PrintErr("Tried to set a non-empty inventory slot");
		    return;
	    }

	    inventory[index] = item;
    }

    void UnequipItem()
    {
	    if (equippedItem == null) return;
	    
	    equippedItem.QueueFree();
	    equippedItem = null;
	    EquippedItemData = null;
    }

    public void EquipInventoryItem(Inventory.StackData item)
    {
	    UnequipItem();
	    
	    Node2D spawnedItem = item.RelatedRes.EquippedScene.Instantiate() as Node2D;
	    hand1.AddChild(spawnedItem);

	    equippedItem = spawnedItem;
	    EquippedItemData = item;
    }
}