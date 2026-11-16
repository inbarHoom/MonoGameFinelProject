using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace MonoGameFinelProject;

public class InventorySlotUI : IDrawable, IUpdatable
{
    private readonly Player player;
    private readonly Vector2 slotPosition;
    private readonly float slotSize;
    private readonly SpriteFont font;

    private readonly List<string> itemIdsInOrder = new();
    private int selectedIndex = -1;
    
    private Sprite itemIcon;
    private readonly Sprite tinyCoinIcon;
    private readonly Text priceText;
    private readonly Text quantityText;

    private string currentItemId;
    private int currentItemPrice;

    public InventorySlotUI(Player player, Vector2 slotPosition, float slotSize, SpriteFont font)
    {
        this.player = player;
        this.slotPosition = slotPosition;
        this.slotSize = slotSize;
        this.font = font;
        
        tinyCoinIcon = new Sprite("Money");
        tinyCoinIcon._transform._scale = new Vector2(0.45f);
        tinyCoinIcon._transform._position = new Vector2(
            slotPosition.X + 10,
            slotPosition.Y + slotSize - 25);
        tinyCoinIcon.UseCamera = false;
        priceText = new Text
        {
            _font = font,
            _textScale = new Vector2(1f),
            _color = Color.Gold,
            _text = ""
        };

        quantityText = new Text
        {
            _font = font,
            _textScale = new Vector2(1f),
            _color = Color.White,
            _text = ""
        };
        player.ItemPickedUp += OnPlayerPickedUpItem;
        player.InventoryCycled += OnInventoryCycled;
    }

    // shows the pickedup item in the ui
    private void OnPlayerPickedUpItem(Item item)
    {
        if(item == null || item.ItemType == Item.e_ItemType.Coin) return;
        if (!itemIdsInOrder.Contains(item.ItemID))
        {
            itemIdsInOrder.Add(item.ItemID);
        }
        selectedIndex = itemIdsInOrder.IndexOf(item.ItemID);
        RefreshDisplayedItem(item.ItemID);
    }
    private void OnInventoryCycled(int direction)
    {
        CleanupRemovedItems();
        if (itemIdsInOrder.Count == 0)
        {
            selectedIndex = -1;
            itemIcon = null;
            currentItemId = null;
            player.DisplayedInventoryItemId =  null;
            return;
        }
        selectedIndex = (selectedIndex + direction) % itemIdsInOrder.Count;
        if (selectedIndex < 0)
            selectedIndex += itemIdsInOrder.Count;

        RefreshDisplayedItem(itemIdsInOrder[selectedIndex]);
    }
    private void CleanupRemovedItems()
    {
        for (int i = itemIdsInOrder.Count - 1; i >= 0; i--)
        {
            if (!player.Inventory.Contains(itemIdsInOrder[i]))
            {
                itemIdsInOrder.RemoveAt(i);
            }
        }
    }
    private void RefreshDisplayedItem(string itemId)
    {
        currentItemId = itemId;
        player.DisplayedInventoryItemId = itemId;
        Item template = ItemFactory.Create(itemId);
        currentItemPrice = template.ItemPrice;

        itemIcon = new Sprite(template.SpriteName);
        itemIcon._sourceRectangle = template._sourceRectangle;
        float targetIconSize = slotSize * 0.55f;
        float scale = targetIconSize / 32f;
        itemIcon._transform._scale = new Vector2(scale);
        itemIcon._transform._position = new Vector2(
            slotPosition.X + (slotSize - targetIconSize) / 2f,
            slotPosition.Y + (slotSize - targetIconSize) / 2f - 6f);
        itemIcon.UseCamera = false;
    }

    public void Update(GameTime gameTime)
    {
        CleanupRemovedItems();
        if (itemIdsInOrder.Count == 0)
        {
            itemIcon = null;
            currentItemId = null;
            player.DisplayedInventoryItemId =  null;
            selectedIndex = -1;
            return;
        }
        if (string.IsNullOrEmpty(currentItemId) || !player.Inventory.Contains(currentItemId))
        {
            selectedIndex = MathHelper.Clamp(selectedIndex, 0, itemIdsInOrder.Count - 1);
            RefreshDisplayedItem(itemIdsInOrder[selectedIndex]);
        }

        int qty = player.Inventory.GetQuantity(currentItemId);

        itemIcon?.Update(gameTime);
        tinyCoinIcon.Update(gameTime);

        priceText._text = currentItemPrice.ToString();
        priceText.Update(gameTime);
        priceText._transform._position = new Vector2(
            tinyCoinIcon._transform._position.X + 25f,
            tinyCoinIcon._transform._position.Y + 10f);

        quantityText._text = $"x{qty}";
        quantityText.Update(gameTime);
        quantityText._transform._position = new Vector2(
            slotPosition.X + slotSize - 20f,
            slotPosition.Y + slotSize - 15f);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (itemIcon == null) return;

        itemIcon.Draw(spriteBatch);
        tinyCoinIcon.Draw(spriteBatch);
        priceText.Draw(spriteBatch);
        quantityText.Draw(spriteBatch);
    }
}