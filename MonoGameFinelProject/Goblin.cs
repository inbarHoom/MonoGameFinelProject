using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public class Goblin : Enemy
{
    private const string Idle = "GoblinIdle";
    private const string Run = "GoblinRun";
    private const string Attack1 = "GoblinAttack1";
    private const string Attack2 = "GoblinAttack2";
    private const string Attack3 = "GoblinAttack3";
    private const string Hit = "GoblinHit";
    private const string Death = "GoblinDeath";

    private Collider bombRangeCollider;
    private bool playerInBombRange;
    
    private float bombRangeTimer;
    private const float BombRangeRequiredSeconds = 1.5f;

    private bool hasThrownBomb;
    private const int bombAttackReleaseFrame = 11;
    
    public Goblin() : base(Idle, 80)
    {
        speed = 200f;
        _transform._position = new Vector2(700, 530);
        _transform._rotation = 0;
        _transform._scale = Vector2.One * 1.5f;
        _color = Color.White;
        _effects = SpriteEffects.FlipHorizontally;
        attack1Damage = 7;
        attack2Damage = 14;
        attack3Damage = 30;
        PlayAnimation(isLooping: true, samples: 10);
    }

    public override void TakeDamage(int amount)
    {
        if(_state == e_EnemyState.Death) return;
        base.TakeDamage(amount);
        if(_state != e_EnemyState.Death)
            base.StartEnemyAnimation(Hit,e_EnemyState.Hit,false,4);
    }

    protected override void Die()
    {
        base.StartEnemyAnimation(Death,e_EnemyState.Death,false,4);
    }

    public override void CreateCollider()
    {
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(70, 75, 75, 80),
            showDebugOutline = false,
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
            LocalBounds = new Rectangle(145, 80, 10, 70),
            showDebugOutline = false,
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
            LocalBounds = new Rectangle(70, 97, 170, 15),
            showDebugOutline = false,
            colliderType = Collider.e_ColliderType.Vision
        };
        visionCollider.onTrigger += OnVisionTrigger;
        SceneManager.Add(visionCollider);
        bombRangeCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(240, 80, 300, 70),
            showDebugOutline = false,
            colliderType = Collider.e_ColliderType.Attack
        };
        bombRangeCollider.onTrigger += OnBombRangeTrigger;
        SceneManager.Add(bombRangeCollider);
    }

    private void OnBombRangeTrigger(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Body) return;
        if (other.Parent is not Player player) return;

        target = player;
        playerInBombRange = true;
    }

    private void StartGoblinAttack1()
    {
        currentAttackDamage = attack1Damage;
        hasHitTarget = false;
        
        StartEnemyAnimation(Attack1,e_EnemyState.Attack1,false,8);
    }
    private void StartGoblinAttack2()
    {
        currentAttackDamage = attack2Damage;
        hasHitTarget = false;
        
        StartEnemyAnimation(Attack2,e_EnemyState.Attack2,false,8);
    }

    private void StartGoblinAttack3()
    {
        currentAttackDamage = attack3Damage;
        hasThrownBomb = false;
        StartEnemyAnimation(Attack3, e_EnemyState.Attack3, false,  8);
    }

    private void ThrowBomb()
    {
        Collider ground =SceneManager.colliders.Find(collider => collider.colliderType == Collider.e_ColliderType.Ground);
        if(target == null || ground == null) return;
        bool facingLeft = (_effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None;

       float handOffsetX = facingLeft ? 60f : 165f;
        float handOffsetY = 85f;

        Vector2 handPos = _transform._position + new Vector2(handOffsetX, handOffsetY);
        Vector2 bombStart = handPos - new Vector2(50f, 50f);
        Bomb bomb = SceneManager.Create<Bomb>();
        
        Vector2 bombTarget = target._transform._position;
        bombTarget.Y = ground._rect.Top - bomb.GroundContactOffset;
        
       // bomb.Throw(this,bombStart,target._transform._position,attack3Damage);
       bomb.Throw(this, bombStart, bombTarget, attack3Damage);
    }
    
    protected override void OnChaseStarted()
    {
        StartEnemyAnimation(Run, e_EnemyState.Run, loops: true, framesPerSecond: 10);
    }

    protected override void StartIdleAnimation()
    {
        StartEnemyAnimation(Idle, e_EnemyState.Idle, loops: true, framesPerSecond: 4);
    }
    protected override string RollDeathDropItemId() => ChestLootTable.RollOneItemId();
    public override void Update(GameTime gameTime)
    {
        bool playerWasInBombRange = playerInBombRange;
        playerInBombRange = false;

        bool canStartBombAttack =
            _state == e_EnemyState.Idle || _state == e_EnemyState.Run;

        if (playerWasInBombRange && canStartBombAttack)
        {
            bombRangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (bombRangeTimer >= BombRangeRequiredSeconds)
            {
                bombRangeTimer = 0f;
                StartGoblinAttack3();
            }
        }
        else
        {
            bombRangeTimer = 0f;
        }

        base.Update(gameTime);
    }

    protected override bool UpdateSpecificBehavior(GameTime gameTime, bool playerInAttackRange)
    {
        if (_state == e_EnemyState.Attack3)
        {
            if (_state == e_EnemyState.Attack3)
            {
                if (!hasThrownBomb &&
                    target != null &&
                    CurrentFrame >= bombAttackReleaseFrame)
                {
                    ThrowBomb();
                    hasThrownBomb = true;
                }

                if (isFinished)
                    StartIdleAnimation();

                return true;
            }
        }
        bool attackFrame = (_state == e_EnemyState.Attack1 || _state == e_EnemyState.Attack2) && CurrentFrame >= 4;

        if (attackFrame && playerInAttackRange && !hasHitTarget)
        {
            target?.TakeDamage(currentAttackDamage);
            hasHitTarget = true;
        }

        if (_state == e_EnemyState.Attack1 && isFinished)
        {
            if (playerInAttackRange)
                StartGoblinAttack2();
            else
                StartIdleAnimation();
            return true;
        }
        else if (_state == e_EnemyState.Attack2 && isFinished)
        {
            StartIdleAnimation();
            return true;
        }
        else if (playerInAttackRange && (_state == e_EnemyState.Idle || _state == e_EnemyState.Run))
        {
            StartGoblinAttack1();
            return true;
        }

        return _state == e_EnemyState.Attack1 || _state == e_EnemyState.Attack2;
    }
}
