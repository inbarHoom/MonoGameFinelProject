using System;
using Microsoft.Xna.Framework;

namespace MonoGameFinelProject;

public static class EnemyLootSpawner
{
    public static void Spawn(Enemy enemy, string itemId)
    {
        if (enemy == null)
            throw new ArgumentNullException(nameof(enemy));

        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("An item ID is required.", nameof(itemId));

        Item item = ItemFactory.Create(itemId);

        float itemWidth = item._sourceRectangle.Width * item._transform._scale.X;
        float itemHeight = item._sourceRectangle.Height * item._transform._scale.Y;

        item._transform._position = new Vector2(enemy._rect.Center.X - itemWidth / 2f, enemy._rect.Bottom - (itemHeight * 1.5f));

        SceneManager.Add(item);
        item.CreateCollider();
    }
}