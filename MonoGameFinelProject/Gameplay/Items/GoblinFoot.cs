namespace MonoGameFinelProject;

public class GoblinFoot : Item
{
    public GoblinFoot() : base("GoblinFoot")
    {
        ItemName = "Goblin Foot";
        ItemDescription = "A goblin Foot";
        ItemType = e_ItemType.Misc;
        IsSellable = true;
        ItemPrice = 5;

        ItemQuantity = 1;
        ItemID = "Goblin_Foot";
    }
}