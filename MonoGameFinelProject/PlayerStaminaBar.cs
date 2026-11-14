using Microsoft.Xna.Framework;
namespace MonoGameFinelProject;

public class PlayerStaminaBar : Sprite
{
    private readonly Player player;
    private readonly float fullScale;

    public PlayerStaminaBar(Player player, Vector2 framePosition, float uiScale)
        : base("StaminaBar")
    {
        this.player = player;
        fullScale = uiScale;

        _sourceRectangle = new Rectangle(31, 8, 71, 3);
        _transform._position =
            framePosition + new Vector2(31 * uiScale, 18 * uiScale);
    }

    public override void Update(GameTime gameTime)
    {
        float staminaRatio = player.MaxStamina > 0
            ? player.CurrentStamina / player.MaxStamina
            : 0f;

        staminaRatio = MathHelper.Clamp(staminaRatio, 0f, 1f);
        _transform._scale = new Vector2(fullScale * staminaRatio, fullScale);

        base.Update(gameTime);
    }
}