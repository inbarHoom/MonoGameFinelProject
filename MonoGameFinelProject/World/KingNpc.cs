using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public class KingNpc : Animation
{
    private readonly Player _player;
    
    public KingNpc(Player player, Vector2 groundPosition) : base("GoodKing")
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));

        const float scale = 1.5f;
        _transform._scale = new Vector2(scale);
        _transform._position = new Vector2(
            groundPosition.X,
            groundPosition.Y - _sourceRectangle.Height * _transform._scale.Y);
        PlayAnimation(true, 6);
    }
    public override void Update(GameTime gameTime)
    {
        float GoodKingCenterX =
            _transform._position.X +
            _sourceRectangle.Width * _transform._scale.X / 2f;
        float playerCenterX =
            _player._transform._position.X +
            _player._sourceRectangle.Width * _player._transform._scale.X / 2f;

        _effects = playerCenterX < GoodKingCenterX
            ? SpriteEffects.FlipHorizontally
            : SpriteEffects.None;

        base.Update(gameTime);
    }
}