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
        collider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(70, 75, 75, 80),
            showDebugOutline = true
        };
        SceneManager.Add(collider);

        attackCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(145, 80, 50, 70),
            showDebugOutline = true
        };
        attackCollider.onTrigger += OnAttackTrigger;
        SceneManager.Add(attackCollider);
    }

    private void OnAttackTrigger(Collider self, Collider other)
    {
        if(other.Parent is not Player player) return;
        targetInRange = true;
        
        if(_state == e_EnemyState.Death || _state == e_EnemyState.Hit) return;

        if (_state == e_EnemyState.Idle || _state == e_EnemyState.Run)
            StartGoblinAttack1();

        if ((_state == e_EnemyState.Attack1 || _state == e_EnemyState.Attack2) && !hasHitTarget)
        {
            player.TakeDamage(currentAttackDamage);
            hasHitTarget = true;
        }
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

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_state == e_EnemyState.Attack1 && isFinished)
        {
            if (targetInRange)
                StartGoblinAttack2();
            else
                StartEnemyAnimation(Idle,e_EnemyState.Idle,true,4);
        }
        else if (_state == e_EnemyState.Attack2 && isFinished)
        {
            StartEnemyAnimation(Idle,e_EnemyState.Idle,true,4);
        }
        else if (_state == e_EnemyState.Hit && isFinished)
        {
            StartEnemyAnimation(
                Idle, e_EnemyState.Idle, true, 4);
        }

        targetInRange = false;
    
    }
}