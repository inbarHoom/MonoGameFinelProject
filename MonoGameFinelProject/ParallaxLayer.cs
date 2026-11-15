namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ParallaxLayer : IDrawable
{
    private readonly Texture2D _texture;
    private readonly float _y;
    private readonly float _scale;
    private readonly float _parallax;

    public ParallaxLayer(string spriteName, float y, float scale, float parallax)
    {
        _texture = SpriteManager.GetSprite(spriteName).texture;
        _y = y;
        _scale = scale;
        _parallax = parallax;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float tileWidth = _texture.Width * _scale;
        float tileHeight = _texture.Height * _scale;

        if (tileWidth <= 0f)
            return;

        float offset = Camera2D.Position.X * _parallax;
        float firstX = -(offset % tileWidth);

        for (float x = firstX; x < Game1.ScreenSize.X; x += tileWidth)
        {
            var destination = new Rectangle(
                (int)x,
                (int)_y,
                (int)tileWidth,
                (int)tileHeight);

            spriteBatch.Draw(_texture, destination, Color.White);
        }
    }
}