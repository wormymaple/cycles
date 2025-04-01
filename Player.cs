using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Player : RigidBody2D
{
    [Export]
    bool controllerMode; // true is xbox remote false is keyboard + mouse
    [ExportCategory("Player Stats")]
    [Export] float speed;

    [Export] int health;

    [Export] float hunger;

    [Export] float hungerRate;

    [Export] int attackDamage;

    [Export] public int inventorySize;

    [ExportCategory("Player Logic")]

    [Export] Curve dashCurve;

    [Export] Curve attackCurve;
    [Export] float attackTimeMax;

    [Export] float dashPower;
    [Export] float dashTimeMax, dashRegenTimeMax;

    [ExportCategory("Animation")]

    [Export] Camera2D mainCamera;
    [Export] float wiggleSpeed, wiggleIntensity;

    [Export] float forcePerspCutoff;

    [Export] Sprite2D[] hands;

    [Export] Vector2 handOffset;

    [Export] Vector2 handVerticalShift;

    [Export] Sprite2D[] eyes;

    [Export] Vector2 eyeOffset;

    [Export] Vector2 eyeVerticalShift;

    [Export] Sprite2D[] feet;

    [Export] Vector2 footOffset;

    [Export] Vector2 footVerticalShift;

    // inventory vars
    public Inventory.StackData[] inventory = new Inventory.StackData[16];
    Node2D equippedItem;
    public Inventory.StackData EquippedItemData;

    // Dash vars
    Vector2 dashDir;
    bool isDashing;
    float dashTime, dashRegenTime;

    // attack vars
    private Vector2 attackDir;
    float attackTime;
    bool isAttacking;

    // animation vars
    float wiggleT;

    private Vector2 lookDir;


    public override void _Ready()
    {
        lookDir = Vector2.Zero;
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
        if (isAttacking)
        {
            attackTime += (float)delta;
            if (attackTime >= attackTimeMax)
            {
                attackTime = attackTimeMax;
                isAttacking = false;
            }
        }

        Animate();
    }

    public void _input(InputEvent @event)
    {
        if (@event.IsActionPressed("hit") && !isAttacking)
        {
            attackTime = 0;
            isAttacking = true;
            Vector2 aimDir = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down").Normalized();
            if (controllerMode)
                attackDir = aimDir.Length() > 0 ? aimDir : lookDir;
            else
            {
                attackDir = GetLocalMousePosition().Normalized();
            }
        }

        if (@event.IsActionPressed("dash") && dashRegenTime >= dashRegenTimeMax)
        {
            isDashing = true;
            dashTime = 0;
            dashRegenTime = 0;
            dashDir = controllerMode
                ? Input.GetVector("left", "right", "up", "down")
                : GetLocalMousePosition().Normalized();
        }
    }

    void Move(float delta)
    {
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        if (inputDir.Abs() > Vector2.Zero)
        {
            lookDir = inputDir.Normalized();
        }
        else if (!controllerMode && !isAttacking)
        {
            lookDir = GetLocalMousePosition().Normalized();
        }
        wiggleT += delta * inputDir.Length();
        LinearVelocity = inputDir * speed;
    }

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

    // Animation Methods
    void SetZIndexes(Sprite2D[] spriteNodes, Vector2[] perpDirs, Vector2 lookDir)
    {
        if (spriteNodes.Length != perpDirs.Length)
        {
            GD.PrintErr("SetZIndexes: spriteNodes and perpDirs length do not match");
            return;
        }
        bool perspCutoff = Mathf.Abs(lookDir.X) < forcePerspCutoff;

        for (int i = 0; i < spriteNodes.Length; i++)
        {
            spriteNodes[i].ZIndex = perpDirs[i].Y < 0 ? -1 : 1;
            if (perspCutoff)
            {
                int order = lookDir.Y < 0 ? -1 : 1;
                spriteNodes[i].ZIndex = order;

            }
        }
    }
    void AnimateAttack(Vector2 attackDir)
    {
        float sample = attackCurve.Sample(attackTime / attackTimeMax);
        Vector2 simulatedLookDir = attackDir.Rotated(sample);
        Vector2 perpDir = simulatedLookDir.Rotated(Mathf.Pi / 2);

        eyes[0].Position = perpDir * eyeOffset + eyeVerticalShift;
        eyes[1].Position = -perpDir * eyeOffset + eyeVerticalShift;

        hands[0].Position = perpDir * handOffset + handVerticalShift;
        hands[1].Position = -perpDir * handOffset + handVerticalShift;

        Sprite2D[] spriteNodes = { eyes[0], eyes[1], hands[0], hands[1] };
        Vector2[] perpDirs = { perpDir, -perpDir, perpDir, -perpDir };
        SetZIndexes(spriteNodes, perpDirs, simulatedLookDir);
    }
    void AnimateBodyParts(
        Sprite2D[] bodyParts,
        Vector2[] perpDirs,
        Vector2 offset,
        Vector2 verticalShift,
        Vector2 wiggle
    )
    {
        if (LinearVelocity == Vector2.Zero)
            wiggle = Vector2.Zero;
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].Position = perpDirs[i] * offset + wiggle + verticalShift;
            bodyParts[i].FlipH = lookDir.X > 0;
        }

        SetZIndexes(bodyParts, perpDirs, lookDir);
    }
    void Animate()
    {
        Vector2 perpDir = lookDir.Rotated(Mathf.Pi / 2);
        Vector2[] perpDirs = [perpDir, -perpDir];

        if (isAttacking)
        {
            AnimateAttack(attackDir);
        }
        else
        {
            Vector2 wiggle = Mathf.Sin(wiggleT * wiggleSpeed) * wiggleIntensity * lookDir;
            AnimateBodyParts(hands, perpDirs, handOffset, handVerticalShift, wiggle);
            AnimateBodyParts(eyes, perpDirs, eyeOffset, eyeVerticalShift, wiggle);

        }
    }

    // Inventory Methods
    int GetInventoryUsage()
    {
        int itemCount = inventory.Count(item => item != null);
        return itemCount;
    }

    public void AddInventoryItem(Inventory.StackData item)
    {
        if (GetInventoryUsage() >= inventorySize)
            return;

        for (int i = 0; i < inventory.Length; i += 1)
        {
            if (inventory[i] != null)
                continue;

            inventory[i] = item;
            return;
        }
    }

    public void RemoveInventoryItem(Inventory.StackData item)
    {
        for (int i = 0; i < inventory.Length; i += 1)
        {
            if (inventory[i] != item)
                continue;

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
        if (equippedItem == null)
            return;

        equippedItem.QueueFree();
        equippedItem = null;
        EquippedItemData = null;
    }

    public void EquipInventoryItem(Inventory.StackData item)
    {
        UnequipItem();

        Node2D spawnedItem = item.RelatedRes.EquippedScene.Instantiate() as Node2D;
        hands[0].AddChild(spawnedItem);

        equippedItem = spawnedItem;
        EquippedItemData = item;
    }
}
