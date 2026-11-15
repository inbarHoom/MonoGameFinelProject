namespace MonoGameFinelProject;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public  class Enemy : Animation , IDamagable
{
    protected enum e_EnemyState
    {
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        Hit,
        Death
    }
    protected Health _health;
    protected float speed;
    protected Collider bodyCollider;
    protected Collider attackCollider;
    protected Collider visionCollider;

    protected bool isGrounded;
    protected float verticalVelocity;
    protected const float Gravity = 1800f;
    
    protected e_EnemyState _state = e_EnemyState.Idle;
    protected int attack3Damage {get; set; }  
    protected int attack1Damage {get; set; } 
    protected int attack2Damage {get; set; } 
    protected int currentAttackDamage;
    protected Player target;
    private Player turnTargetAfterHit;
    protected bool hasHitTarget;
    protected bool targetInRange;
    protected bool targetInView;
    
    public int CurrentHealth => _health.Current;
    public int MaxHealth => _health.Maximum;
    public bool IsDead => _state == e_EnemyState.Death;
    protected Enemy(string SpriteName, int maxHealth) : base(SpriteName)
    {
        _health = new Health(maxHealth);
    }
    public virtual void CreateCollider()
    {
        bodyCollider = new Collider();
        bodyCollider.Parent = this;
        SceneManager.Add(bodyCollider);
        bodyCollider.onCollision += OnGroundCollision;
    }
    protected virtual void StartEnemyAnimation(string spriteName, e_EnemyState newState, bool loops, int framesPerSecond, double lastFrameHoldSeconds = 0)
    {
        _state = newState;
        base.StartAnimation(spriteName, loops, framesPerSecond, lastFrameHoldSeconds);
    }

    public virtual void TakeDamage(int amount)
    {
        if(_state == e_EnemyState.Death) return;
        _health.TakeDamage(amount);
        if (_health.Current == 0)
            Die();
    }

    protected virtual void Die()
    {
        
    }
    
    protected void OnGroundCollision(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Ground) return;
        if (verticalVelocity < 0) return; // Ignore the floor while moving up.

        Vector2 position = _transform._position;
        position.Y = other._rect.Top - self.LocalBounds.Bottom;
        _transform._position = position;

        verticalVelocity = 0;
        isGrounded = true;
    }

    public virtual void FaceTarget(Player player)
    {
        if (player == null) return;

        _effects = player._transform._position.X < _transform._position.X
            ? SpriteEffects.FlipHorizontally
            : SpriteEffects.None;
    }

    public void TurnTowardAttackerAfterHitFromBehind(Player player)
    {
        turnTargetAfterHit = null;
        if (player == null) return;

        bool enemyFacingLeft = (_effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None;
        bool playerIsLeft = player._transform._position.X < _transform._position.X;
        
        if (playerIsLeft != enemyFacingLeft)
            turnTargetAfterHit = player;
    }

    protected virtual void OnVisionTrigger(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Body || other.Parent is not Player player) return;

        target = player;
        targetInView = true;
    }

    protected virtual void OnAttackTrigger(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Body || other.Parent is not Player player) return;

        target = player;
        targetInRange = true;
    }

    protected void ChaseTarget(GameTime gameTime)
    {
        if (target == null) return;

        float dx = target._transform._position.X - _transform._position.X;
        if (dx == 0) return;

        float direction = Math.Sign(dx);
        FaceTarget(target);

        Vector2 position = _transform._position;
        position.X += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _transform._position = position;

        if (_state != e_EnemyState.Run)
            OnChaseStarted();
    }

    protected virtual void OnChaseStarted()
    {
    }

    protected virtual void StartIdleAnimation()
    {
    }

    protected virtual bool UpdateSpecificBehavior(GameTime gameTime, bool playerInAttackRange)
    {
        return false;
    }

    public override void Update(GameTime gameTime)
    {
        bool seesPlayer = targetInView;
        bool playerInAttackRange = targetInRange;
        targetInView = false;
        targetInRange = false;

        if (!isGrounded)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            verticalVelocity += Gravity * deltaSeconds;

            Vector2 position = _transform._position;
            position.Y += verticalVelocity * deltaSeconds;
            _transform._position = position;
        }
        
        if (_state == e_EnemyState.Death)
        {
            if(!isFinished) base.Update(gameTime);
            return;
        }

        base.Update(gameTime);

        if (_state == e_EnemyState.Hit)
        {
            if (isFinished)
            {
                if (turnTargetAfterHit != null)
                {
                    FaceTarget(turnTargetAfterHit);
                    turnTargetAfterHit = null;
                }
                StartIdleAnimation();
            }
            return;
        }

        if (UpdateSpecificBehavior(gameTime, playerInAttackRange))
            return;

        if (seesPlayer)
        {
            if (!playerInAttackRange)
                ChaseTarget(gameTime);
        }
        else if (_state == e_EnemyState.Run)
        {
            StartIdleAnimation();
        }
    }
}
