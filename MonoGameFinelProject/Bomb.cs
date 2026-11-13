using Microsoft.Xna.Framework;

namespace MonoGameFinelProject;

public class Bomb : Animation , IThrowable
{
    private enum e_BombState
    {
        Inactive,
        Flying,
        Exploding,
        Finished,
        Returning,
        AtTarget,
        Landed
    }

    private Sprite thrower;
    private e_BombState state = e_BombState.Inactive;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float flightElapsed;
    private float flightDuration;
    private float arcHeight = 140f;
    private int damage;

    private Collider explosionCollider;
    private readonly Collider bodyCollider;
    private bool hasDamagedTarget;
    
    private Vector2 velocity;
    private float targetX;

    private const float BombGravity = 1800f;
    private const float InitialUpwardSpeed = -700f;
    private const float HorizontalSpeed = 400f;
    
    public int GroundContactOffset => bodyCollider.LocalBounds.Bottom;

    private const int BombFramesBeforeExplosion = 9;
    private const int BombFramesPerSecond = 4;
    private const int ExplosionFramesPerSecond = 15;
    public Bomb() : base("Bomb")
    {
        explosionCollider = new Collider
        {
            Parent = this,
            IsEnabled = false,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(10,15,95,60),
            colliderType = Collider.e_ColliderType.Attack,
            showDebugOutline = false
        };
        explosionCollider.onTrigger += OnExplosionTrigger;
        SceneManager.Add(explosionCollider);
        bodyCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = false,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(30,25,50,50),
            colliderType = Collider.e_ColliderType.Body,
            showDebugOutline = false
        };
        bodyCollider.onCollision += OnBodyCollider;
        SceneManager.Add(bodyCollider);
    }

    private void OnBodyCollider(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Ground) return;
        if (state != e_BombState.Flying && state != e_BombState.Returning) return;

        Vector2 position = _transform._position;
        position.Y = other._rect.Top - self.LocalBounds.Bottom;
        _transform._position = position;
        velocity = Vector2.Zero;
        state = e_BombState.Landed;
        if(CurrentFrame >= BombFramesBeforeExplosion)
            BeginExplosion();
        else
            ResumeAnimation();
    }

    private void OnExplosionTrigger(Collider self, Collider other)
    {
        if (other.colliderType != Collider.e_ColliderType.Body) return;
        if (hasDamagedTarget) return;
        
        if (thrower is Goblin && other.Parent is Player player)
        {
            player.TakeDamage(damage);
            hasDamagedTarget = true;
        }
        else if (thrower is Player && other.Parent is Enemy enemy)
        {
            enemy.TakeDamage(damage);
            hasDamagedTarget = true;
        }
    }

    public void Throw(Sprite thrower,Vector2 startPos, Vector2 targetPos, int damage)
    {
        this.thrower = thrower;
        startPosition = startPos;
        targetPosition = targetPos;
        this.damage = damage;
        targetX = targetPos.X;
        velocity = new Vector2(targetX < startPos.X ? -HorizontalSpeed : HorizontalSpeed, InitialUpwardSpeed);

        flightElapsed = 0;
       // flightDuration = (float)BombFramesBeforeExplosion / BombFramesPerSecond;
        state = e_BombState.Flying;

        _transform._position = startPos;
        PlayAnimation( false,  BombFramesPerSecond);
    }

    public bool TryParry(Player player)
    {
        Collider ground = SceneManager.colliders.Find(
            collider => collider.colliderType == Collider.e_ColliderType.Ground);

        if (ground == null) return false;
        
        if ((state != e_BombState.Flying && state != e_BombState.Landed) || player == null)
            return false;

        if (thrower is not Enemy originalThrower)
            return false;
        
        startPosition = _transform._position;
        targetPosition = new Vector2(originalThrower._transform._position.X + 30f, ground._rect.Top - GroundContactOffset);
        thrower = player;
        flightElapsed = 0f;
        flightDuration = 0.45f;
        arcHeight = 60f;
        state = e_BombState.Returning;
        PauseAnimation();
        return true;
    }
    
    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (state == e_BombState.Flying)
        {
            velocity.Y += BombGravity * dt;
            
            Vector2 position = _transform._position;
            float stepX = velocity.X * dt;
            float remainingX = targetX - position.X;

            if ((velocity.X > 0 && remainingX <= stepX) || (velocity.X < 0 && remainingX >= stepX))
            {
                position.X = targetX;
                velocity.X = 0;
            }
            else
            {
                position.X += stepX;
            }

            position.Y += velocity.Y * dt;
            _transform._position = position;
        }
        
        if (state == e_BombState.Returning)
        {
            flightElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float t = MathHelper.Clamp(flightElapsed / flightDuration, 0f, 1f);

            float x = MathHelper.Lerp(startPosition.X, targetPosition.X, t);
            float straightY = MathHelper.Lerp(startPosition.Y, targetPosition.Y, t);
            float arc = 4f * arcHeight * t * (1f - t);
            float y = straightY - arc;

            _transform._position = new Vector2(x, y);

            if (t >= 1f)
            {
                _transform._position = targetPosition;
                if (state == e_BombState.Returning)
                {
                    state = e_BombState.AtTarget;
                    ResumeAnimation();
                }
            }
        }

        base.Update(gameTime);
        
        if (state == e_BombState.AtTarget &&
            CurrentFrame >= BombFramesBeforeExplosion)
        {
            BeginExplosion();
        }
        /*
        if ((state == e_BombState.AtTarget || state == e_BombState.Landed) &&
            CurrentFrame >= BombFramesBeforeExplosion)
        {
            BeginExplosion();
            if (isFinished)
            {
                
            }
        }
        */
        if (state == e_BombState.Flying &&
            CurrentFrame >= BombFramesBeforeExplosion)
        {
            PauseAnimation();
        }

        if ((state == e_BombState.Landed || state == e_BombState.AtTarget) &&
            CurrentFrame >= BombFramesBeforeExplosion)
        {
            BeginExplosion();
        }

        if (state == e_BombState.Exploding &&
            CurrentFrame == BombFramesBeforeExplosion)
        {
            samples = ExplosionFramesPerSecond;
        }
        if (state == e_BombState.Exploding && isFinished)
        {
            state = e_BombState.Finished;

            bodyCollider.IsEnabled = false;
            explosionCollider.IsEnabled = false;

            SceneManager.Remove(bodyCollider);
            SceneManager.Remove(explosionCollider);
            SceneManager.Remove(this);
        }
    }
    private void BeginExplosion()
    {
        state = e_BombState.Exploding;
        bodyCollider.IsEnabled = false;
        explosionCollider.IsEnabled = true;
    }
}