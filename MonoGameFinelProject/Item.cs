using Microsoft.Xna.Framework;

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
    private Collider pickupCollider;
    public bool PlayerInRange { get; private set; }

    protected Item(string SpritenName) : base(SpritenName)
    {
     //   _transform._scale = new Vector2(1.5f);
    }

    public void CreateCollider()
    {
        pickupCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = false,
            colliderType = Collider.e_ColliderType.Body,
            showDebugOutline = true
        };
        pickupCollider.onTrigger += OnPickupTrigger;
        SceneManager.Add(pickupCollider);
    }
    private void OnPickupTrigger(Collider self, Collider other)
    {
        if (other.colliderType == Collider.e_ColliderType.Body && other.Parent is Player)
        {
            PlayerInRange = true;
        }
    }
    public override void Update(GameTime gameTime)
    {
        PlayerInRange = false; 
        base.Update(gameTime);
    }

    public bool TryPickup(Player player)
    {
        if (player == null) return false;
        
        player.Inventory.AddItem(ItemID,ItemQuantity);
        if (pickupCollider != null)
        {
            pickupCollider.IsEnabled = false;
            SceneManager.Remove(pickupCollider);
        }
        SceneManager.Remove(this);
        return true;
    }

    public void Sell(Item item)
    {
        throw new System.NotImplementedException();
    }
}