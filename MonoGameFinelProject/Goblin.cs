using System;
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
    
    
    public Goblin() : base(Idle, 80)
    {
        _transform._position = new Vector2(500, 530);
        _transform._rotation = 0;
        _transform._scale = Vector2.One * 1.5f;
        _color = Color.White;
        _effects = SpriteEffects.FlipHorizontally;
        PlayAnimation(isLooping: true, samples: 10);
    }

    public override void TakeDamage(int amount)
    {
        base.StartEnemyAnimation(Hit,e_EnemyState.Hit,false,4);
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        base.StartEnemyAnimation(Death,e_EnemyState.Death,false,4);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}