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
    private readonly string _label ;
    private readonly SpriteFont _font;
    private readonly Action _onClick;
    private readonly float _textScale;
    private ButtonState _previousLeftButton = ButtonState.Released;
    private bool _isHovered;
    private readonly Texture2D _iconTexture;
    private readonly Rectangle _iconSource;
    private readonly bool _hasIcon;
    
    public MenuButton(
        Rectangle bounds,
        string label,
        SpriteFont font,
        Action onClick,
        Texture2D iconTexture,
        Rectangle iconSource)
    {
        _bounds = bounds;
        _label = label;
        _font = font;
        _onClick = onClick;
        _iconTexture = iconTexture;
        _iconSource = iconSource;
        _hasIcon = true;
        
        Spritesheet borderSheet = SpriteManager.GetSprite("BorderSheet");
        _borderTexture = borderSheet.texture;
        _borderSource = borderSheet[3, 0]; // fourth column, top row
    }

    public MenuButton(
        Rectangle bounds,
        string label,
        SpriteFont font,
        Action onClick,
        float textScale = 1f)
    {
        _bounds = bounds;
        _label = label;
        _font = font;
        _onClick = onClick;
        _textScale = textScale;
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
        spriteBatch.Draw(_borderTexture, _bounds, _borderSource, tint);
        if (_hasIcon)
        {
            var iconBounds = new Rectangle(
                _bounds.X + 20,
                _bounds.Y + 20,
                _bounds.Width - 40,
                _bounds.Height - 40);

            spriteBatch.Draw(_iconTexture, iconBounds, _iconSource, tint);
        }
        else
        {
            spriteBatch.Draw(
                _borderTexture,
                _bounds,
                _borderSource,
                tint);

       
            Vector2 textSize = _font.MeasureString(_label) * _textScale;
            Vector2 textPosition = new Vector2(
                _bounds.Center.X - textSize.X/2 ,
                _bounds.Center.Y - textSize.Y/2 + 5);

            spriteBatch.DrawString(
                _font,
                _label,
                textPosition,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(_textScale),
                SpriteEffects.None,
                0f);
        }
      
    }
}