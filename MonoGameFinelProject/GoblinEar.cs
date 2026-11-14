namespace MonoGameFinelProject;

public class GoblinEar : Item
{
    public GoblinEar() : base("GoblinEar")
    {
        ItemName = "Goblin Ear";
        ItemDescription = "A goblin Ear";
        ItemType = e_ItemType.Misc;
        IsSellable = true;
        ItemPrice = 2;
        ItemQuantity = 1;
        ItemID = "Goblin_Ear";
    }
}