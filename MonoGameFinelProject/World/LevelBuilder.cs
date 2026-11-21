using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public class LevelBuilder
{
    public LevelManager Build(LevelDefinition definition, out List<MerchantSiteInfo> merchantSites)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (definition.SectionWidth <= 0f)
            throw new ArgumentOutOfRangeException(nameof(definition.SectionWidth));

        if (definition.Sections.Count == 0)
            throw new ArgumentException("A level must contain at least one section.", nameof(definition));

        merchantSites = new List<MerchantSiteInfo>();

        var levelManager = new LevelManager();
        var sectionsByIndex = new Dictionary<int, LevelSection>();

        for (int i = 0; i < definition.Sections.Count; i++)
        {
            LevelSectionDefinition sectionDefinition = definition.Sections[i];

            if (sectionDefinition.Index != i)
                throw new InvalidOperationException("Level sections must have sequential, zero-based indices.");

            float left = sectionDefinition.Index * definition.SectionWidth;
            var section = new LevelSection(left, left + definition.SectionWidth);

            sectionsByIndex.Add(sectionDefinition.Index, section);
            levelManager.AddSection(section);
        }

        var campsiteGenerator = new CampsiteGenerator();

        foreach (LevelSectionDefinition sectionDefinition in definition.Sections)
        {
            LevelSection section = sectionsByIndex[sectionDefinition.Index];

            foreach (float localX in sectionDefinition.ChestPositions)
            {
                Chest chest = SceneManager.Create<Chest>();
                PlaceChest(chest, section.Left + localX, definition.ChestPlacementY);
                section.Chests.Add(chest);
            }

            // Preserve the current behavior: merchant sections use merchant
            // campsites instead of randomly generated regular campsites.
            if (sectionDefinition.MerchantCampsiteStartPositions.Count == 0)
            {
                campsiteGenerator.GenerateInSection(section, definition.GroundY, sectionDefinition.CampsiteCount);
            }

            foreach (float localStartX in sectionDefinition.MerchantCampsiteStartPositions)
            {
                merchantSites.Add(campsiteGenerator.GenerateMerchantCampsite(section, definition.GroundY, section.Left + localStartX));
            }

            foreach (EnemySpawnDefinition enemy in sectionDefinition.Enemies)
            {
                SpawnEnemy(section, enemy, section.Left + enemy.LocalX);
            }
        }
     
        return levelManager;
    }

    private static void SpawnEnemy(LevelSection section, EnemySpawnDefinition spawn, float worldX)
    {
        SpriteEffects effects = spawn.Facing switch
        {
            FacingDirection.Left => SpriteEffects.FlipHorizontally,
            FacingDirection.Right => SpriteEffects.None,
            _ => throw new ArgumentOutOfRangeException(nameof(spawn.Facing))
        };

        switch (spawn.Kind)
        {
            case EnemyKind.Goblin:
                SpawnEnemy<Goblin>(section, worldX, effects);
                break;
            case EnemyKind.Knight:
                SpawnEnemy<Knight>(section, worldX, effects);
                break;
            case EnemyKind.FantasyWarrior:
                SpawnEnemy<FantasyWarrior>(section, worldX, effects);
                break;
            case EnemyKind.King:
                SpawnEnemy<King>(section, worldX, effects);
                break;
            case EnemyKind.DeathBringer:
                SpawnEnemy<DeathBringer>(section, worldX, effects);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(spawn.Kind));
        }
    }
    private static T SpawnEnemy<T>(LevelSection section, float worldX, SpriteEffects effects)
        where T : Enemy, new()
    {
        var enemy = SceneManager.Create<T>();

        float enemyWidth = enemy._sourceRectangle.Width * enemy._transform._scale.X;

        float safeWorldX = MathHelper.Clamp(worldX, section.Left, section.Right - enemyWidth);

        enemy._transform._position = new Vector2(safeWorldX, 530);
        enemy._effects = effects;
        enemy.CreateCollider();

        SceneManager.Add(new EnemyHealthBar(enemy));
        section.Enemies.Add(enemy);

        return enemy;
    }

    
    private static void PlaceChest(Chest chest, float worldX, float chestPlacementY)
    {
        float chestHeight = chest._sourceRectangle.Height * chest._transform._scale.Y;

        chest._transform._position = new Vector2(worldX, chestPlacementY - chestHeight);

        chest.CreateCollider();
    }
    
}