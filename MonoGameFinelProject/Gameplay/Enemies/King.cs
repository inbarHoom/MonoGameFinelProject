using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public class King : Enemy
{
    private const string Idle = "KingIdle";
    private const string Run = "KingRun";
    private const string Attack1 = "KingAttack1";
    private const string Attack2 = "KingAttack2";
    private const string Attack3 = "KingAttack3";
    private const string Hit = "KingHit";
    private const string Death = "KingDeath";

    private const int AttackHitFrame = 3;

    public King() : base(Idle, 180)
    {
        speed = 130f;
        _transform._scale = Vector2.One * 1.7f;

        attack1Damage = 14;
        attack2Damage = 20;
        attack3Damage = 30;

        StartIdleAnimation();
    }

    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(100, 80, 75, 95),
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
            LocalBounds = new Rectangle(160, 90, 15, 85),
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
            LocalBounds = new Rectangle(120, 97, 230, 20),
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

    private void StartAttack3()
    {
        currentAttackDamage = attack3Damage;
        hasHitTarget = false;
        StartEnemyAnimation(Attack3, e_EnemyState.Attack3, false, 8);
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
    protected override string RollDeathDropItemId() => EnemyLootTable.RollEliteDropItemId();

    protected override bool UpdateSpecificBehavior(
        GameTime gameTime,
        bool playerInAttackRange)
    {
        bool isAttacking =
            _state == e_EnemyState.Attack1 ||
            _state == e_EnemyState.Attack2 ||
            _state == e_EnemyState.Attack3;

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
                if (!playerInAttackRange)
                    StartIdleAnimation();
                else if (_state == e_EnemyState.Attack1)
                    StartAttack2();
                else if (_state == e_EnemyState.Attack2)
                    StartAttack3();
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