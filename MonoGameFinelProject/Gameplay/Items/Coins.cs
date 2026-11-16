namespace MonoGameFinelProject;

public class Coins : Item
{
    public Coins() : base("Money")
    {
        ItemName = "Gold Coins";
        ItemDescription = "10 coins of gold";
        ItemType = e_ItemType.Coin;
        IsSellable = true;
        ItemPrice = 1;

        ItemQuantity = 10;
        ItemID = "Gold_Coins";
    }
}