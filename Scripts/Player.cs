using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : RigidBody2D
{
    [ExportCategory("Controllers")]
    [Export] Node2D dayNightCycle;
    // Sounds 
    [ExportCategory("Sound Effects")]
    [Export] AudioStreamPlayer footstepSound;
    [Export] AudioStreamPlayer swingSound;
    [Export] AudioStreamPlayer dashSound;

    [ExportCategory("Stats")]
    [Export] float speed;
    [Export] public float health;
    [Export] public float maxHealth;
    [Export] public float temp;
    [Export] public float maxTemp;
    [Export] public float tempRate;
    [Export] public float hunger;
    [Export] public float maxHunger;
    [Export] float defaultHungerRate;
    [Export] public float currHungerRate;
    [Export] float dashHungerRate;
    [Export] float moveHungerRate;
    [Export] public int attackDamage;
    [Export] public int inventorySize;

    [ExportCategory("Player Logic")]
    [Export] float attackTimeMax;
    [Export] Curve attackCurve;

    [Export] Curve dashCurve;
    [Export] float dashPower;
    [Export] float dashTimeMax, dashRegenTimeMax;

    [ExportCategory("Camera and Animation")]
    [Export] Camera2D mainCamera;

    [Export] float wiggleSpeed, wiggleIntensity;
    [Export] float alignRotSpeed;
    [Export] float forcePerspCutoff;

    [Export] Vector2 handOffset;
    [Export] Vector2 handVerticalShift;


    [Export] Vector2 eyeOffset;
    [Export] Vector2 eyeVerticalShift;
    [ExportCategory("Sprites")]
    [Export] Sprite2D[] hands;
    [Export] Sprite2D[] eyes;
    public List<Inventory.StackData> Inventory = [];
    Node2D equippedItem;
    public Inventory.StackData EquippedItemData;

    // Dash vars
    Vector2 dashDir;
    bool isDashing;
    float dashTime, dashRegenTime;

    // attack vars
    Vector2 attackDir;
    float attackTime;
    bool isAttacking;

    // animation vars
    float wiggleT;
    Vector2 lookDir, lookingDir;

    bool controllerMode;

    public void DecrementStats(float reductionRate, ref int stat, float fDelta)
    {
        if (stat > 0)
        {
            stat -= (int) (fDelta * reductionRate); 
        }
    }
    public void DecrementStats(float reductionRate,ref float stat, float fDelta)
    {
        if (stat > 0f)
        {
            stat -= fDelta * reductionRate; 
        }
    }
    public void IncrementStats(float additionRate,ref int stat, int maxStat, float fDelta)
    {
        if (stat < maxStat)
        {
            stat +=(int) (fDelta * additionRate);
        }
    }
        public void IncrementStats(float additionRate,ref float stat, float maxStat, float fDelta)
    {
        if (stat < maxStat)
        {
            stat += fDelta * additionRate;
        }
    }
 
    public override void _Ready()
    {
        lookDir = Vector2.Down;
        lookingDir = Vector2.Down;
        dashRegenTime = dashRegenTimeMax;
        // inventory = new List<Inventory.StackData>(inventorySize); REPLACE AFTER TESTING
    }


    public override void _Process(double delta)
    {
        float fDelta = (float)delta;
        IncrementStats(currHungerRate, ref hunger, maxHunger, fDelta);
        DecrementStats(tempRate, ref temp, fDelta);
        GD.Print($"hunger rate {currHungerRate} | temp {temp}");         
        if (isDashing)
        {
            dashTime += fDelta;
            if (dashTime > dashTimeMax)
            {
                currHungerRate = defaultHungerRate;
                dashTime = dashTimeMax;
                isDashing = false;
            }

            float targetVel = dashCurve.Sample(dashTime / dashTimeMax) * dashPower;
            LinearVelocity = dashDir * targetVel;
        }
        else
        {
            Move(fDelta);
            dashRegenTime += fDelta;
        }
        if (isAttacking)
        {
            attackTime += fDelta;
            if (attackTime >= attackTimeMax)
            {
                attackTime = attackTimeMax;
                isAttacking = false;
            }
        }

        Animate(fDelta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("hit"))
        {
            if (isAttacking) return;
            swingSound.Play();
            attackTime = 0;
            isAttacking = true;
            Vector2 aimDir = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down").Normalized();
            if (controllerMode)
                attackDir = aimDir.Length() > 0 ? aimDir : lookingDir;
            else
            {
                attackDir = GetLocalMousePosition().Normalized();
            }
        }

        if (@event.IsActionPressed("dash") && dashRegenTime >= dashRegenTimeMax)
        {
            currHungerRate = dashHungerRate;
            dashSound.Play();
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
        if (inputDir != Vector2.Zero)
        {
            currHungerRate = moveHungerRate;
            lookDir = inputDir.Normalized();
            if (!footstepSound.IsPlaying()) footstepSound.Play();
        }
        else if (!controllerMode && !isAttacking)
        {
            currHungerRate = defaultHungerRate;
            lookDir = GetLocalMousePosition().Normalized();
        }
        wiggleT += delta * inputDir.Length();
        LinearVelocity = inputDir * speed;
    }

    // Animation Methods
    void SetZIndexes(Sprite2D[] spriteNodes, Vector2[] perpDirs, Vector2 targetDir, int[] targetZIndices)
    {
        if (spriteNodes.Length != perpDirs.Length)
        {
            GD.PrintErr("SetZIndexes: spriteNodes and perpDirs length do not match");
            return;
        }
        bool perspCutoff = Mathf.Abs(targetDir.X) < forcePerspCutoff;

        for (int i = 0; i < spriteNodes.Length; i++)
        {
            int targetZIndex = targetZIndices[i];
            if (perspCutoff)
            {
                int order = targetDir.Y < 0 ? -1 : 1;
                spriteNodes[i].ZIndex = order * targetZIndex;
            }
            else
                spriteNodes[i].ZIndex = (perpDirs[i].Y < 0 ? -1 : 1) * targetZIndex;
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
        if (equippedItem != null)
            equippedItem.Rotation = sample / 2f * Mathf.Pi * -Mathf.Sign(perpDir.Y);

        Sprite2D[] spriteNodes = [eyes[0], eyes[1], hands[0], hands[1]];
        int[] targetZIndices = [1, 1, 3, 3];
        Vector2[] perpDirs = [perpDir, -perpDir, perpDir, -perpDir];
        SetZIndexes(spriteNodes, perpDirs, simulatedLookDir, targetZIndices);
    }
    void AnimateBodyParts(
        Sprite2D[] bodyParts,
        Vector2[] perpDirs,
        Vector2 offset,
        Vector2 verticalShift,
        Vector2 wiggle,
        int targetZIndex
    )

    {
        if (LinearVelocity == Vector2.Zero)
            wiggle = Vector2.Zero;
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].Position = perpDirs[i] * offset + wiggle + verticalShift;
            bodyParts[i].FlipH = lookingDir.X > 0;
        }

        int[] zIndices = [targetZIndex, targetZIndex];
        SetZIndexes(bodyParts, perpDirs, lookingDir, zIndices);
    }
    void Animate(float delta)
    {
        Vector2 perpDir = lookingDir.Rotated(Mathf.Pi / 2);
        Vector2[] perpDirs = [perpDir, -perpDir];

        float currentAngle = Mathf.Atan2(lookingDir.Y, lookingDir.X);
        float targetAngle = Mathf.Atan2(lookDir.Y, lookDir.X);
        float moveAmount = targetAngle - currentAngle;
        if (Mathf.Abs(moveAmount) > Mathf.Pi) moveAmount -= Mathf.Tau * Mathf.Sign(moveAmount);

        lookingDir = lookingDir.Rotated(moveAmount * alignRotSpeed * delta);

        if (isAttacking)
        {
            AnimateAttack(attackDir);
        }
        else
        {
            Vector2 wiggle = Mathf.Sin(wiggleT * wiggleSpeed) * wiggleIntensity * lookingDir;
            AnimateBodyParts(hands, perpDirs, handOffset, handVerticalShift, wiggle, 3);
            AnimateBodyParts(eyes, perpDirs, eyeOffset, eyeVerticalShift, wiggle, 1);

        }
    }

    int GetInventoryUsage() => Inventory.Count;

    public bool AddInventoryItem(Inventory.StackData item)
    {
        GD.Print(GetInventoryUsage() >= inventorySize);
        if (GetInventoryUsage() >= inventorySize) return false;

        Inventory.Add(item);
        return true;
    }

    public void RemoveInventoryItem(Inventory.StackData item)
    {
        for (int i = 0; i < GetInventoryUsage(); i += 1)
        {
            if (Inventory[i] != item) continue;

            Inventory.RemoveAt(i);
            return;
        }

        GD.PrintErr("Tried to remove item not in inventory");
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
        hands[0].AddChild(spawnedItem);

        equippedItem = spawnedItem;
        EquippedItemData = item;
    }

    public bool ConsumeInventoryItem(ItemRes targetItem, int targetAmount)
    {
        int currCount = 0;
        List<Inventory.StackData> targetElements = new List<Inventory.StackData>();

        for (int i = 0; i < Inventory.Count; i++)
        {
            var currItem = Inventory[i];
            if (currItem.RelatedRes == targetItem)
            {
                currCount += currItem.StackCount;
                targetElements.Add(currItem);
            }
        }
        if (currCount < targetAmount) return false;

        foreach (var currItem in targetElements)
        {
            if (targetAmount == 0) return true;
            if (targetAmount >= currItem.StackCount)
            {
                targetAmount -= currItem.StackCount;
                RemoveInventoryItem(currItem);
            }
            else
            {
                currItem.StackCount -= targetAmount;
                targetAmount = 0;
            }

        }
        return targetAmount == 0;
    }
}