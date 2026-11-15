using Microsoft.Xna.Framework;
namespace MonoGameFinelProject;

public class DeathBringer : Enemy
{
    private static readonly AnimationClip AttackClip =
        new(0, 2, 9, 9,  false);

    private static readonly AnimationClip DeathClip =
        new(3, 3, 11, 11,  false);

    private static readonly AnimationClip HurtClip =
        new(2, 3, 3, 3, false);

    private static readonly AnimationClip IdleClip =
        new(0, 0, 8, 8,  true);

    private static readonly AnimationClip WalkClip =
        new(0, 1, 8, 8,  true);

    private const int AttackDamage = 25;
    private const int AttackHitFrame = 5;
    public DeathBringer() : base("DeathBringerSheet", maxHealth: 200)
    {
        speed = 140f;
        _transform._scale = Vector2.One * 2f;

        PlayDeathBringerAnimation(IdleClip, e_EnemyState.Idle);
    }
    private void PlayDeathBringerAnimation(
        AnimationClip clip,
        e_EnemyState state)
    {
        _state = state;
        PlayClip(clip);
    }
    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(70, 75, 75, 80),
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
            LocalBounds = new Rectangle(145, 80, 45, 70),
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
            LocalBounds = new Rectangle(70, 97, 220, 20),
            colliderType = Collider.e_ColliderType.Vision
        };
        visionCollider.onTrigger += OnVisionTrigger;
        SceneManager.Add(visionCollider);
    }
    protected override void StartIdleAnimation()
    {
        PlayDeathBringerAnimation(IdleClip, e_EnemyState.Idle);
    }

    protected override void OnChaseStarted()
    {
        PlayDeathBringerAnimation(WalkClip, e_EnemyState.Run);
    }

    public override void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        base.TakeDamage(amount);

        // base.TakeDamage calls Die() at zero health.
        // Don't replace the death animation with the hurt animation.
        if (!IsDead)
            PlayDeathBringerAnimation(HurtClip, e_EnemyState.Hit);
    }

    protected override void Die()
    {
        PlayDeathBringerAnimation(DeathClip, e_EnemyState.Death);
    }

    protected override bool UpdateSpecificBehavior(
        GameTime gameTime,
        bool playerInAttackRange)
    {
        if (_state == e_EnemyState.Attack1)
        {
            if (CurrentFrame >= AttackHitFrame &&
                playerInAttackRange &&
                !hasHitTarget)
            {
                target?.TakeDamage(AttackDamage);
                hasHitTarget = true;
            }

            if (isFinished)
                StartIdleAnimation();

            return true;
        }

        if (playerInAttackRange &&
            (_state == e_EnemyState.Idle || _state == e_EnemyState.Run))
        {
            hasHitTarget = false;
            PlayDeathBringerAnimation(AttackClip, e_EnemyState.Attack1);
            return true;
        }

        return false;
    }
}