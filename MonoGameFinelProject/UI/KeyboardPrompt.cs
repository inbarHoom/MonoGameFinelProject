using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public class KeyboardPrompt : Sprite
{
    private readonly Keys key;
    private readonly Rectangle releasedSource;
    private readonly Rectangle heldSource;

    public bool IsVisible { get; set; } = true;
    public bool AutoFlicker { get; set; } = false;
    private float flickerTimer;
    private bool flickerState;
    private const float FlickerInterval = 0.12f;
        
    public KeyboardPrompt(
        Keys key,
        Rectangle releasedSource,
        Rectangle heldSource,
        Vector2 position,
        float scale,
        bool autoFlicker = false)
        : base("KeyboardABC")
        {
        this.key = key;
        this.releasedSource = releasedSource;
        this.heldSource = heldSource;
        this.AutoFlicker = autoFlicker;
        _transform._position = position;
        _transform._scale = new Vector2(scale);
        _sourceRectangle = releasedSource;
    }
    public KeyboardPrompt(int x,
        Keys key,
        Rectangle releasedSource,
        Rectangle heldSource,
        Vector2 position,
        float scale,
        bool autoFlicker = false)
        : base("KeyboardSymbols")
    {
        this.key = key;
        this.releasedSource = releasedSource;
        this.heldSource = heldSource;
        this.AutoFlicker = autoFlicker;
        _transform._position = position;
        _transform._scale = new Vector2(scale);
        _sourceRectangle = releasedSource;
    }
    

    public override void Update(GameTime gameTime)
    {
        if(!IsVisible) return;
        if (AutoFlicker)
        {
            flickerTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (flickerTimer >= FlickerInterval)
            {
                flickerTimer -= FlickerInterval;
                flickerState = !flickerState;
            }

            _sourceRectangle = flickerState ? heldSource : releasedSource;
        }
        else
        {
            bool isHeld = Keyboard.GetState().IsKeyDown(key);
            _sourceRectangle = isHeld ? heldSource : releasedSource;
        }
        base.Update(gameTime);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;
        base.Draw(spriteBatch);
    }
}