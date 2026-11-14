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

    private string itemName{ get; set; }
    private string itemDescription { get; set; }
    private Sprite itemSprite{ get; set; }
    private e_ItemType itemType = e_ItemType.None;
    private bool isSellable { get; set; }
    private int itemPrice { get; set; }
    private int itemQuantity { get; set; }
    private string itemID {get;set;}

    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    public string ItemDescription 
    {
        get { return itemDescription; }
        set { itemDescription = value; }
    }
    public Sprite ItemSprite
    {
        get { return itemSprite; }
        set { itemSprite = value; }
    }
    public e_ItemType ItemType 
    {
        get { return itemType; }
        set { itemType = value; }
    }
    public bool IsSellable
    {
        get { return isSellable; }
        set { isSellable = value; }
    }
    public int ItemPrice 
    {
        get { return itemPrice; }
        set { itemPrice = value; }
    }
    public int ItemQuantity 
    {
        get { return itemQuantity; }
        set { itemQuantity = value; }
    }
    public string ItemID 
    {
        get { return itemID; }
        set { itemID = value; }
    }

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