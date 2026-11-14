namespace MonoGameFinelProject;

public class Item : Sprite , IPickupable , ISellable
{
    public enum e_ItemType
    {
        Coin,
        Misc,
        Weapon,
        Potion
    }
    private string itemName { get; set; }
    private string itemDescription { get; set; }
    private e_ItemType itemType { get; set; }
    private bool isSellable { get; set; }
    private int itemPrice { get; set; }
    private int itemQuantity { get; set; }

    protected Item(string SpritenName) : base(SpritenName)
    {
    }

    public bool TryPickup(Player player)
    {
        throw new System.NotImplementedException();
    }

    public void Sell(Item item)
    {
        throw new System.NotImplementedException();
    }
}