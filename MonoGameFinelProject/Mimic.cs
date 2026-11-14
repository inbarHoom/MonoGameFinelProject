using System;
using Microsoft.Xna.Framework;
namespace MonoGameFinelProject;

public class Mimic : Enemy
{
    private const string Idle = "MimicIdle";
    private const string Walk = "MimicWalk";
    private const string Attack1 = "MimicAttack1";
    private const string Attack2 = "MimicAttack2";
    private const string Hurt = "MimicHurt";
    private const string Death = "MimicDeath";
    private const string Transform = "MimicTransform";
    
    private bool _deathReported;
    private bool _isTransforming;
    public event Action<Mimic> DeathAnimationFinished;

    public Mimic() : base("MimicIdle", 80)
    {
        _transform._scale = Vector2.One;
        speed = 180f;
        _transform._scale = Vector2.One * 2.5f;
        attack1Damage = 12;
        attack2Damage = 20;
    }

    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(145, 140, 80, 65),
            showDebugOutline = true,
            colliderType = Collider.e_ColliderType.Body
        };
        bodyCollider.onCollision += OnGroundCollision;
        SceneManager.Add(bodyCollider);
        attackCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(225, 140, 45, 65),
            showDebugOutline = true,
            colliderType = Collider.e_ColliderType.Attack
        };
        attackCollider.onTrigger += OnAttackTrigger;
        SceneManager.Add(attackCollider);
        visionCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(145, 150, 220, 40),
            showDebugOutline = true,
            colliderType = Collider.e_ColliderType.Vision
        };
        visionCollider.onTrigger += OnVisionTrigger;
        SceneManager.Add(visionCollider);
    }

    protected override void Die()
    {
        StartEnemyAnimation("MimicDeath", e_EnemyState.Death, false, 6);
    }
    public override void TakeDamage(int amount)
    {
        if (_state == e_EnemyState.Death) return;
        base.TakeDamage(amount);
        if (_state != e_EnemyState.Death)
        {
            _isTransforming = false;
            StartEnemyAnimation(Hurt, e_EnemyState.Hit, loops: false, framesPerSecond: 6);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if(_isTransforming)
        {
            base.Update(gameTime);
            if (isFinished)
            {
                _isTransforming = false;
                StartIdleAnimation();
            }
            return;
        }
        base.Update(gameTime);
        if (_state == e_EnemyState.Death &&
            isFinished &&
            !_deathReported)
        {
            _deathReported = true;
            DeathAnimationFinished?.Invoke(this);
        }
    }

    public void BeginTransform()
    {
        _isTransforming = true;
        StartEnemyAnimation("MimicTransform", e_EnemyState.Idle, false, 7);
    }
    protected override void OnChaseStarted()
    {
        StartEnemyAnimation(Walk, e_EnemyState.Run, loops: true, framesPerSecond: 8);
    }
    protected override void StartIdleAnimation()
    {
        StartEnemyAnimation(Idle, e_EnemyState.Idle, loops: true, framesPerSecond: 8);
    }
    
    protected override bool UpdateSpecificBehavior(GameTime gameTime, bool playerInAttackRange)
    {
        bool attackFrame = (_state == e_EnemyState.Attack1 || _state == e_EnemyState.Attack2) && CurrentFrame >= 6;
        if (attackFrame && playerInAttackRange && !hasHitTarget)
        {
            target?.TakeDamage(currentAttackDamage);
            hasHitTarget = true;
        }

        if (_state == e_EnemyState.Attack1 && isFinished)
        {
            if (playerInAttackRange)
            {
                currentAttackDamage = attack2Damage;
                hasHitTarget = false;
                StartEnemyAnimation(Attack2, e_EnemyState.Attack2, false, 10);
            }
            else
            {
                StartIdleAnimation();
            }
            return true;
        }
        else if (_state == e_EnemyState.Attack2 && isFinished)
        {
            StartIdleAnimation();
            return true;
        }
        else if (playerInAttackRange && (_state == e_EnemyState.Idle || _state == e_EnemyState.Run))
        {
            currentAttackDamage = attack1Damage;
            hasHitTarget = false;
            StartEnemyAnimation(Attack1, e_EnemyState.Attack1, false, 10);
            return true;
        }

        return _state == e_EnemyState.Attack1 || _state == e_EnemyState.Attack2;
    }
}