using System.Collections.Generic;

namespace MonoGameFinelProject;

public enum EnemyKind
{
    Goblin,
    Knight,
    FantasyWarrior,
    King,
    DeathBringer
}

public enum FacingDirection
{
    Left,
    Right
}

public enum ParallaxPlacement
{
    Background,
    Foreground
}

public sealed class LevelDefinition
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Difficulty { get; set; }

    public float SectionWidth { get; set; } = 2400f;
    
    public float ChestPlacementY { get; set; } = 1100f;
    public float GroundY { get; set; } = 940f;

    public List<ParallaxDefinition> ParallaxLayers { get; } = new();
    public List<LevelSectionDefinition> Sections { get; } = new();

    public float WorldWidth => SectionWidth * Sections.Count;
}

public sealed class ParallaxDefinition
{
    public string SpriteName { get; set; }
    public float Y { get; set; }
    public float Scale { get; set; }
    public float Parallax { get; set; }
    public float? BottomY { get; set; }
    public ParallaxPlacement Placement { get; set; }
}

public sealed class LevelSectionDefinition
{
    // Zero-based: 0 means the first section.
    public int Index { get; set; }

    // Positions are relative to the section's left edge.
    public List<float> ChestPositions { get; } = new();
    public List<EnemySpawnDefinition> Enemies { get; } = new();

    public int CampsiteCount { get; set; }
    public List<float> MerchantCampsiteStartPositions { get; } = new();
    
    
}

public sealed class EnemySpawnDefinition
{
    public EnemyKind Kind { get; set; }
    public float LocalX { get; set; }
    public FacingDirection Facing { get; set; }

    public EnemySpawnDefinition(
        EnemyKind kind,
        float localX,
        FacingDirection facing = FacingDirection.Left)
    {
        Kind = kind;
        LocalX = localX;
        Facing = facing;
    }
}