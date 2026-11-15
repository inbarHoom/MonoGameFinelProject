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
        
        if (player == null)
            return;
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

        if (CurrentSection == null || CurrentSection.IsCleared())
            return;
        const float boundaryInset = 80f;

        float minX = CurrentSection.Left + boundaryInset;
        float maxX = CurrentSection.Right - boundaryInset;

        Vector2 position = player._transform._position;
        position.X = MathHelper.Clamp(position.X, minX, maxX);
        player._transform._position = position;
   
    }
}