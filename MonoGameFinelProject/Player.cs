using System;
using System.Diagnostics;
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
    public Collider bodyCollider { get; set; }
    private Collider attackCollider;
    
    private e_PlayerAction _action =  e_PlayerAction.Idle;
    private KeyboardState prevKeyboard;
    // jump
    private bool isAirborne;
    private float groundY;
    private float verticalVelocity;
    private const float gravity = 1800f;
    private const float jumpVelocity = -800f;
    // attack combo
    private bool comboWindowOpen;
    private float comboWindowRemaining;
    private const float ComboWindowSeconds = 0.2f;
    private const int lightAttackDamage = 10;
    private const int heavyAttackDamage = 20;
    private int currentAttackDamage;
    private bool hasHitTarget;

    private readonly Health _health = new Health(100);
   

    public Player() : base(Idle)
    {
        _transform._position = new Vector2(100, 500);
        _transform._rotation = 0;
        _transform._scale = Vector2.One * 1.5f;
        _color = Color.White;
        _effects = SpriteEffects.None;

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
            showDebugOutline = true,
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
            showDebugOutline = true,
            colliderType = Collider.e_ColliderType.Attack
        };
        attackCollider.onTrigger += OnAttackTrigger;
        SceneManager.Add(attackCollider);
    }

    private void OnAttackTrigger(Collider self, Collider other)
    {
        if(other.colliderType != Collider.e_ColliderType.Body) return;
        bool isAttacking = _action == e_PlayerAction.LightAttack || _action == e_PlayerAction.HeavyAttack;
        if(!isAttacking || hasHitTarget) return;
        if (other.Parent is Enemy enemy)
        {
            enemy.TurnTowardAttackerAfterHitIfHitFromBehind(this);
            enemy.TakeDamage(currentAttackDamage);
            hasHitTarget = true;
        }
    }

    private void OnBodyCollision(Collider self, Collider other)
    {
    
    }

    public void TakeDamage(int amount)
    {
        if(_action == e_PlayerAction.Death) return;
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

    public void Heal(int amount) => _health.Heal(amount);

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
    
      public override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        bool lightAtkPressed = WasPressed(keyboard, Keys.J);
        bool heavyAtkPressed = WasPressed(keyboard, Keys.K);
        bool jumpPressed = WasPressed(keyboard, Keys.Space);
        if(WasPressed(keyboard,Keys.B)) TakeDamage(20);

        Vector2 position = _transform._position;

        // Horizontal input works on the ground and in the air.
        int horizontal = 0;
        if (keyboard.IsKeyDown(Keys.A)) horizontal--;
        if (keyboard.IsKeyDown(Keys.D)) horizontal++;

        position.X += horizontal * speed * deltaSeconds;

        if (horizontal < 0)
            _effects = SpriteEffects.FlipHorizontally;
        else if (horizontal > 0)
            _effects = SpriteEffects.None;

        bool moving = horizontal != 0;

        if (_action == e_PlayerAction.Hit)
        {
            if (isFinished)
                ShowGroundAnimation(moving);
        }
        else if (_action == e_PlayerAction.Death)
        {
            if (isFinished)
                Game1.GameOver = true;
        }
        // An attack keeps playing even after its key is released.
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
            /*
            if (isFinished)
            {
                if (heavyAtkPressed)
                {
                    StartAnimation(heavyAttack, e_PlayerAction.HeavyAttack,  false,  12);
                }
                else if (!isAirborne)
                {
                    ShowGroundAnimation(moving);
                }
            }
            */
        }
        else if (_action == e_PlayerAction.HeavyAttack)
        {
            if (isFinished && !isAirborne)
                ShowGroundAnimation(moving);
        }
    
        else if (isAirborne)
        {
            verticalVelocity += gravity * deltaSeconds;
            position.Y += verticalVelocity * deltaSeconds;

            // Switch from Jump to Fall as the player starts descending.
            if (verticalVelocity >= 0 && _action == e_PlayerAction.Jump)
            {
                StartPlayerAnimation(Fall, e_PlayerAction.Fall, true, 8);
            }

            // Land at the same height from which the jump began.
            if (position.Y >= groundY && verticalVelocity > 0)
            {
                position.Y = groundY;
                verticalVelocity = 0;
                isAirborne = false;
                ShowGroundAnimation(moving);
            }
        }
        else if (jumpPressed)
        {
            isAirborne = true;
            groundY = position.Y;
            verticalVelocity = jumpVelocity;

            StartPlayerAnimation(Jump, e_PlayerAction.Jump,false, 10);
        }
        else if (lightAtkPressed)
        {
            StartPlayerAnimation(lightAttack, e_PlayerAction.LightAttack, false,  12,  0.05);
        }
        else
        {
            ShowGroundAnimation(moving);
        }

        _transform._position = position;

        base.Update(gameTime);
        prevKeyboard = keyboard;
    }
/*
    private void SetAnimation(string animationName, bool loops, int framesPerSecond)
    {
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        ChangeAnimation(animationName);
        PlayAnimation(loops, framesPerSecond);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        bool lightAtkPressed = keyboard.IsKeyDown(Keys.J) && !prevKeyboard.IsKeyUp(Keys.J);
        bool heavyAtkPressed = keyboard.IsKeyDown(Keys.K) && !prevKeyboard.IsKeyUp(Keys.K);
        float distance = (float)gameTime.ElapsedGameTime.TotalMilliseconds * speed;
        bool moving = false;
        Vector2 position = _transform._position;

        if (keyboard.IsKeyDown(Keys.D))
        {
            position.X += distance;
            _effects = SpriteEffects.None;
            moving = true;
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            position.X -= distance;
            _effects = SpriteEffects.FlipHorizontally;
            moving = true;
        }
        if (keyboard.IsKeyDown(Keys.W))
        {
            position.Y -= distance;
            moving = true;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            position.Y += distance;
            moving = true;
        }

        if (lightAtkPressed)
            SetAnimation(lightAttack, loops: false, framesPerSecond: 12);
        else if (heavyAtkPressed)
            SetAnimation(heavyAttack, loops: false, framesPerSecond: 12);
        else if (keyboard.IsKeyDown(Keys.Space))
        {
            
            SetAnimation(Jump, loops: false, framesPerSecond: 10);
        }
        else if (moving)
            SetAnimation(Run, loops: true, framesPerSecond: 10);
        else
            SetAnimation(Idle, loops: true, framesPerSecond: 10);

        _transform._position = position;
        prevKeyboard = keyboard;
        base.Update(gameTime);
    }
    */
}
