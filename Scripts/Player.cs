using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : RigidBody2D
{
    // Exports 
    
    [ExportCategory("Controllers")] 
    [Export] Node2D dayNightCycle;

    // Sounds 
    [ExportCategory("Sound Effects")] 
    [Export] AudioStreamPlayer footstepSound;
    [Export] AudioStreamPlayer swingSound;
    [Export] AudioStreamPlayer dashSound;

    [ExportCategory("Stats Maxs")] 
    [Export] float defaultMaxSpeed;
    [Export] float defaultMaxHealth;
    [Export] private float defaultMaxTemp;
    [Export] float defaultMaxHunger;
    [Export] public int defaultAttackDamage;
    
    [ExportCategory("Stat rates")]
    [Export] private float dayTempRate;
    [Export] private float nightTempRate;
    [Export] float idleHungerRate;
    [Export] float dashHungerRate;
    [Export] float moveHungerRate;
    
    [ExportCategory("debufs")]
    [Export] float hungrySpeed;
    
    [ExportCategory("Inv")]
    [Export] public int inventorySize;
    
    [ExportCategory("Player Logic")] 
    [Export] float attackTimeMax;
    [Export] Curve attackCurve;
    [Export] Curve dashCurve;
    [Export] float dashPower;
    [Export] float dashTimeMax, dashRegenTimeMax;

    [ExportCategory("Camera and Animation")] 
    [Export] Camera2D mainCamera;
    [Export] private GpuParticles2D dashParticles;
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
    
    //encapsulated fields
    
    public List<Inventory.StackData> Inventory;
    Node2D equippedItem;
    public Inventory.StackData EquippedItemData;
    
    
    //stat vars
    
    private float maxHealth;
    private float currHealth;
    
    private float maxSpeed;
    private float currSpeed;

    private float minTemp;
    private float maxTemp;
    private float currTemp;
    private float currTempRate;
    private float currClothingModifier;

    private float maxHunger;
    private float currHunger;
    private float currHungerRate;
    
    private float currAttackDamage;
    
    // Dash vars
    Vector2 dashDir;
    bool isDashing;
    float dashTime, dashRegenTime;

    // attack vars
    Vector2 attackDir;
    float attackTime;
    public bool isAttacking;

    // animation vars
    float wiggleT;
    Vector2 lookDir, lookingDir;

    bool controllerMode;

    // Signals
    public void DayStarted()
    {
        currTempRate = dayTempRate;
        minTemp = maxTemp / 2;
    }

    public void NightStarted()
    {
        currTempRate = nightTempRate;
        minTemp = 0f;
    }

    public void Die()
    {
        QueueFree();
    }
    // Getters
    public float GetHealthPerc()
    {
        return currHealth/maxHealth;
    }
    public float GetHungerPerc()
    {
        return currHunger/maxHunger;
    }
    public float GetTempPerc()
    {
        return currTemp/maxTemp;
    }
    // Setters
    public void ModifyHunger(float value)
    {
        currHunger = Mathf.Clamp(currHunger+value, 0, maxHunger);
        if (currHunger == maxHunger)
        {
            currSpeed = hungrySpeed;
        }
        else
        {
            currSpeed = maxSpeed;
        }
    }
    public void ModifyTemp(float value)
    {
        currTemp = Mathf.Clamp(currTemp+(value*(1f - currClothingModifier)), minTemp, maxTemp);
        if (currTemp == 0) Die();
    }

    public void ModifyHealth(float value)
    {
        currHealth = Mathf.Clamp(currHealth+value, 0, maxHealth);
        if (currHealth == 0)
        {
            Die();
        }
    }

    public void ModifySpeed(float value)
    {
        currSpeed = value;
    }

    public void ModifyAttackDamage(float value)
    {
        currAttackDamage = value;
    }

    public void ModifyClothingModifier(float value)
    {
        currClothingModifier = value;
    }

    public override void _Ready()
    {
        maxHealth = defaultMaxHealth;
        maxHunger = defaultMaxHunger;
        maxTemp = defaultMaxTemp;
        maxSpeed = defaultMaxSpeed;

        currHealth = maxHealth;
        currTemp = maxTemp;
        minTemp = maxTemp / 2;
        currTempRate = dayTempRate;
        currHunger = 0;
        currHungerRate = idleHungerRate;
        currSpeed = defaultMaxSpeed;
        
        lookDir = Vector2.Down;
        lookingDir = Vector2.Down;
        
        
        dashRegenTime = dashRegenTimeMax;
        Inventory = new List<Inventory.StackData>();
    }


    public override void _Process(double delta)
    {
        float fDelta = (float)delta;
        
        ModifyHunger(currHungerRate * fDelta);
        ModifyTemp(currTempRate * fDelta);
        
        if (isDashing)
        {
            dashTime += fDelta;
            if (dashTime > dashTimeMax)
            {
                currHungerRate = idleHungerRate;
                dashTime = dashTimeMax;
                isDashing = false;
            }

            float targetVel = dashCurve.Sample(dashTime / dashTimeMax) * dashPower;
            LinearVelocity = dashDir * targetVel;
            lookDir = dashDir;
        }
        else
        {
            dashParticles.Emitting = false;
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
        if (@event.IsActionPressed("controller_override"))
            controllerMode = true;
        else if (@event.IsActionPressed("keyboard_override"))
            controllerMode = false;

        if (@event.IsActionPressed("hit"))
        {
            if (isAttacking) return;
            swingSound.Play();
            attackTime = 0;
            isAttacking = true;
            Vector2 aimDir = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down").Normalized();
            if (aimDir.Length() > 0.1f)
                attackDir = aimDir.Length() > 0 ? aimDir : lookingDir;
            else
            {
                attackDir = GetLocalMousePosition().Normalized();
            }

            ((Item)equippedItem)?.Use(attackDir);
        }

        if (@event.IsActionPressed("dash") && dashRegenTime >= dashRegenTimeMax && currHunger != 0)
        {
            dashParticles.Emitting = true;
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
            currHungerRate = idleHungerRate;
            lookDir = GetLocalMousePosition().Normalized();
        }

        wiggleT += delta * inputDir.Length();
        LinearVelocity = inputDir * currSpeed;
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

    public bool AddInventoryItem(Inventory.StackData newItem)
    {
        if (GetInventoryUsage() >= inventorySize) return false;
        for (int i = 0; i < Inventory.Count; i++)
        {
            Inventory.StackData currInvItem = Inventory[i];
            if (currInvItem.RelatedRes.Name == newItem.RelatedRes.Name)
            {
                int roomLeft = currInvItem.RelatedRes.MaxStack - currInvItem.StackCount;
                while (roomLeft > 0 && newItem.StackCount > 0)
                {
                    currInvItem.StackCount += 1;
                    newItem.StackCount -= 1;
                    roomLeft -= 1;
                }
            }

            if (newItem.StackCount < 1) return true;
        }

        Inventory.Add(newItem);
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