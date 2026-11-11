namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
public abstract class Enemy : Animation , IDamagable
{
    protected Health _health;
    protected float speed = 500f;
    protected Collider collider { get; set; }

    protected Enemy(string SpriteName, int maxHealth) : base(SpriteName)
    {
        _health = new Health(maxHealth);
    }

    public virtual void TakeDamage(int amount)
    {
        _health.TakeDamage(amount);
        if (_health.Current == 0)
            Die();
    }

    protected virtual void Die()
    {
        
    }
    public abstract override void Update(GameTime gameTime);
}