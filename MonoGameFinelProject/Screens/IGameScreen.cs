using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public interface IGameScreen
{
    void Enter();
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
    void Exit();
}