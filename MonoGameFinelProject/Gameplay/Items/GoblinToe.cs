
namespace MonoGameFinelProject;

public class GoblinToe : Item
{
    public GoblinToe() : base("GoblinToe")
    {
        ItemName = "Goblin Toe";
        ItemDescription = "A goblin toe.";
        ItemType = e_ItemType.Misc;
        IsSellable = true;
        ItemPrice = 2;
        ItemQuantity = 1;
        ItemID = "Goblin_Toe";
    }
}