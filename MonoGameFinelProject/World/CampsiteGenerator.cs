using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace MonoGameFinelProject;

public sealed class CampsiteGenerator
{
    private const float SiteWidth = 760f;
    private const float PropScale = 2f;
    private const float TableScale = 2.5f;
    private const int LayoutCount = 14;

    // Source rectangles in Decor.png
    private static readonly Rectangle TentA = new(0, 32, 96, 64);
    private static readonly Rectangle TentB = new(96, 32, 96, 64);
    private static readonly Rectangle BoxA = new(0, 0, 32, 32);
    private static readonly Rectangle BoxB = new(32, 0, 32, 32);
    private static readonly Rectangle BarrelA = new(64, 0, 32, 32);
    private static readonly Rectangle BarrelB = new(96, 0, 32, 32);
    private static readonly Rectangle Stool = new(128, 0, 32, 32);
    private static readonly Rectangle Table = new(256, 0, 32, 32);
    private static readonly Rectangle Chair = new(288, 0, 32, 32);

    private readonly Random _random = new();
    private int _previousLayout = -1;
    

    public void GenerateInSection(  LevelSection section, float groundY, int siteCount)
    {
        if (siteCount <= 0)
            return;

        const float minimumGap = 400f;
        const float maxJitter = 20f;
        
        float sectionWidth = section.Right - section.Left;
        float spacing = SiteWidth + minimumGap + 2f * maxJitter;
        float totalSiteWidth = SiteWidth + (siteCount - 1) * spacing + 2f * maxJitter;

        if (totalSiteWidth > sectionWidth)
        {
            throw new ArgumentException(
                "There is not enough room for the requested campsites.");
        }

        float firstX = section.Left + (sectionWidth - totalSiteWidth) / 2f + maxJitter;

        for (int i = 0; i < siteCount; i++)
        {
            float jitter = (float)(_random.NextDouble() * 2 - 1) * maxJitter;
            float startX = firstX + i * spacing + jitter;
            if (OverlapsChest(startX, section.Chests))
                continue;
            GenerateOne(startX, groundY);
        }
    }

    public MerchantSiteInfo GenerateMerchantCampsite(
        LevelSection section,
        float groundY)
    {
        float x = section.Left + (section.Right - section.Left - SiteWidth) / 2f;

        if (OverlapsChest(x, section.Chests))
        {
            throw new InvalidOperationException(
                "The merchant campsite overlaps a chest.");
        }

        AddAtlasProp(TentA, x, groundY, PropScale);

        float tableX = x + 220f;
        var table = new Sprite("DecorAtlas")
        {
            _sourceRectangle = Table
        };
        AddAtGround(table, tableX, groundY, new Vector2(4f, 2.5f));

        AddAtlasProp(Stool, tableX + 130f, groundY, PropScale);
        AddFire("CampfireWithFood", x + 660f, groundY);

        return new MerchantSiteInfo(
            new Vector2(x + 360f, groundY),
            new Vector2(tableX, groundY));
    }
    
    private static bool OverlapsChest(float siteStartX, List<Chest> chests)
    {
        const float chestClearance = 100f;
        float siteEndX = siteStartX + SiteWidth;

        foreach (Chest chest in chests)
        {
            float chestStartX = chest._transform._position.X;
            float chestEndX =
                chestStartX +
                chest._sourceRectangle.Width * chest._transform._scale.X;

            if (siteStartX < chestEndX + chestClearance &&
                siteEndX > chestStartX - chestClearance)
            {
                return true;
            }
        }

        return false;
    }

    private void GenerateOne(float x, float groundY)
    {
        int layout = _random.Next(LayoutCount);

        // Avoid using the same template for two consecutive campsites.
        if (layout == _previousLayout)
            layout = (layout + 1 + _random.Next(LayoutCount - 1)) % LayoutCount;

        _previousLayout = layout;

        switch (layout)
        {
            case 0:
                // Tent left, table, two boxes together, fire right.
                AddAtlasProp(TentA, x, groundY, PropScale);
                AddTable(x + 210, groundY);
                AddAtlasProp(Stool, x + 305, groundY, PropScale);
                AddBoxPair(x + 390, groundY);
                AddFire("Campfire", x + 660, groundY);
                break;

            case 1:
                // Cooking fire left, tent right, barrels in between.
                AddFire("CampfireWithFood", x, groundY);
                AddAtlasProp(BarrelA, x + 85, groundY, PropScale);
                AddAtlasProp(BarrelB, x + 150, groundY, PropScale);
                AddTable(x + 260, groundY);
                AddAtlasProp(Chair, x + 355, groundY, PropScale);
                AddAtlasProp(TentB, x + 550, groundY, PropScale);
                break;

            case 2:
                // Tents on both sides with cooking fire in the middle.
                AddAtlasProp(TentA, x, groundY, PropScale);
                AddAtlasProp(Table, x + 220, groundY, TableScale);
                AddAtlasProp(Stool, x + 315, groundY, PropScale);
                AddFire("CampfireWithFood", x + 420, groundY);
                AddAtlasProp(TentB, x + 550, groundY, PropScale);
                break;

            case 3:
                // No tent: table, paired boxes, cooking fire.
                AddTable(x + 80, groundY);
                AddAtlasProp(Chair, x + 175, groundY, PropScale);
                AddBoxPair(x + 280, groundY);
                AddAtlasProp(BarrelA, x + 440, groundY, PropScale);
                AddFire("CampfireWithFood", x + 660, groundY);
                break;

            case 4:
                // Just a fire and stool, with the fire on the left.
                AddFire("Campfire", x, groundY);
                AddAtlasProp(Stool, x + 100, groundY, PropScale);
                break;

            case 5:
                // Just the cooking-pot fire and stool, reversed.
                AddAtlasProp(Stool, x + 540, groundY, PropScale);
                AddFire("CampfireWithFood", x + 660, groundY);
                break;

            case 6:
                // No tent: table and chair, with fire on the left.
                AddFire("CampfireWithFood", x, groundY);
                AddTable(x + 170, groundY);
                AddAtlasProp(Chair, x + 265, groundY, PropScale);
                AddAtlasProp(BarrelA, x + 390, groundY, PropScale);
                AddBoxPair(x + 470, groundY);
                break;

            case 7:
                // Tent on the right, no boxes, regular fire on the left.
                AddFire("Campfire", x, groundY);
                AddAtlasProp(BarrelA, x + 100, groundY, PropScale);
                AddTable(x + 230, groundY);
                AddAtlasProp(Stool, x + 325, groundY, PropScale);
                AddAtlasProp(TentB, x + 550, groundY, PropScale);
                break;

            case 8:
                // Small tent with a cooking fire on the left.
                AddFire("CampfireWithFood", x, groundY);
                AddAtlasProp(BarrelB, x + 100, groundY, PropScale);
                AddSprite("SmallTent", x + 260, groundY, PropScale);
                AddTable(x + 420, groundY);
                AddAtlasProp(Chair, x + 515, groundY, PropScale);
                break;

            case 9:
                // No tent or boxes: two fires with a table between them.
                AddFire("Campfire", x, groundY);
                AddTable(x + 170, groundY);
                AddAtlasProp(Stool, x + 265, groundY, PropScale);
                AddFire("CampfireWithFood", x + 430, groundY);
                break;

            case 10:
                // Tent left, cooking fire right, no boxes.
                AddAtlasProp(TentB, x, groundY, PropScale);
                AddTable(x + 220, groundY);
                AddAtlasProp(Chair, x + 315, groundY, PropScale);
                AddAtlasProp(BarrelA, x + 425, groundY, PropScale);
                AddFire("CampfireWithFood", x + 660, groundY);
                break;

            case 11:
                // No tent: two boxes and regular fire on the right.
                AddAtlasProp(Stool, x + 70, groundY, PropScale);
                AddTable(x + 155, groundY);
                AddAtlasProp(Chair, x + 250, groundY, PropScale);
                AddBoxPair(x + 370, groundY);
                AddFire("Campfire", x + 660, groundY);
                break;

            case 12:
                // Tent on the right, paired boxes and cooking fire on the left.
                AddFire("CampfireWithFood", x, groundY);
                AddBoxPair(x + 100, groundY);
                AddTable(x + 270, groundY);
                AddAtlasProp(Stool, x + 365, groundY, PropScale);
                AddAtlasProp(TentA, x + 550, groundY, PropScale);
                break;

            default:
                // No tent or boxes: barrels, table, stool, cooking fire.
                AddAtlasProp(BarrelA, x + 70, groundY, PropScale);
                AddAtlasProp(BarrelB, x + 135, groundY, PropScale);
                AddTable(x + 260, groundY);
                AddAtlasProp(Stool, x + 355, groundY, PropScale);
                AddFire("CampfireWithFood", x + 660, groundY);
                break;
        }
    }

    private static void AddBoxPair(float x, float groundY)
    {
        AddAtlasProp(BoxA, x, groundY, PropScale);
        AddAtlasProp(BoxB, x + 64, groundY, PropScale);
    }

    private static void AddTable(float x, float groundY)
    {
        AddAtlasProp(Table, x, groundY, TableScale);
    }

    private static void AddAtlasProp(
        Rectangle source,
        float x,
        float groundY,
        float scale)
    {
        var sprite = new Sprite("DecorAtlas")
        {
            _sourceRectangle = source
        };

        AddAtGround(sprite, x, groundY, scale);
    }

    private static void AddSprite(
        string spriteName,
        float x,
        float groundY,
        float scale)
    {
        AddAtGround(new Sprite(spriteName), x, groundY, scale);
    }

    private static void AddFire(string spriteName, float x, float groundY)
    {
        var fire = new Animation(spriteName);
        AddAtGround(fire, x, groundY, PropScale);
        fire.PlayAnimation(true, 10);
    }

    private static void AddAtGround(
        Sprite sprite,
        float x,
        float groundY,
        float scale)
    {
        AddAtGround(sprite, x, groundY, new Vector2(scale));
    }

    private static void AddAtGround(
        Sprite sprite,
        float x,
        float groundY,
        Vector2 scale)
    {
        sprite._transform._scale = scale;
        sprite._transform._position = new Vector2(
            x,
            groundY - sprite._sourceRectangle.Height * scale.Y);

        SceneManager.Add(sprite);
    }
}

public readonly record struct MerchantSiteInfo(
    Vector2 MerchantGroundPosition,
    Vector2 TableGroundPosition);