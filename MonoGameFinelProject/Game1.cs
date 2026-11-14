using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace MonoGameFinelProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static Vector2 ScreenSize = new Vector2(1920, 1080);
    SpriteManager _spriteManager = null;
    AudioManager _audioManager = null;
    private SpriteFont dungeonFont;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.PreferredBackBufferWidth = 1920;

        ScreenSize.X = _graphics.PreferredBackBufferWidth;
        ScreenSize.Y = _graphics.PreferredBackBufferHeight;
        _graphics.IsFullScreen = true;
    }

    public static bool GameOver { get; set; }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteManager = new SpriteManager(Content);
        _audioManager = new AudioManager(Content); 
        _spriteBatch = new SpriteBatch(GraphicsDevice);
// UI
    // Fonts
        dungeonFont = Content.Load<SpriteFont>("Fonts/DungeonFont");
    //Abilities
        SpriteManager.AddSprite("LightAttackUI", "UI/Abilities/4");
        SpriteManager.AddSprite("HeavyAttackUI", "UI/Abilities/5"); 
    //Hud
        SpriteManager.AddSprite("BackgroundBar", "UI/ui_bar5");
        SpriteManager.AddSprite("HealthBar", "UI/ui_bar2");
        SpriteManager.AddSprite("StaminaBar", "UI/ui_bar6");
        SpriteManager.AddSprite("BarUI", "UI/ui_front_border5");
        SpriteManager.AddSprite("ItemSlot", "UI/ui_select_item3");
        SpriteManager.AddSprite("PickupIcon", "UI/Icon3");
        
        SpriteManager.AddSprite("KeyboardABC", "UI/kb_dark_alphanumeric");
// Items
        SpriteManager.AddSprite("ItemsSheet", "SpriteSheets/Items/items 32x32");
        SpriteManager.AddSprite("GoblinToe", "SpriteSheets/Items/Icon2");
        SpriteManager.AddSprite("GoblinEar", "SpriteSheets/Items/Icon3");
        SpriteManager.AddSprite("GoblinHand", "SpriteSheets/Items/Icon5");
        SpriteManager.AddSprite("GoblinFoot", "SpriteSheets/Items/Icon6");
        SpriteManager.AddSprite("Money", "SpriteSheets/Items/Icon41");
        SpriteManager.AddSprite("BombPickup", "SpriteSheets/Items/Icon49");
// Collider
        SpriteManager.AddSprite("Pixel", "Images/pixel");
// Player
        SpriteManager.AddSprite("MartialHeroIdle", "SpriteSheets/MartialHero/Idle", 1, 8);
        SpriteManager.AddSprite("MartialHeroRun", "SpriteSheets/MartialHero/Run", 1, 8);
        SpriteManager.AddSprite("MartialHeroAttack1", "SpriteSheets/MartialHero/Attack1", 1, 6);
        SpriteManager.AddSprite("MartialHeroAttack2", "SpriteSheets/MartialHero/Attack2", 1, 6);
        SpriteManager.AddSprite("MartialHeroJump", "SpriteSheets/MartialHero/Jump", 1, 2);
        SpriteManager.AddSprite("MartialHeroFall", "SpriteSheets/MartialHero/Fall", 1, 2);
        SpriteManager.AddSprite("MartialHeroHit", "SpriteSheets/MartialHero/Take Hit - white silhouette", 1, 4);
        SpriteManager.AddSprite("MartialHeroDeath", "SpriteSheets/MartialHero/Death", 1, 6);
// Bomb
        SpriteManager.AddSprite("Bomb", "SpriteSheets/Goblin/Bomb_sprite", 1, 19);
// Enemies:
// Goblin
        SpriteManager.AddSprite("GoblinIdle", "SpriteSheets/Goblin/Idle", 1, 4);
        SpriteManager.AddSprite("GoblinRun", "SpriteSheets/Goblin/Run", 1, 8);
        SpriteManager.AddSprite("GoblinAttack1", "SpriteSheets/Goblin/Attack", 1, 8);
        SpriteManager.AddSprite("GoblinAttack2", "SpriteSheets/Goblin/Attack2", 1, 8);
        SpriteManager.AddSprite("GoblinAttack3", "SpriteSheets/Goblin/Attack3", 1, 12);
        SpriteManager.AddSprite("GoblinHit", "SpriteSheets/Goblin/Take Hit", 1, 4);
        SpriteManager.AddSprite("GoblinDeath", "SpriteSheets/Goblin/Death", 1, 4);
// Mimic
        SpriteManager.AddSprite("ChestClosed", "SpriteSheets/Mimic/idle_closed");
        SpriteManager.AddSprite("ChestOpen", "SpriteSheets/Mimic/idle_open");
        SpriteManager.AddSprite("ChestOpening", "SpriteSheets/Mimic/opening",1,6);
        SpriteManager.AddSprite("MimicAttack1", "SpriteSheets/Mimic/attack_1" ,1,14);
        SpriteManager.AddSprite("MimicAttack2", "SpriteSheets/Mimic/attack_2" ,1,13);
        SpriteManager.AddSprite("MimicDeath", "SpriteSheets/Mimic/death" ,1,6);
        SpriteManager.AddSprite("MimicHurt", "SpriteSheets/Mimic/hurt",1,3);
        SpriteManager.AddSprite("MimicIdle", "SpriteSheets/Mimic/idle_transformed",1,9);
        SpriteManager.AddSprite("MimicTransform", "SpriteSheets/Mimic/transform",1,7);
        SpriteManager.AddSprite("MimicWalk", "SpriteSheets/Mimic/walk",1,6);
        
        Init();
    }

    void Init()
    {
        var floor = new Sprite("Pixel");
        floor._transform._position = new Vector2(0, 900);
        floor._transform._scale = new Vector2(ScreenSize.X, 20);
        floor._color = Color.Yellow;
        SceneManager.Add(floor);

        var floorCollider = new Collider
        {
            Parent = floor,
            colliderType = Collider.e_ColliderType.Ground,
            showDebugOutline = true
        };
        SceneManager.Add(floorCollider);
        
       var player = SceneManager.Create<Player>();
       var goblin = SceneManager.Create<Goblin>();
       
       player.CreateCollider();
       goblin.CreateCollider(); 
     
       const float uiScale = 3.5f;
       const int uiX = 15;
       const int uiY = 15;

       

       SceneManager.Add(new PlayerHealthBar(player, new Vector2(uiX, uiY), uiScale));
       
       var staminaBackground = new Sprite("BackgroundBar");
       staminaBackground._sourceRectangle = new Rectangle(31, 8, 71, 3);
       staminaBackground._transform._position =
           new Vector2(uiX + 31 * uiScale, uiY + 18 * uiScale);
       staminaBackground._transform._scale = new Vector2(uiScale, uiScale);
       SceneManager.Add(staminaBackground);

       SceneManager.Add(new PlayerStaminaBar(player, new Vector2(uiX, uiY), uiScale));
       
       var barUI = new Sprite("BarUI");
       barUI._transform._position = new Vector2(uiX, uiY);
       barUI._transform._scale = new Vector2(uiScale, uiScale);
       SceneManager.Add(barUI);
       
       var coinIcon = new Sprite("Money");
       coinIcon._transform._position = new Vector2(
           uiX + 31 * uiScale,
           uiY + 32 * uiScale -10);
       coinIcon._transform._scale = new Vector2(1.2f);
       SceneManager.Add(coinIcon);

       Text coinCount = new Text();
        coinCount._transform._position = new Vector2(
            coinIcon._transform._position.X * 1.5f, coinIcon._transform._position.Y + 30);
        coinCount._font = dungeonFont;
        coinCount._textScale = new Vector2(2.5f, 2.5f);
        coinCount._text = "0";
       
        SceneManager.Add(coinCount);
       
       const float itemSlotScale = 3f;
       const float iconScale = 0.3f;
       const int bottomMargin = 24;
       const int leftMargin = 60;
       const int slotGap = 10;

       float slotWidth = 32 * itemSlotScale;   
       float slotHeight = 32 * itemSlotScale;
       float iconSize = 256 * iconScale;       
        float slotSize = 32 * itemSlotScale;
        const float attackSlotGap = 20f;
        const float rightMargin = 50f;

        float slotY = ScreenSize.Y - slotSize - bottomMargin;
        float leftSlotX = leftMargin; // light attack back
        float rightSlotX = leftSlotX + slotSize + attackSlotGap; // heavy back
        float pickupSlotX = rightSlotX + slotSize + attackSlotGap; // pick up back

        float inventorySlotX = ScreenSize.X - slotSize - rightMargin;
        
       var leftSlot = new Sprite("ItemSlot");
       leftSlot._transform._position = new Vector2(leftSlotX, slotY);
       leftSlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(leftSlot);

       var rightSlot = new Sprite("ItemSlot");
       rightSlot._transform._position = new Vector2(rightSlotX, slotY);
       rightSlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(rightSlot);
       
       var pickupSlot = new Sprite("ItemSlot");
       pickupSlot._transform._position = new Vector2(pickupSlotX, slotY);
       pickupSlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(pickupSlot);
       
       var inventorySlot = new Sprite("ItemSlot");
       inventorySlot._transform._position = new Vector2(inventorySlotX, slotY);
       inventorySlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(inventorySlot);

       var lightIcon = new Sprite("LightAttackUI");
       lightIcon._transform._position= new Vector2(leftSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       lightIcon._transform._scale = new Vector2(iconScale);
       SceneManager.Add(lightIcon);

       var heavyIcon = new Sprite("HeavyAttackUI");
       heavyIcon._transform._position = new Vector2(rightSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       heavyIcon._transform._scale = new Vector2(iconScale);
       SceneManager.Add(heavyIcon);

       float pickupIconScale = iconSize / 32f;
       
       var pickupIcon = new Sprite("PickupIcon");
       pickupIcon._transform._position = new Vector2(pickupSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       pickupIcon._transform._scale = new Vector2(pickupIconScale);
       SceneManager.Add(pickupIcon);
       
       Vector2 lightIconPosition = new Vector2(
           leftSlotX + (slotWidth - iconSize) / 2, 
           slotY + (slotHeight - iconSize) / 2);

       Vector2 heavyIconPosition = new Vector2(
           rightSlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);
       
       float keyScale = 4f;
       Vector2 keyOffset = new Vector2(-30, -30);
       
      // float keyInsetX = 13 * itemSlotScale;
     //  float keyInsetY = 5f;

       // Vector2 pickupKeyPosition = new Vector2(
       //     pickupSlotX + keyInsetX + keyInsetY,
       //     slotY + keyInsetY);
       //
       // Vector2 inventoryKeyPosition = new Vector2(
       //     inventorySlotX + keyInsetX + keyInsetY,
       //     slotY + keyInsetY);
       
       Vector2 pickupSlotPosition = new Vector2(pickupSlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);
       Vector2 inventorySlotPosition = new Vector2( inventorySlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);


       Vector2  lightKeyPosition = new Vector2(
           (int)MathF.Round(lightIconPosition.X + keyOffset.X),
           (int)MathF.Round(lightIconPosition.Y + keyOffset.Y));
       
       Vector2  heavyKeyPosition = new Vector2(
           (int)MathF.Round(heavyIconPosition.X + keyOffset.X),
           (int)MathF.Round(heavyIconPosition.Y + keyOffset.Y));
       
       SceneManager.Add(new KeyboardPrompt(
           Keys.J,
           new Rectangle(0, 144, 16, 16), // J released
           new Rectangle(16, 144, 16, 16), // J held
           lightKeyPosition,
           keyScale));

       SceneManager.Add(new KeyboardPrompt(
           Keys.K,
           new Rectangle(0, 160, 16, 16), // K released
           new Rectangle(16, 160, 16, 16), // K held
           heavyKeyPosition,
           keyScale));
       
       SceneManager.Add(new KeyboardPrompt(
           Keys.E,
           new Rectangle(0, 64, 16, 16), // E released
           new Rectangle(16, 64, 16, 16), // E held
           pickupSlotPosition + keyOffset,
           keyScale));

       SceneManager.Add(new KeyboardPrompt(
           Keys.F,
           new Rectangle(0, 80, 16, 16), // F released
           new Rectangle(16, 80, 16, 16), // F held
           inventorySlotPosition + keyOffset,
           keyScale));
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit(); 
        if(!GameOver)
            SceneManager.Instance.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        SceneManager.Instance.Draw(_spriteBatch);
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
