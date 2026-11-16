using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public sealed class MerchantNpc : Animation
{
    private readonly Player _player;

    public MerchantNpc(Player player, Vector2 groundPosition)
        : base("Merchant")
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));

        const float scale = 2f;
        const float verticalOffset = 100f;
        _transform._scale = new Vector2(scale);
        _transform._position = new Vector2(
            groundPosition.X,
            groundPosition.Y - _sourceRectangle.Height * scale + verticalOffset);

        PlayAnimation(true, 8);
    }

    public override void Update(GameTime gameTime)
    {
        float merchantCenterX =
            _transform._position.X +
            _sourceRectangle.Width * _transform._scale.X / 2f;
        float playerCenterX =
            _player._transform._position.X +
            _player._sourceRectangle.Width * _player._transform._scale.X / 2f;

        _effects = playerCenterX < merchantCenterX
            ? SpriteEffects.FlipHorizontally
            : SpriteEffects.None;

        base.Update(gameTime);
    }
}
