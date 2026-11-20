using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public class VolumeSlider
{
     private readonly Rectangle _bounds;
    private readonly string _label;
    private readonly SpriteFont _font;
    private readonly Texture2D _pixel;
    private readonly Action<float> _onValueChanged;

    private float _value;
    private bool _isDragging;
    private ButtonState _previousLeftButton = ButtonState.Released;

    public VolumeSlider(
        Rectangle bounds,
        string label,
        float initialValue,
        SpriteFont font,
        Action<float> onValueChanged)
    {
        _bounds = bounds;
        _label = label;
        _font = font;
        _onValueChanged = onValueChanged;
        _pixel = SpriteManager.GetSprite("Pixel").texture;
        _value = MathHelper.Clamp(initialValue, 0f, 1f);
    }

    public void Update()
    {
        MouseState mouse = Mouse.GetState();

        bool justPressed =
            mouse.LeftButton == ButtonState.Pressed &&
            _previousLeftButton == ButtonState.Released;

        Rectangle hitBounds = new Rectangle(
            _bounds.X - 10,
            _bounds.Y - 20,
            _bounds.Width + 20,
            _bounds.Height + 40);

        if (justPressed && hitBounds.Contains(mouse.Position))
            _isDragging = true;

        if (_isDragging && mouse.LeftButton == ButtonState.Pressed)
        {
            _value = MathHelper.Clamp(
                (mouse.X - _bounds.Left) / (float)_bounds.Width,
                0f,
                1f);

            _onValueChanged?.Invoke(_value);
        }

        if (mouse.LeftButton == ButtonState.Released)
            _isDragging = false;

        _previousLeftButton = mouse.LeftButton;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_pixel, _bounds, Color.DimGray);

        var filledBounds = new Rectangle(
            _bounds.X,
            _bounds.Y,
            (int)(_bounds.Width * _value),
            _bounds.Height);
        spriteBatch.Draw(_pixel, filledBounds, Color.Goldenrod);

        int knobX = _bounds.X + (int)(_bounds.Width * _value) - 8;
        var knobBounds = new Rectangle(
            knobX,
            _bounds.Y - 6,
            16,
            _bounds.Height + 12);
        spriteBatch.Draw(_pixel, knobBounds, Color.White);

        const float textScale = 2f;
        string text = $"{_label}: {(int)(_value * 100)}%";

        Vector2 textPosition = new Vector2(
            _bounds.X,
            _bounds.Y - _font.LineSpacing * textScale - 8f);

        Vector2 scale = new Vector2(textScale);

// Draw the white outline around the text.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                spriteBatch.DrawString(
                    _font, text, textPosition + new Vector2(x, y),
                    Color.Black, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
            }
        }

// Draw the black text on top.
        spriteBatch.DrawString(
            _font, text, textPosition,
            Color.White, 0f, Vector2.Zero, scale,
            SpriteEffects.None, 0f);
        
        
    }
}