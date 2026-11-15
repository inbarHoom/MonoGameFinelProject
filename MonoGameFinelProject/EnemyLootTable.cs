using System;

namespace MonoGameFinelProject;

public static class EnemyLootTable
{
    private static readonly Random Random = new();

    private static readonly (string ItemId, int Weight)[] EliteDrops =
    {
        ("Bomb_Pickup", 1),
        ("Gold_Coins", 1),

        // Tomorrow, add potion IDs and weights here, for example:
        // ("Potion_Health", 1),
    };

    public static string RollEliteDropItemId()
    {
        int totalWeight = 0;
        foreach (var drop in EliteDrops)
            totalWeight += drop.Weight;

        int roll = Random.Next(totalWeight);

        foreach (var drop in EliteDrops)
        {
            if (roll < drop.Weight)
                return drop.ItemId;

            roll -= drop.Weight;
        }

        throw new InvalidOperationException("Enemy drop weights are invalid.");
    }
}