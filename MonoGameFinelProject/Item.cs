namespace MonoGameFinelProject;

public class Item : Sprite , IPickupable , ISellable
{
    public enum e_ItemType
    {
        None,
        Coin,
        Misc,
        Weapon,
        Potion
    }

    private string ItemName => ItemName; 
    private string ItemDescription => ItemDescription;
    private Sprite itemSprite;
    private e_ItemType ItemType => e_ItemType.None;
    private bool IsSellable => IsSellable;
    private int itemPrice => itemPrice;
    private int itemQuantity { get; set; }
    private string ItemID {get;set;}

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