using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public sealed class MerchantShopSystem : IDrawable, IUpdatable
{
    private const float OfferRange = 180f;
    private const float MerchantRange = 220f;
    private const float OfferScale = 1.25f;
    private const float OfferSize = 32f * OfferScale;
    private const float TableScaleX = 4f;
    private const float TableScaleY = 2.5f;
    private const float OfferGap = 16f;
    private const float MessageDuration = 3f;
    private const float OfferTextScale = 1.5f;
    private const float MessageTextScale = 1.5f;
    private const float MessageMaxWidth = 230f;
    private const string InsufficientFundsMessage =
        "You can't pay me for this one buddy";

    private readonly Player _player;
    private readonly SpriteFont _font;
    private readonly List<MerchantStand> _stands = new();
    private readonly List<Offer> _offers = new();
    private readonly Random _random = new();
    private readonly KeyboardPrompt _sellPrompt;
    private Offer _activeOffer;
    private MerchantStand _nearbyMerchant;

    private sealed class MerchantStand
    {
        public MerchantNpc Merchant { get; }
        public float MessageTimer { get; set; }
        public string Message { get; set; }

        public MerchantStand(MerchantNpc merchant)
        {
            Merchant = merchant;
        }
    }

    private sealed class Offer
    {
        public MerchantStand Stand { get; }
        public Item Definition { get; }
        public Sprite Icon { get; }
        public KeyboardPrompt Prompt { get; }
        public bool Sold { get; set; }

        public Offer(
            MerchantStand stand,
            Item definition,
            Sprite icon,
            KeyboardPrompt prompt)
        {
            Stand = stand;
            Definition = definition;
            Icon = icon;
            Prompt = prompt;
        }
    }

    public MerchantShopSystem(
        Player player,
        SpriteFont font,
        IReadOnlyList<MerchantSiteInfo> sites)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _font = font ?? throw new ArgumentNullException(nameof(font));
        _sellPrompt = new KeyboardPrompt(
            Keys.F,
            new Rectangle(0, 80, 16, 16),
            new Rectangle(16, 80, 16, 16),
            Vector2.Zero,
            3f,
            autoFlicker: true)
        {
            IsVisible = false
        };

        foreach (MerchantSiteInfo site in sites)
        {
            var merchant = new MerchantNpc(
                _player,
                site.MerchantGroundPosition);
            SceneManager.AddBefore(merchant, _player);

            var stand = new MerchantStand(merchant);
            _stands.Add(stand);

            AddTwoOffers(stand, site.TableGroundPosition);
        }
    }

    private void AddTwoOffers(
        MerchantStand stand,
        Vector2 tableGroundPosition)
    {
        string[] itemIds =
        {
            "Bomb_Pickup",
            "Health_Potion",
            "Stamina_Potion"
        };

        int first = _random.Next(itemIds.Length);
        int second = _random.Next(itemIds.Length - 1);
        if (second >= first)
            second++;

        float tableWidth = 32f * TableScaleX;
        float tableTop =
            tableGroundPosition.Y - 32f * TableScaleY;
        float firstX =
            tableGroundPosition.X +
            (tableWidth - (2f * OfferSize + OfferGap)) / 2f;
        float iconY = tableTop - OfferSize + 59f;

        AddOffer(stand, itemIds[first], firstX, iconY, true);
        AddOffer(
            stand,
            itemIds[second],
            firstX + OfferSize + OfferGap,
            iconY,
            false);
    }

    private void AddOffer(
        MerchantStand stand,
        string itemId,
        float x,
        float y,
        bool promptOnLeft)
    {
        Item definition = ItemFactory.Create(itemId);
        var icon = new Sprite(definition.SpriteName)
        {
            _sourceRectangle = definition._sourceRectangle
        };
        icon._transform._scale = new Vector2(OfferScale);
        icon._transform._position = new Vector2(x, y);
        SceneManager.AddBefore(icon, _player);

        float promptX = promptOnLeft
            ? x - 28f
            : x + OfferSize + 4f;
        var prompt = new KeyboardPrompt(
            Keys.E,
            new Rectangle(0, 64, 16, 16),
            new Rectangle(16, 64, 16, 16),
            new Vector2(promptX, y + 8f),
            2.25f,
            autoFlicker: true)
        {
            IsVisible = false
        };

        var offer = new Offer(stand, definition, icon, prompt);
        _offers.Add(offer);
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (MerchantStand stand in _stands)
        {
            if (stand.MessageTimer > 0f)
            {
                stand.MessageTimer = Math.Max(0f, stand.MessageTimer - elapsed);
                if (stand.MessageTimer == 0f)
                    stand.Message = null;
            }
        }

        _activeOffer = FindNearbyOffer();

        foreach (Offer offer in _offers)
        {
            bool isActive = ReferenceEquals(offer, _activeOffer);
            offer.Prompt.IsVisible = isActive;
            if (isActive)
                offer.Prompt.Update(gameTime);
        }

        _nearbyMerchant = FindNearbyMerchant();
        _sellPrompt.IsVisible = _nearbyMerchant != null;
        if (_nearbyMerchant != null)
        {
            MerchantNpc merchant = _nearbyMerchant.Merchant;
            float merchantCenterX =
                merchant._transform._position.X +
                merchant._sourceRectangle.Width *
                merchant._transform._scale.X / 2f;
            float promptWidth =
                _sellPrompt._sourceRectangle.Width *
                _sellPrompt._transform._scale.X;
            _sellPrompt._transform._position = new Vector2(
                merchantCenterX - promptWidth / 2f,
                merchant._transform._position.Y + 8f);
            _sellPrompt.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sellPrompt.Draw(spriteBatch);

        if (_activeOffer != null)
        {
            _activeOffer.Prompt.Draw(spriteBatch);

            float centerX =
                _activeOffer.Icon._transform._position.X +
                _activeOffer.Icon._sourceRectangle.Width * OfferScale / 2f;
            float iconY = _activeOffer.Icon._transform._position.Y;

            DrawCenteredOutlinedText(
                spriteBatch,
                _activeOffer.Definition.ItemDescription,
                centerX,
                iconY - 48f,
                Color.Black,
                Color.White,
                OfferTextScale);

            DrawCenteredOutlinedText(
                spriteBatch,
                $"Price: {_activeOffer.Definition.ItemPrice} coins",
                centerX,
                iconY - 28f,
                Color.Black,
                Color.White,
                OfferTextScale);
        }

        foreach (MerchantStand stand in _stands)
        {
            if (stand.MessageTimer > 0f && !string.IsNullOrEmpty(stand.Message))
                DrawStandMessage(spriteBatch, stand);
        }
    }

    private void DrawStandMessage(
        SpriteBatch spriteBatch,
        MerchantStand stand)
    {
        List<string> lines = WrapText(
            stand.Message,
            MessageMaxWidth,
            MessageTextScale);

        float lineHeight = _font.LineSpacing * MessageTextScale;
        float centerX =
            stand.Merchant._transform._position.X +
            stand.Merchant._sourceRectangle.Width *
            stand.Merchant._transform._scale.X / 2f;
        float firstY =
            stand.Merchant._transform._position.Y -
            lines.Count * lineHeight -
            8f +
            50f;

        for (int i = 0; i < lines.Count; i++)
        {
            DrawCenteredOutlinedText(
                spriteBatch,
                lines[i],
                centerX,
                firstY + i * lineHeight,
                Color.Red,
                Color.Black,
                MessageTextScale);
        }
    }

    private List<string> WrapText(
        string text,
        float maxWidth,
        float scale)
    {
        var lines = new List<string>();
        string currentLine = "";

        foreach (string word in text.Split(' '))
        {
            string candidate = string.IsNullOrEmpty(currentLine)
                ? word
                : currentLine + " " + word;

            if (!string.IsNullOrEmpty(currentLine) &&
                _font.MeasureString(candidate).X * scale > maxWidth)
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = candidate;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines;
    }

    private void DrawCenteredOutlinedText(
        SpriteBatch spriteBatch,
        string text,
        float centerX,
        float worldY,
        Color color,
        Color outlineColor,
        float scale)
    {
        Vector2 size = _font.MeasureString(text) * scale;
        Vector2 screenPosition = new Vector2(
            centerX - size.X / 2f,
            worldY) - Camera2D.Position;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                spriteBatch.DrawString(
                    _font,
                    text,
                    screenPosition + new Vector2(x, y),
                    outlineColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(scale),
                    SpriteEffects.None,
                    0f);
            }
        }

        spriteBatch.DrawString(
            _font,
            text,
            screenPosition,
            color,
            0f,
            Vector2.Zero,
            new Vector2(scale),
            SpriteEffects.None,
            0f);
    }

    private Offer FindNearbyOffer()
    {
        float playerCenterX =
            _player._transform._position.X +
            _player._sourceRectangle.Width *
            _player._transform._scale.X / 2f;
        float playerCenterY =
            _player._transform._position.Y +
            _player._sourceRectangle.Height *
            _player._transform._scale.Y / 2f;

        float bestDistanceSquared = OfferRange * OfferRange;
        Offer nearest = null;

        foreach (Offer offer in _offers)
        {
            if (offer.Sold)
                continue;

            float offerCenterX =
                offer.Icon._transform._position.X +
                offer.Icon._sourceRectangle.Width * OfferScale / 2f;
            float offerCenterY =
                offer.Icon._transform._position.Y +
                offer.Icon._sourceRectangle.Height * OfferScale / 2f;

            float dx = playerCenterX - offerCenterX;
            float dy = playerCenterY - offerCenterY;
            float distanceSquared = dx * dx + dy * dy;

            if (distanceSquared <= bestDistanceSquared)
            {
                bestDistanceSquared = distanceSquared;
                nearest = offer;
            }
        }

        return nearest;
    }

    public bool TryBuyNearby()
    {
        Offer offer = FindNearbyOffer();
        if (offer == null)
            return false;

        int price = offer.Definition.ItemPrice;
        if (_player.Inventory.GetQuantity("Gold_Coins") < price)
        {
            offer.Stand.Message = InsufficientFundsMessage;
            offer.Stand.MessageTimer = MessageDuration;
            return true;
        }

        if (!_player.Inventory.TryRemoveItem("Gold_Coins", price))
            return true;

        Item purchasedItem = ItemFactory.Create(offer.Definition.ItemID);
        _player.Inventory.AddItem(
            purchasedItem.ItemID,
            purchasedItem.ItemQuantity);
        _player.OnItemPickedUp(purchasedItem);
        _player.NotifyWalletChanged();

        offer.Sold = true;
        SceneManager.Remove(offer.Icon);
        offer.Stand.Message = null;
        offer.Stand.MessageTimer = 0f;
        _activeOffer = FindNearbyOffer();

        AudioManager.PlaySoundEffect("PickupItem");
        return true;
    }

    public bool IsNearMerchant()
    {
        return FindNearbyMerchant() != null;
    }

    private MerchantStand FindNearbyMerchant()
    {
        float playerCenterX =
            _player._transform._position.X +
            _player._sourceRectangle.Width *
            _player._transform._scale.X / 2f;
        float playerCenterY =
            _player._transform._position.Y +
            _player._sourceRectangle.Height *
            _player._transform._scale.Y / 2f;

        foreach (MerchantStand stand in _stands)
        {
            float merchantCenterX =
                stand.Merchant._transform._position.X +
                stand.Merchant._sourceRectangle.Width *
                stand.Merchant._transform._scale.X / 2f;
            float merchantCenterY =
                stand.Merchant._transform._position.Y +
                stand.Merchant._sourceRectangle.Height *
                stand.Merchant._transform._scale.Y / 2f;

            float dx = playerCenterX - merchantCenterX;
            float dy = playerCenterY - merchantCenterY;

            if (dx * dx + dy * dy <= MerchantRange * MerchantRange)
                return stand;
        }

        return null;
    }

    public bool TrySellHeldItem()
    {
        string itemId = _player.DisplayedInventoryItemId;
        if (string.IsNullOrEmpty(itemId))
            return false;

        Item item = ItemFactory.Create(itemId);
        if (!item.IsSellable || item.ItemType == Item.e_ItemType.Coin)
            return false;

        if (!_player.Inventory.TryRemoveItem(itemId, 1))
            return false;

        _player.Inventory.AddItem("Gold_Coins", item.ItemPrice);
        _player.NotifyWalletChanged();
        AudioManager.PlaySoundEffect("CoinUp", false, 0.5f);
        return true;
    }
}
