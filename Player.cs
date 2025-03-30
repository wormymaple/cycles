using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : RigidBody2D
{
    [Export] float speed;
    [Export] int health;
    [Export] float hunger;
    [Export] float hungerRate;
    [Export] int attackDamage;
    [Export] Curve dashCurve;
    [Export] float dashPower;
    [Export] float dashTimeMax, dashRegenTimeMax;
	[Export] Camera2D mainCamera;
	[Export] Sprite2D hand1, hand2;
	[Export] Vector2 handOffset;
	[Export] float handWiggleIntensity, handWiggleSpeed;
	[Export] public int inventorySize;
	float handWiggleT;
	[Export] float forceHandPerspCutoff;

	public Inventory.StackData[] inventory = new Inventory.StackData[16];

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
	    GD.Print(inventory.Length);
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
    
    void Animate()
    {
		Vector2 mousePos = mainCamera.GetGlobalMousePosition();
		Vector2 lookDir = (mousePos - GlobalPosition).Normalized();

		Vector2 perpDir = lookDir.Rotated(Mathf.Pi / 2);
		Vector2 handWiggle = Mathf.Sin(handWiggleT * handWiggleSpeed) * handWiggleIntensity * lookDir;
		hand1.Position = perpDir * handOffset + handWiggle;
		hand2.Position = -perpDir * handOffset - handWiggle;

		hand1.ZIndex = perpDir.Y < 0 ? -1 : 1;
		hand2.ZIndex = -perpDir.Y < 0 ? -1 : 1;

		if (Mathf.Abs(lookDir.X) < forceHandPerspCutoff)
		{
			int order = lookDir.Y < 0 ? -1 : 1;
			hand1.ZIndex = order;
			hand2.ZIndex = order;
		}
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
		handWiggleT += delta * inputDir.Length();
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

    public void EquipInventoryItem(Inventory.StackData item)
    {
	    
    }
}
