namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using System;

public class Camera2D
{
    public static Vector2 Position { get; private set; }
    public static float Zoom { get; set; } = 1.5f;

    public static void Follow(Player player, float levelWidth, float screenWidth)
    {
        if (player == null)
            return;

        float visibleWorldWidth = screenWidth / Zoom;
        float desiredX = player._transform._position.X - visibleWorldWidth / 2f;
        float maxX = Math.Max(0f, levelWidth - visibleWorldWidth);

        Position = new Vector2(MathHelper.Clamp(desiredX, 0f, maxX), 0f);
    }

    public static void Reset()
    {
        Position = Vector2.Zero;
    }
}