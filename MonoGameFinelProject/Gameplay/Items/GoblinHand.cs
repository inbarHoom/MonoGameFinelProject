namespace MonoGameFinelProject;

public class GoblinHand : Item
{
    public GoblinHand() : base("GoblinHand")
    {
        ItemName = "Goblin Hand";
        ItemDescription = "A goblin Hand.";
        ItemType = e_ItemType.Misc;
        IsSellable = true;
        ItemPrice = 5;

        ItemQuantity = 1;
        ItemID = "Goblin_Hand";
    }
}