using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public class FantasyWarrior : Enemy
{
    private const string Idle = "FantasyWarriorIdle";
    private const string Run = "FantasyWarriorRun";
    private const string Attack1 = "FantasyWarriorAttack1";
    private const string Attack2 = "FantasyWarriorAttack2";
    private const string Attack3 = "FantasyWarriorAttack3";
    private const string Hit = "FantasyWarriorHit";
    private const string Death = "FantasyWarriorDeath";

    private const int AttackHitFrame = 4;

    public FantasyWarrior() : base(Idle, 130)
    {
        speed = 160f;
        _transform._scale = Vector2.One * 1.5f;

        attack1Damage = 10;
        attack2Damage = 15;
        attack3Damage = 22;

        StartIdleAnimation();
    }

    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(90, 75, 55, 80),
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
            LocalBounds = new Rectangle(145, 80, 15, 70),
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
            LocalBounds = new Rectangle(120, 90, 200, 20),
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