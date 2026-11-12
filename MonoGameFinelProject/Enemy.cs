namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
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
    protected float speed = 500f;
    protected Collider collider { get; set; }
    protected Collider attackCollider;
    protected e_EnemyState _state = e_EnemyState.Idle;
    protected int attack3Damage {get; set; }  
    protected int attack1Damage {get; set; } 
    protected int attack2Damage {get; set; } 
    protected int currentAttackDamage;
    protected bool hasHitTarget;
    protected bool targetInRange;
    
    protected Enemy(string SpriteName, int maxHealth) : base(SpriteName)
    {
        _health = new Health(maxHealth);
    }
    public virtual void CreateCollider()
    {
        collider = new Collider();
        collider.Parent = this;
        SceneManager.Add(collider);
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
    public override void Update(GameTime gameTime)
    {
        if (_state == e_EnemyState.Death)
        {
            if(!isFinished) base.Update(gameTime);
            return;
        }
        base.Update(gameTime);
    }
}