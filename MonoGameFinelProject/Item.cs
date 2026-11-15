using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public string SpriteName { get; }
    private Sprite itemSprite{ get; set; }
    private e_ItemType itemType = e_ItemType.None;
    private bool isSellable { get; set; }
    private int itemPrice { get; set; }
    private int itemQuantity { get; set; }
    private string itemID {get;set;}
 
    public float MinPickupGapPixels { get; set; } = 0f;
    public float MaxPickupGapPixels { get; set; } = 40f;
    
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
    private readonly KeyboardPrompt ePrompt;
    private bool touchedThisFrame ;
    public bool PlayerInRange { get; private set; }

    protected Item(string SpritenName) : base(SpritenName)
    {
        SpriteName = SpritenName;
        _transform._scale = new Vector2(1.5f);
        
        ePrompt = new KeyboardPrompt(
            Keys.E,
            new Rectangle(0, 64, 16, 16),
            new Rectangle(16, 64, 16, 16),
            Vector2.Zero,
            2.5f,
            autoFlicker: true)
        {
            IsVisible = false
        };
    }

    public void CreateCollider()
    {
        _rect = GetDestRectangle(_sourceRectangle);
        
        pickupCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = false,
            colliderType = Collider.e_ColliderType.Item,
            showDebugOutline = false
        };
        pickupCollider.onTrigger += OnPickupTrigger;
        SceneManager.Add(pickupCollider);
    }
    private void OnPickupTrigger(Collider self, Collider other)
    {
        if (other.colliderType == Collider.e_ColliderType.Body && other.Parent is Player)
        {
            touchedThisFrame = true;
            PlayerInRange = true;
        }
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        PlayerInRange = false;
        if (pickupCollider != null && pickupCollider.IsEnabled)
        {
            for (int i = 0; i < SceneManager.colliders.Count; i++)
            {
                Collider other = SceneManager.colliders[i];
                if (other.IsEnabled && other.colliderType == Collider.e_ColliderType.Body &&
                    other.Parent is Player && other.Parent is Player player && IsInPickupRange(player))
                {
                    PlayerInRange = true;
                    break;
                }
            }
        }
        ePrompt.IsVisible = PlayerInRange;
        if (PlayerInRange)
        {
            ePrompt._transform._position = new Vector2(_rect.Left - 15, _rect.Top - 30);
            ePrompt.Update(gameTime);
        }
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        ePrompt.Draw(spriteBatch);
    }

    public bool TryPickup(Player player)
    {
        if (player == null) return false;
        
        player.Inventory.AddItem(ItemID,ItemQuantity);
        player.OnItemPickedUp(this);
        if (pickupCollider != null)
        {
            pickupCollider.IsEnabled = false;
            SceneManager.Remove(pickupCollider);
        }
        SceneManager.Remove(this);
        return true;
    }
    public bool IsInPickupRange(Player player)
    {
        if (player == null || player.BodyCollider == null ||
            pickupCollider == null || !pickupCollider.IsEnabled)
        {
            return false;
        }

        Rectangle body = player.BodyCollider._rect;
        Rectangle item = pickupCollider._rect;
        float dx = item.Left > body.Right ? item.Left - body.Right
            : body.Left > item.Right ? body.Left - item.Right
            : 0f;

        float dy = item.Top > body.Bottom ? item.Top - body.Bottom
            : body.Top > item.Bottom ? body.Top - item.Bottom
            : 0f;
        float distanceSquared = dx * dx + dy * dy;
        float minSquared = MinPickupGapPixels * MinPickupGapPixels;
        float maxSquared = MaxPickupGapPixels * MaxPickupGapPixels;

        return distanceSquared >= minSquared &&
               distanceSquared <= maxSquared;
    }

    public void Sell(Item item)
    {
        throw new System.NotImplementedException();
    }
}