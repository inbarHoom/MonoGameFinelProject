namespace MonoGameFinelProject;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class LevelManager
{
    public const float SectionWidth = 2400f;
    private readonly List<LevelSection> _sections = new();

    public LevelSection CurrentSection { get; private set; }

    public void AddSection(LevelSection section)
    {
        _sections.Add(section);
    }

    public void Update(Player player)
    {
        CurrentSection = null;

        foreach (LevelSection section in _sections)
        {
            bool playerInsideSection =
                player._transform._position.X >= section.Left &&
                player._transform._position.X <= section.Right;

            if (playerInsideSection)
            {
                CurrentSection = section;
                break;
            }
        }

        if (CurrentSection == null)
            return;

        float minX = CurrentSection.Left + 80f;
        float maxX = CurrentSection.Right - 80f;

        float clampedX = MathHelper.Clamp(player._transform._position.X, minX, maxX);
        player._transform._position = new Vector2(clampedX, player._transform._position.Y);

        if (CurrentSection.IsCleared())
            CurrentSection.Unlock();
        else
            CurrentSection.Lock();
    }
}