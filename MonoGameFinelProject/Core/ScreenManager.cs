using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public sealed class ScreenManager
{
    private IGameScreen _currentScreen;
    private readonly List<IGameScreen> _overlays = new();
    
    public void ChangeScreen(IGameScreen nextScreen)
    {
        if (nextScreen == null)
            throw new ArgumentNullException(nameof(nextScreen));

        for (int i = _overlays.Count - 1; i >= 0; i--)
            _overlays[i].Exit();

        _overlays.Clear();
        _currentScreen?.Exit();

        _currentScreen = nextScreen;
        _currentScreen.Enter();
    }

    public void PushOverlay(IGameScreen overlay)
    {
        if (overlay == null)
            throw new ArgumentNullException(nameof(overlay));

        _overlays.Add(overlay);
        overlay.Enter();
    }

    public bool PopOverlay()
    {
        if (_overlays.Count == 0)
            return false;

        int top = _overlays.Count - 1;
        _overlays[top].Exit();
        _overlays.RemoveAt(top);
        return true;
    }

    public void Update(GameTime gameTime)
    {
        if (_overlays.Count > 0)
            _overlays[_overlays.Count - 1].Update(gameTime);
        else
            _currentScreen?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentScreen?.Draw(spriteBatch);

        foreach (IGameScreen overlay in _overlays)
            overlay.Draw(spriteBatch);
    }
}