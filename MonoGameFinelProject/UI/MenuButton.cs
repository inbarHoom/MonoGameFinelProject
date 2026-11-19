using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public sealed class MenuButton
{
    private readonly Texture2D _borderTexture;
    private readonly Rectangle _borderSource;
    private readonly Rectangle _bounds;
    private readonly string _label;
    private readonly SpriteFont _font;
    private readonly Action _onClick;
    private ButtonState _previousLeftButton = ButtonState.Released;
    private bool _isHovered;

    public MenuButton(
        Rectangle bounds,
        string label,
        SpriteFont font,
        Action onClick)
    {
        _bounds = bounds;
        _label = label;
        _font = font;
        _onClick = onClick;

        Spritesheet borderSheet = SpriteManager.GetSprite("BorderSheet");
        _borderTexture = borderSheet.texture;
        _borderSource = borderSheet[3, 0]; // fourth column, top row
    }

    public void Update()
    {
        MouseState mouse = Mouse.GetState();
        _isHovered = _bounds.Contains(mouse.Position);

        bool justPressed =
            mouse.LeftButton == ButtonState.Pressed &&
            _previousLeftButton == ButtonState.Released;

        if (_isHovered && justPressed)
            _onClick?.Invoke();

        _previousLeftButton = mouse.LeftButton;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color tint = _isHovered ? Color.White : Color.LightGray;

        spriteBatch.Draw(
            _borderTexture,
            _bounds,
            _borderSource,
            tint);

        Vector2 textSize = _font.MeasureString(_label);
        Vector2 textPosition = new Vector2(
            _bounds.Center.X - textSize.X / 2f,
            _bounds.Center.Y - textSize.Y / 2f);

        spriteBatch.DrawString(
            _font,
            _label,
            textPosition,
            Color.White);
    }
}