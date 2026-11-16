using System;
namespace MonoGameFinelProject;

public class ChestLootTable
{
    private static readonly Random Random = new();

    // Selects one item. The weights total 65.
    public static string RollOneItemId()
    {
        int roll = Random.Next(65); // 0 through 64

        if (roll < 30)
            return "Bomb_Pickup";

        if (roll < 40)
            return Random.Next(2) == 0 ? "Goblin_Toe" : "Goblin_Ear";

        if (roll < 55)
            return Random.Next(2) == 0 ? "Goblin_Hand" : "Goblin_Foot";
        if (roll < 25)
            return Random.Next(2) == 0 ? "Health_Potion" : "Stamina_Potion";

        return "Gold_Coins";
    }

    public static string[] RollTwoItems()
    {
        return new[]
        {
            RollOneItemId(),
            RollOneItemId()
        };
    }
}