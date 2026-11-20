using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public sealed class MainMenuScreen : IGameScreen
{
    private readonly Texture2D _background;
    private readonly MenuButton _playButton;
    private readonly MenuButton _settingsButton;
    private readonly MenuButton _quitButton;

    public MainMenuScreen(
        SpriteFont font,
        Action onPlay,
        Action onSettings,
        Action onQuit)
    {
        _background = SpriteManager.GetSprite("MainMenuBackground").texture;

        const int buttonWidth = 320;
        const int buttonHeight = 80;
        const int gap = 20;

        int x = ((int)Game1.ScreenSize.X - buttonWidth) / 2;
        int firstY = ((int)Game1.ScreenSize.Y -
                      (buttonHeight * 3 + gap * 2)) / 2;

        _playButton = new MenuButton(
            new Rectangle(x, firstY, buttonWidth, buttonHeight),
            "Play",
            font,
            onPlay,
            1.8f);

        _settingsButton = new MenuButton(
            new Rectangle(x, firstY + buttonHeight + gap, buttonWidth, buttonHeight),
            "Settings",
            font,
            onSettings,
            1.8f);

        _quitButton = new MenuButton(
            new Rectangle(x, firstY + (buttonHeight + gap) * 2, buttonWidth, buttonHeight),
            "Quit",
            font,
            onQuit,
            1.8f);
    }

    public void Enter() { }

    public void Update(GameTime gameTime)
    {
        _playButton.Update();
        _settingsButton.Update();
        _quitButton.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _background,
            new Rectangle(
                0,
                0,
                (int)Game1.ScreenSize.X,
                (int)Game1.ScreenSize.Y),
            Color.White);

        _playButton.Draw(spriteBatch);
        _settingsButton.Draw(spriteBatch);
        _quitButton.Draw(spriteBatch);
    }

    public void Exit() { }
}