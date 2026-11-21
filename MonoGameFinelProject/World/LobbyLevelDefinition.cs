using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGameFinelProject;

public static class LobbyLevelDefinition
{
    public static LevelDefinition Create()
    {
        var level = new LevelDefinition()
        {
            Purpose = LevelPurpose.Lobby,
            Id = "lobby-1",
            DisplayName = "Lobby 1",
            Difficulty = "Lobby",
            SectionWidth = 2400f,
            ChestPlacementY = 1100f,
            GroundY = 940f,
            PlayerLocalX = 120f,
            GoodKingLocalX = 300f,
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
        
        level.Sections.Add(new LevelSectionDefinition
        {
            Index = 0
        });
        
        return level;
    }

}