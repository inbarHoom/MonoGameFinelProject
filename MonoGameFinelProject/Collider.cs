using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Collider : Sprite
{
    public enum e_ColliderType
    {
        None,
        Body,
        Attack,
        Vision,
        Ground,
        Item
    }
    
    public e_ColliderType colliderType = e_ColliderType.None;
    
     public bool isTrigger { get; set; } = false;

     public bool IsEnabled { get; set; } = true;
     public bool UseCustomBounds { get; set; }
     public Rectangle LocalBounds { get; set; }
     
     public bool showDebugOutline { get; set; }
     
    public Action<Collider, Collider> onTrigger;
    public Action<Collider, Collider> onCollision;

    public Sprite Parent { get; set; } 
    

    public Collider() : base("Pixel")
    { }

    public bool IsColliding(Collider otherCollider)
    {
        return _rect.Intersects(otherCollider._rect);
    }

    void Notify(Collider other)
    {
        if (isTrigger)
            onTrigger?.Invoke(this, other);
        else
            onCollision?.Invoke(this, other);
    }
    
    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Z))
            TurnOffCollider();
        if (Parent == null || !IsEnabled) return;
        base.Update(gameTime);
        RefreshBounds();
        /*
        if(!UseCustomBounds)
            _rect = Parent._rect;
        else
        {
            _rect = LocalBounds;
        }
        */
        // go on all colliders and check if intersect with them
        // while ignore self
        for (int i = 0; i < SceneManager.colliders.Count; i++)
        {
            Collider other = SceneManager.colliders[i];
            
            if (other == this ||other.Parent == null || !other.IsEnabled) continue;
          other.RefreshBounds();
          
            if (IsColliding(other))
                Notify(other);
        }
        
        
    }

    private void RefreshBounds()
    {
        if(Parent ==  null) return;
        Rectangle parentRect = Parent._rect;
        if (!UseCustomBounds)
        {
            _rect= parentRect;
            return;
        }

        int offsetX = LocalBounds.X;
        bool flipped = (Parent._effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None;
        if (flipped)
            offsetX = parentRect.Width - LocalBounds.X - LocalBounds.Width;
        
        _rect = new Rectangle(parentRect.X + offsetX, parentRect.Y + LocalBounds.Y, LocalBounds.Width, LocalBounds.Height);
    }

    private void TurnOffCollider()
    {
        for (int i = 0; i < SceneManager.colliders.Count; i++)
        {
            Collider other = SceneManager.colliders[i];
            other.showDebugOutline = !other.showDebugOutline;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if(!IsEnabled || !showDebugOutline) return;
    #if DEBUG
        // draw outline bounds
        int thickness = 5;

        Color color = default;
        switch (colliderType)
        {
            case e_ColliderType.Body:
            {
                 color = Color.LawnGreen;
                break;
            }
            case e_ColliderType.Attack:
            {
                 color = Color.Orange;
                break;
            }
            case e_ColliderType.Vision:
            {
                 color = Color.DeepSkyBlue;
                break;
            }
            case e_ColliderType.Ground:
                color = Color.SaddleBrown;
                break;
            case e_ColliderType.Item:
                color = Color.HotPink;
                break;
        }
        
        // top
        spriteBatch.Draw(_texture,
            new Rectangle(
                _rect.X,
                _rect.Y,
                _rect.Width,
                thickness
            ), 
            color);
        
        // left
        spriteBatch.Draw(_texture,
            new Rectangle(
                _rect.X,
                _rect.Y,
                thickness,
                _rect.Height
            ), 
            color);
        
        // right
        spriteBatch.Draw(_texture,
            new Rectangle(
                _rect.X + _rect.Width - thickness,
                _rect.Y,
                thickness,
                _rect.Height
            ), 
            color);
        
        // bottom
        spriteBatch.Draw(_texture,
            new Rectangle(
                _rect.X,
                _rect.Y + _rect.Height - thickness,
                _rect.Width,
                thickness
            ), 
            color);
    #endif        
    }
}