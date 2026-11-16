using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;    
namespace MonoGameFinelProject;

public class PlayerHealthBar : Sprite
{
    private readonly Player player;
    private readonly float fullScale;
    private readonly Sprite damageTrail;

    private float trailRatio = 1f;
    private const float TrailFollowSpeed = 3f;
    public PlayerHealthBar(Player player, Vector2 framePosition, float uiScale) : base("HealthBar")
    {
        this.player = player;
        fullScale = uiScale;
        UseCamera = false;
       
        
        var barCrop = new Rectangle(31, 8, 71, 3);
        _sourceRectangle = barCrop;

        _transform._position = framePosition + new Vector2(31 * uiScale, 8 * uiScale);
        damageTrail = new Sprite("BackgroundBar");
        damageTrail._sourceRectangle = barCrop;
        damageTrail._transform._position = _transform._position;
        damageTrail._transform._scale = new Vector2(uiScale, uiScale);
        damageTrail.UseCamera = false;
    }

    public override void Update(GameTime gameTime)
    {
        float healthRatio = player.MaxHealth > 0 ? (float)player.CurrentHealth / player.MaxHealth : 0f;

        healthRatio = MathHelper.Clamp(healthRatio, 0f, 1f);

        _transform._scale = new Vector2(fullScale * healthRatio, fullScale);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float blend = 1f - MathF.Exp(-TrailFollowSpeed * dt);
        trailRatio = MathHelper.Lerp(trailRatio, healthRatio, blend);
        
        damageTrail._transform._scale = new Vector2(fullScale * trailRatio, fullScale);
        damageTrail.Update(gameTime);
        base.Update(gameTime);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        damageTrail.Draw(spriteBatch);
        base.Draw(spriteBatch);
    }
}