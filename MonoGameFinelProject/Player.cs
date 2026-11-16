using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;


public class Player : Animation, IDamagable, IHealable
{
    // Animations
    private const string Idle = "MartialHeroIdle";
    private const string Run = "MartialHeroRun";
    private const string lightAttack = "MartialHeroAttack1";
    private const string heavyAttack = "MartialHeroAttack2";
    private const string Jump = "MartialHeroJump";
    private const string Hit = "MartialHeroHit";
    private const string Fall = "MartialHeroFall";
    private const string Death = "MartialHeroDeath";

    private enum e_PlayerAction
    {
        Idle,
        Run,
        Jump,
        LightAttack,
        HeavyAttack,
        Hit,
        Fall,
        Death
    }
    
    
    public float speed = 500f;
    // colliders
    private Collider bodyCollider;
    public Collider BodyCollider => bodyCollider;
    private Collider feetCollider;
    private Collider attackCollider;
    
    private e_PlayerAction _action =  e_PlayerAction.Idle;
    private KeyboardState prevKeyboard;
    // jump
    private bool isAirborne = true;
    // private float groundY;
    private float verticalVelocity;
    private const float gravity = 1800f;
    private const float jumpVelocity = -800f;
    // attack combo
    private bool comboWindowOpen;
    private float comboWindowRemaining;
    private const float ComboWindowSeconds = 0.35f;
    private const int lightAttackDamage = 10;
    private const int heavyAttackDamage = 20;
    private int currentAttackDamage;
    private bool hasHitTarget;
    // health bar
    private readonly Health _health = new Health(100);
    public int CurrentHealth => _health.Current;
    public int MaxHealth => _health.Maximum;
    private int parryHitFrame = 3;
    // stamina bar    
    private const float StaminaCapacity = 12f;
    private const float LightAttackStaminaCost = 1f;
    private const float HeavyAttackStaminaCost = 2f;
    private const float JumpStaminaCost = StaminaCapacity * 0.20f;
    private const float StaminaRegenDelay = 1f;
    private const float StaminaRegenPerSecond = 1f;
    public float CurrentStamina { get; private set; } = StaminaCapacity;
    public float MaxStamina => StaminaCapacity;
    private float staminaRegenTimer;
    // inventory
    private readonly PlayerInventory _inventory = new();
    public PlayerInventory Inventory => _inventory;
    // key prompt
    private KeyboardPrompt comboKPrompt;
    public string DisplayedInventoryItemId { get;  set; }
    public event Action<Item> ItemPickedUp;

    public event Action<int> InventoryCycled;
    
    public Player() : base(Idle)
    {
        _transform._position = new Vector2(100, 500);
        _transform._rotation = 0;
        _transform._scale = Vector2.One * 1.5f;
        _color = Color.White;
        _effects = SpriteEffects.None;
        comboKPrompt = new KeyboardPrompt(
            Keys.K,
            new Rectangle(0, 160, 16, 16),
            new Rectangle(16, 160, 16, 16),
            Vector2.Zero,
            2.8f,
            autoFlicker: true)
        {
            IsVisible = false
        };
        
        PlayAnimation(isLooping: true, samples: 10);
    }

    public void CreateCollider()
    {
        // player hitbox
        bodyCollider = new Collider
        {
            Parent = this,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(100,50,100,135),
            showDebugOutline = false,
            colliderType = Collider.e_ColliderType.Body
        };
        bodyCollider.onCollision += OnBodyCollision;
        SceneManager.Add(bodyCollider);
        
        // attack hitbox
        attackCollider = new Collider
        {
            Parent = this,
            isTrigger = true,
            IsEnabled = false,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(200, 50, 80, 140),
            showDebugOutline = false,
            colliderType = Collider.e_ColliderType.Attack
        };
        attackCollider.onTrigger += OnAttackTrigger;
        SceneManager.Add(attackCollider);
        
    }

    private void OnAttackTrigger(Collider self, Collider other)
    {
        if (_action == e_PlayerAction.HeavyAttack && CurrentFrame >= parryHitFrame && other.Parent is Bomb bomb)
        {
            if (bomb.TryParry(this))
                hasHitTarget = true;

            return;
        }
        bool isAttacking = _action == e_PlayerAction.LightAttack || _action == e_PlayerAction.HeavyAttack;
        if(!isAttacking || hasHitTarget) return;
        
        if (other.Parent is Enemy enemy)
        {
            if(other.colliderType != Collider.e_ColliderType.Body) return;
            enemy.TurnTowardAttackerAfterHitFromBehind(this);
            enemy.TakeDamage(currentAttackDamage);
            hasHitTarget = true;
        }
       
    }

    private void OnBodyCollision(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Ground) return;
        if (verticalVelocity < 0) return; 

        Vector2 position = _transform._position;
        position.Y = other._rect.Top - self.LocalBounds.Bottom;
        _transform._position = position;

        verticalVelocity = 0;
        isAirborne = false;
    }

    public void TakeDamage(int amount)
    {
        if(_action == e_PlayerAction.Death) return;
        if (isAirborne)
        {
            verticalVelocity = 0;
        }
        _health.TakeDamage(amount);
        _action = e_PlayerAction.Hit;
        StartPlayerAnimation(Hit,e_PlayerAction.Hit,false,4);
        if (_health.Current <= 0)
            Die();
    }

    public void Die()
    {
        _action = e_PlayerAction.Death;
        StartPlayerAnimation(Death, e_PlayerAction.Death,false,4);
    }
    private void TryUseEquippedItem()
    {
        string itemId = DisplayedInventoryItemId;

        if (string.IsNullOrEmpty(itemId))
            return;

        switch (itemId)
        {
            case "Bomb_Pickup":
                if (_inventory.TryRemoveItem(itemId, 1))
                    ThrowBomb();
                break;

            case "Health_Potion":
                if (_inventory.GetQuantity(itemId) > 0 &&
                    TryHealFraction(HealthPotion.healFraction))
                {
                    _inventory.TryRemoveItem(itemId, 1);
                }

                break;

            case "Stamina_Potion":
                if (_inventory.GetQuantity(itemId) > 0 &&
                    TryRestoreStaminaToFull())
                {
                    _inventory.TryRemoveItem(itemId, 1);
                }

                break;
        }
    }
    private void ThrowBomb()
    {
        Collider ground = SceneManager.colliders.Find(collider => collider.colliderType == Collider.e_ColliderType.Ground);
        if (ground == null) return;

        bool facingLeft = (_effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None;

        float handOffsetX = facingLeft ? 80f : 190f;
        float handOffsetY = 90f;
        Vector2 handPos = _transform._position + new Vector2(handOffsetX, handOffsetY);
        Vector2 bombStart = handPos - new Vector2(50f, 50f);

        Bomb bomb = SceneManager.Create<Bomb>();
        
        float throwDistance = 320f;
        float targetX = bombStart.X + (facingLeft ? -throwDistance : throwDistance);
        Vector2 bombTarget = new Vector2(
            targetX,
            ground._rect.Top - bomb.GroundContactOffset);

        BombPickUp bombData = new BombPickUp();
        bomb.Throw(this, bombStart, bombTarget, bombData.damage);
    }

    public void Heal(int amount) => _health.Heal(amount);
    
    private bool TryHealFraction(float fraction)
    {
        if (_action == e_PlayerAction.Death || fraction <= 0f || CurrentHealth >= MaxHealth)
        {
            return false;
        }

        int amount = Math.Max(1, (int)Math.Ceiling(MaxHealth * fraction));
        int healthBefore = CurrentHealth;

        _health.Heal(amount);

        return CurrentHealth > healthBefore;
    }

    private bool TryRestoreStaminaToFull()
    {
        if (_action == e_PlayerAction.Death || CurrentStamina >= MaxStamina)
            return false;

        CurrentStamina = MaxStamina;
        staminaRegenTimer = 0f;
        return true;
    }

    private bool WasPressed(KeyboardState keyboard, Keys key)
    {
        return keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key);
    }

    private void StartPlayerAnimation(string spriteName, e_PlayerAction newAction, bool loops, int framesPerSecond ,double lastFrameHoldSeconds = 0)
    {
        _action = newAction;
        if (attackCollider != null)
        {
            attackCollider.IsEnabled =
                newAction == e_PlayerAction.LightAttack ||
                newAction == e_PlayerAction.HeavyAttack;
        }
        if (newAction == e_PlayerAction.LightAttack)
        {
            currentAttackDamage = lightAttackDamage;
            hasHitTarget = false;
        }
        else if (newAction == e_PlayerAction.HeavyAttack)
        {
            currentAttackDamage = heavyAttackDamage;
            hasHitTarget = false;
        }
        base.StartAnimation(spriteName, loops, framesPerSecond, lastFrameHoldSeconds);
    }
    private void ShowGroundAnimation(bool moving)
    {
        if (moving)
        {
            if (_action != e_PlayerAction.Run)
                StartPlayerAnimation(Run, e_PlayerAction.Run, loops: true, framesPerSecond: 10);
        }
        else if (_action != e_PlayerAction.Idle)
        {
            StartPlayerAnimation(Idle, e_PlayerAction.Idle, loops: true, framesPerSecond: 10);
        }
    }
    
    private bool TrySpendStamina(float amount)
    {
        if (CurrentStamina < amount)
            return false;

        CurrentStamina -= amount;
        staminaRegenTimer = StaminaRegenDelay;
        return true;
    }

    private void UpdateStamina(float deltaSeconds)
    {
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer = Math.Max(0f, staminaRegenTimer - deltaSeconds);
            return;
        }

        CurrentStamina = Math.Min(
            StaminaCapacity,
            CurrentStamina + StaminaRegenPerSecond * deltaSeconds);
    }
    private void TryInteract()
    {
        if (BodyCollider == null)
            return;
        
        foreach (Collider collider in SceneManager.colliders)
        {
            if (collider.IsEnabled &&
                collider.Parent is Chest chest &&
                chest.PlayerInRange)
            {
                chest.Open();
                return;
            }
        }

        List<Item> itemsToPickup = new();
        
        foreach (Collider collider in SceneManager.colliders)
        {
            
            if (collider.IsEnabled &&
                collider.Parent is Item pickupItem &&
                pickupItem.IsInPickupRange(this))
                {
                    itemsToPickup.Add(pickupItem);
                }
        }
        foreach (Item item in itemsToPickup)
        {
            Debug.WriteLine($"Calling TryPickup: {item.ItemID}");
            item.TryPickup(this);
        }
       
    }
    private void CycleInventory(int direction)
    {
        InventoryCycled?.Invoke(direction);
    }
      public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateStamina(deltaSeconds);

        bool lightAtkPressed = WasPressed(keyboard, Keys.J);
        bool heavyAtkPressed = WasPressed(keyboard, Keys.K);
        bool jumpPressed = WasPressed(keyboard, Keys.Space);
        if (WasPressed(keyboard, Keys.E))
        {
            TryInteract();
        }
        if (WasPressed(keyboard, Keys.Z))
        {
            Collider.ToggleDebugOutlines();
        }

        if (WasPressed(keyboard, Keys.F))
        {
            TryUseEquippedItem();
        }
        if (WasPressed(keyboard, Keys.OemComma) || WasPressed(keyboard, Keys.Left))
        {
            CycleInventory(-1);
        }
        if (WasPressed(keyboard, Keys.OemPeriod) || WasPressed(keyboard, Keys.Right))
        {
            CycleInventory(1);
        }
        if(WasPressed(keyboard,Keys.B)) TakeDamage(20);

        Vector2 position = _transform._position;

        int horizontal = 0;
        if (keyboard.IsKeyDown(Keys.A)) horizontal--;
        if (keyboard.IsKeyDown(Keys.D)) horizontal++;

        position.X += horizontal * speed * deltaSeconds;

        if (horizontal < 0)
            _effects = SpriteEffects.FlipHorizontally;
        else if (horizontal > 0)
            _effects = SpriteEffects.None;

        bool moving = horizontal != 0;
        
        if (isAirborne)
        {
            verticalVelocity += gravity * deltaSeconds;
            position.Y += verticalVelocity * deltaSeconds;

            if (verticalVelocity >= 0 && _action == e_PlayerAction.Jump)
                StartPlayerAnimation(Fall, e_PlayerAction.Fall, true, 8);
        }
        
        if (_action == e_PlayerAction.Hit)
        {
            if (isFinished)
                if (isAirborne)
                {
                    StartPlayerAnimation(Fall, e_PlayerAction.Fall, true, 8);
                }
                else
                {
                    ShowGroundAnimation(moving);    
                }
                
        }
        else if (_action == e_PlayerAction.Death)
        {
            if (isFinished)
                Game1.GameOver = true;
        }
        else  if (_action == e_PlayerAction.LightAttack)
        {
            if (isFinished && !comboWindowOpen)
            {
                comboWindowOpen = true;
                comboWindowRemaining = ComboWindowSeconds;
            }

            if (comboWindowOpen)
            {
                if (heavyAtkPressed)
                {
                    comboWindowOpen = false;
                    if(TrySpendStamina(HeavyAttackStaminaCost))
                      StartPlayerAnimation(heavyAttack,e_PlayerAction.HeavyAttack,false,12);
                }
                else
                {
                    comboWindowRemaining -= deltaSeconds;
                    if (comboWindowRemaining <= 0) 
                    {
                        comboWindowOpen = false;
                        ShowGroundAnimation(moving);
                    }
                }
            }
        }
        else if (_action == e_PlayerAction.HeavyAttack)
        {
            if (isFinished && !isAirborne)
                ShowGroundAnimation(moving);
        }
    
        else if (isAirborne)
        {
            // Gravity and the Fall animation were handled earlier in Update.
            // This prevents the grounded actions below from running in midair.
        }
        else if (jumpPressed && TrySpendStamina(JumpStaminaCost))
        {
            isAirborne = true;
            verticalVelocity = jumpVelocity;

            StartPlayerAnimation(Jump, e_PlayerAction.Jump,false, 10);
        }
        else if (lightAtkPressed && TrySpendStamina(LightAttackStaminaCost))
        {
            StartPlayerAnimation(lightAttack, e_PlayerAction.LightAttack, false,  12,  0.05);
        }
        else
        {
            ShowGroundAnimation(moving);
        }

        _transform._position = position;

        base.Update(gameTime);
        comboKPrompt.IsVisible = comboWindowOpen && _action == e_PlayerAction.LightAttack && StaminaCapacity >= HeavyAttackStaminaCost;
        if (comboKPrompt.IsVisible && bodyCollider != null)
        { 
            comboKPrompt._transform._position = new Vector2(
                bodyCollider._rect.Right - 10,
                bodyCollider._rect.Top - 30);
            comboKPrompt.Update(gameTime);
        }
        prevKeyboard = keyboard;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        comboKPrompt.Draw(spriteBatch);
    }

    public void OnItemPickedUp(Item item)
    {
        if (item == null) return;
        DisplayedInventoryItemId = item.ItemID;
        ItemPickedUp?.Invoke(item);
    }
}
