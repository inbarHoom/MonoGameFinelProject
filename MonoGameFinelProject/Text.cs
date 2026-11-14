namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Text : IDrawable , IUpdatable
{
    public Transform _transform { get; set; } = new();
    public string _text { get; set; }

    public Vector2 _textSize { get; set; } = Vector2.Zero;
    
    public Vector2 _textScale { get; set; } = Vector2.One;
    
    public Color _color { get; set; } = Color.White;
    
    public SpriteFont _font { get; set; } 

    public virtual void Update(GameTime gameTime)
    {
        if (_text == null)  return;
        
        _textSize = _font.MeasureString(_text);
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            _font,
            _text,
            _transform._position,
            _color,
            MathHelper.ToRadians(_transform._rotation),
            _textSize * 0.5f,
            _textScale, 
            SpriteEffects.None,
            0);
    }

}