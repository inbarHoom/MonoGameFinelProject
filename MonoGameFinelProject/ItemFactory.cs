namespace MonoGameFinelProject;

public static class ItemFactory
{
    public static Item Create(string itemId)
    {
        return itemId switch
        {
            "Bomb_Pickup" => new BombPickUp(),
            "Goblin_Toe"  => new GoblinToe(),
            "Goblin_Ear"  => new GoblinEar(),
            "Goblin_Hand" => new GoblinHand(),
            "Goblin_Foot" => new GoblinFoot(),
            "Gold_Coins"       => new Coins(),

            _ => throw new System.ArgumentException(
                $"Unknown item ID: {itemId}",
                nameof(itemId))
        };
    }
}