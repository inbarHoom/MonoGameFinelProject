using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public class KeyboardPrompt : Sprite
{
    private readonly Keys key;
    private readonly Rectangle releasedSource;
    private readonly Rectangle heldSource;

    public KeyboardPrompt(
        Keys key,
        Rectangle releasedSource,
        Rectangle heldSource,
        Vector2 position,
        float scale)
        : base("KeyboardABC")
    {
        this.key = key;
        this.releasedSource = releasedSource;
        this.heldSource = heldSource;

        _transform._position = position;
        _transform._scale = new Vector2(scale);
        _sourceRectangle = releasedSource;
    }

    public override void Update(GameTime gameTime)
    {
        bool isHeld = Keyboard.GetState().IsKeyDown(key);
        _sourceRectangle = isHeld ? heldSource : releasedSource;

        base.Update(gameTime);
    }
}