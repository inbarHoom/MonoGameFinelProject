namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using System;

public class Camera2D
{
    public static Vector2 Position { get; private set; }

    public static void Follow(Player player, float levelWidth, float screenWidth)
    {
        if (player == null)
            return;

        float desiredX = player._transform._position.X - screenWidth / 2f;
        float maxX = Math.Max(0f, levelWidth - screenWidth);

        Position = new Vector2(
            MathHelper.Clamp(desiredX, 0f, maxX), 0f);
    }

    public static void Reset()
    {
        Position = Vector2.Zero;
    }
}