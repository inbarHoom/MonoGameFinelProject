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
        
        if (player == null || _sections.Count == 0)
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

      
        const float boundaryInset = 80f;

        float playerWidth = player._sourceRectangle.Width * player._transform._scale.X;
        float minX = _sections[0].Left + boundaryInset;
        float maxX = _sections[_sections.Count - 1].Right - boundaryInset - playerWidth;

        if (CurrentSection != null && !CurrentSection.IsCleared())
        {
            float sectionMinX = CurrentSection.Left + boundaryInset;
            float sectionMaxX = CurrentSection.Right - boundaryInset;

            if (sectionMinX > minX)
                minX = sectionMinX;

            if (sectionMaxX < maxX)
                maxX = sectionMaxX;
        }

        Vector2 position = player._transform._position;
        position.X = MathHelper.Clamp(position.X, minX, maxX);
        player._transform._position = position;
   
    }
}