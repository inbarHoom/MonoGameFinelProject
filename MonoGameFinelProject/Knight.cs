using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public class Knight : Enemy
{
    private const string Idle = "KnightIdle";
    private const string Run = "KnightRun";
    private const string Attack1 = "KnightAttack1";
    private const string Attack2 = "KnightAttack2";
    private const string Hit = "KnightHit";
    private const string Death = "KnightDeath";

    private const int AttackHitFrame = 4;

    public Knight() : base(Idle, 100)
    {
        speed = 150f;
        _transform._scale = Vector2.One * 1.5f;

        attack1Damage = 9;
        attack2Damage = 16;

        StartIdleAnimation();
    }

    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(100, 95, 75, 80),
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
            LocalBounds = new Rectangle(165, 90, 25, 65),
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
            LocalBounds = new Rectangle(140, 97, 190, 20),
            colliderType = Collider.e_ColliderType.Vision
        };
        visionCollider.onTrigger += OnVisionTrigger;
        SceneManager.Add(visionCollider);
    }

    private void StartAttack1()
    {
        currentAttackDamage = attack1Damage;
        hasHitTarget = false;
        StartEnemyAnimation(Attack1, e_EnemyState.Attack1, false, 8);
    }

    private void StartAttack2()
    {
        currentAttackDamage = attack2Damage;
        hasHitTarget = false;
        StartEnemyAnimation(Attack2, e_EnemyState.Attack2, false, 8);
    }

    protected override void StartIdleAnimation()
    {
        StartEnemyAnimation(Idle, e_EnemyState.Idle, true, 8);
    }

    protected override void OnChaseStarted()
    {
        StartEnemyAnimation(Run, e_EnemyState.Run, true, 8);
    }

    public override void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        base.TakeDamage(amount);

        if (!IsDead)
            StartEnemyAnimation(Hit, e_EnemyState.Hit, false, 8);
    }

    protected override void Die()
    {
        StartEnemyAnimation(Death, e_EnemyState.Death, false, 8);
    }

    protected override bool UpdateSpecificBehavior(
        GameTime gameTime,
        bool playerInAttackRange)
    {
        bool isAttacking =
            _state == e_EnemyState.Attack1 ||
            _state == e_EnemyState.Attack2;

        if (isAttacking)
        {
            if (CurrentFrame >= AttackHitFrame &&
                playerInAttackRange &&
                !hasHitTarget)
            {
                target?.TakeDamage(currentAttackDamage);
                hasHitTarget = true;
            }

            if (isFinished)
            {
                if (_state == e_EnemyState.Attack1 && playerInAttackRange)
                    StartAttack2();
                else
                    StartIdleAnimation();
            }

            return true;
        }

        if (playerInAttackRange &&
            (_state == e_EnemyState.Idle || _state == e_EnemyState.Run))
        {
            StartAttack1();
            return true;
        }

        return false;
    }
}