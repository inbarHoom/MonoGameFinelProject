using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameFinelProject;

public class EnemyHealthBar : IDrawable, IUpdatable
{
    private readonly Enemy _enemy;
    private readonly Sprite _background;
    private readonly Sprite _fill;
    private readonly Sprite _frame;

    private const int SourceWidth = 71;
    private const int SourceHeight = 3;
    private const float BarWidth = 90f;
    private const float BarHeight = 8f;
    private const float GapAboveEnemy = 2f;

    public EnemyHealthBar(Enemy enemy)
    {
        _enemy = enemy;

        _background = new Sprite("BackgroundBar");
        _fill = new Sprite("HealthBar");
        _frame = new Sprite("EnemyHealthFrame");

        var barSource = new Rectangle(31, 8, SourceWidth, SourceHeight);
        _background._sourceRectangle = barSource;
        _fill._sourceRectangle = barSource;
    }

    public void Update(GameTime gameTime)
    {
        if (_enemy.MaxHealth <= 0)
            return;

        float healthRatio = MathHelper.Clamp((float)_enemy.CurrentHealth / _enemy.MaxHealth, 0f, 1f);

        float x = _enemy._rect.Center.X - BarWidth / 2f;
        float y = _enemy._rect.Top - GapAboveEnemy - BarHeight;

        Vector2 position = new Vector2(x, y);
        Vector2 fullScale = new Vector2(BarWidth / SourceWidth, BarHeight / SourceHeight);

        _background._transform._position = position;
        _background._transform._scale = fullScale;

        _fill._transform._position = position;
        _fill._transform._scale = new Vector2(
            fullScale.X * healthRatio,
            fullScale.Y);

        _background.Update(gameTime);
        _fill.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_enemy.IsDead)
            return;
        
        _background.Draw(spriteBatch);
        _fill.Draw(spriteBatch);
        _frame.Draw(spriteBatch);
    }
}