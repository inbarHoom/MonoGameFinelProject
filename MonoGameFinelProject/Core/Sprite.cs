namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Sprite : IDrawable , IUpdatable
{
    public Transform _transform { get; set; } = new();
    public Texture2D _texture { get; set; }
    public Color _color { get; set; } = Color.White;
    public Vector2 _origin { get; set; } = Vector2.Zero;
    public int sortingLayer { get; set; } = 0;
    public SpriteEffects _effects { get; set; } = SpriteEffects.None;
    private Rectangle source; 
    public bool UseCamera { get; set; } = true;
    
    public Rectangle _sourceRectangle
    {
        get
        {
            return source;
        }
        set
        {
            source = value;
            _rect = GetDestRectangle(source);
        }
    }
    public Rectangle _rect { get; set; }
    protected Spritesheet _spritesheet;

    public Sprite(string SpritenName)
    {
        _spritesheet = SpriteManager.GetSprite(SpritenName);
        _sourceRectangle = _spritesheet[0, 0];
        _texture = _spritesheet.texture;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (_texture == null) return;
        
        float zoom = UseCamera ? Camera2D.Zoom : 1f;
        
        Vector2 drawPosition = UseCamera
            ? new Vector2(
                (_transform._position.X - Camera2D.Position.X) * zoom,
                Game1.ScreenSize.Y / 2f
                + (_transform._position.Y - Game1.ScreenSize.Y / 2f) * zoom - 200)
            : _transform._position;
        
        Vector2 drawScale = _transform._scale * zoom;
        
        spriteBatch.Draw(_texture,
            drawPosition, 
            _sourceRectangle,
            _color,
            MathHelper.ToRadians(_transform._rotation),
            _origin, 
            drawScale, 
            _effects,
            sortingLayer
        );
    }
    public virtual void Update(GameTime gameTime)
    {
        _rect = GetDestRectangle(_sourceRectangle);
    }
    protected Rectangle GetDestRectangle(Rectangle rect)
    {
        int width = (int)(rect.Width * _transform._scale.X);
        int height = (int)(rect.Height * _transform._scale.Y);

        int pos_x = (int)(_transform._position.X - _origin.X * _transform._scale.X);
        int pos_y  = (int)(_transform._position.Y - _origin.Y * _transform._scale.Y);

        return new Rectangle(pos_x, pos_y, width, height);
    }
}