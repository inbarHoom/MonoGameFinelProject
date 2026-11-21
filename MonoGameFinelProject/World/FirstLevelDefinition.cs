using System.Collections.Specialized;

namespace MonoGameFinelProject;

public static class FirstLevelDefinition
{
    public static LevelDefinition Create()
    {
        var level = new LevelDefinition()
        {
            Purpose = LevelPurpose.Gameplay,
            Id = "level-1",
            DisplayName = "Level 1",
            Difficulty = "Normal",
            SectionWidth = 2400f,
            ChestPlacementY = 1100f,
            GroundY = 940f
        };
        
        level.ParallaxLayers.Add(new ParallaxDefinition
        {
            SpriteName = "TerrainSky",
            Y = 0f, Scale = 1f, Parallax = 0f,
            Placement = ParallaxPlacement.Background
        });
        level.ParallaxLayers.Add(new ParallaxDefinition
        {
            SpriteName = "TerrainHills",
            Y = 350f, Scale = 1f, Parallax = 0.2f,
            Placement = ParallaxPlacement.Background
        });
        level.ParallaxLayers.Add(new ParallaxDefinition
        {
            SpriteName = "TerrainBackgroundTrees",
            Y = 370f, Scale = 1f, Parallax = 0.3f,
            Placement = ParallaxPlacement.Background
        });
        level.ParallaxLayers.Add(new ParallaxDefinition
        {
            SpriteName = "TerrainGround",
            Y = 900f, Scale = 0.3f, Parallax = 1.5f,
            Placement = ParallaxPlacement.Background
        });
        level.ParallaxLayers.Add(new ParallaxDefinition
        {
            SpriteName = "TerrainForegroundFoliage",
            Y = 1010f, Scale = 0.2f, Parallax = 1.2f,
            BottomY = 1080f,
            Placement = ParallaxPlacement.Foreground
        });
        
            var s0 = Section(0, campsites: 2);
        s0.ChestPositions.Add(1200f);
        Enemy(s0, EnemyKind.Goblin, 700f, FacingDirection.Right);
        Enemy(s0, EnemyKind.Goblin, 1600f);
        Enemy(s0, EnemyKind.Goblin, 1900f);

        var s1 = Section(1, campsites: 2);
        s1.ChestPositions.Add(2100f); // World X 4500.
        Enemy(s1, EnemyKind.Goblin, 600f);
        Enemy(s1, EnemyKind.Knight, 1150f);
        Enemy(s1, EnemyKind.Goblin, 250f, FacingDirection.Right);
        Enemy(s1, EnemyKind.Goblin, 1700f);

        var s2 = Section(2);
        s2.MerchantCampsiteStartPositions.Add(820f);

        var s3 = Section(3, campsites: 2);
        s3.ChestPositions.Add(200f); // World X 7400.
        s3.ChestPositions.Add(550f); // World X 7750.
        Enemy(s3, EnemyKind.Goblin, 850f, FacingDirection.Right);
        Enemy(s3, EnemyKind.Knight, 1120f);
        Enemy(s3, EnemyKind.Goblin, 1390f);
        Enemy(s3, EnemyKind.Knight, 1660f);
        Enemy(s3, EnemyKind.FantasyWarrior, 1930f);
        Enemy(s3, EnemyKind.Goblin, 2200f, FacingDirection.Right);

        var s4 = Section(4);
        s4.MerchantCampsiteStartPositions.Add(820f);

        var s5 = Section(5, campsites: 2);
        s5.ChestPositions.Add(1200f); // World X 13200.
        s5.ChestPositions.Add(1400f); // World X 13400.
        Enemy(s5, EnemyKind.Goblin, 250f, FacingDirection.Right);
        Enemy(s5, EnemyKind.Goblin, 550f);
        Enemy(s5, EnemyKind.Knight, 850f);
        Enemy(s5, EnemyKind.FantasyWarrior, 1000f);
        Enemy(s5, EnemyKind.Knight, 1700f);
        Enemy(s5, EnemyKind.Goblin, 2000f);
        Enemy(s5, EnemyKind.Goblin, 2250f, FacingDirection.Right);

        var s6 = Section(6, campsites: 2);
        s6.ChestPositions.Add(100f); // World X 14500.
        Enemy(s6, EnemyKind.Goblin, 400f, FacingDirection.Right);
        Enemy(s6, EnemyKind.Goblin, 700f);
        Enemy(s6, EnemyKind.Knight, 1000f);
        Enemy(s6, EnemyKind.FantasyWarrior, 1300f);
        Enemy(s6, EnemyKind.Goblin, 1600f);
        Enemy(s6, EnemyKind.Knight, 1900f);
        Enemy(s6, EnemyKind.King, 2250f, FacingDirection.Right);
        Enemy(s6, EnemyKind.DeathBringer, 2370f, FacingDirection.Right);

        level.Sections.AddRange(new[] { s0, s1, s2, s3, s4, s5, s6 });
        return level;
    }
    private static LevelSectionDefinition Section(
        int index,
        int campsites = 0)
    {
        return new LevelSectionDefinition
        {
            Index = index,
            CampsiteCount = campsites
        };
    }

    private static void Enemy(
        LevelSectionDefinition section,
        EnemyKind kind,
        float localX,
        FacingDirection facing = FacingDirection.Left)
    {
        section.Enemies.Add(new EnemySpawnDefinition(kind, localX, facing));
    }
}