using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace MonoGameFinelProject;

public class LevelSection
{
    public float Left { get; }
    public float Right { get; }

    public List<Enemy> Enemies { get; } = new();
    public List<Chest> Chests { get; } = new();

    public LevelSection(float left, float right)
    {
        Left = left;
        Right = right;
    }

    public bool IsCleared()
    {
        if (Enemies.Count == 0)
            return true;

        foreach (Enemy enemy in Enemies)
        {
            if (enemy != null && !enemy.IsDead)
                return false;
        }

        return true;
    }
    
}