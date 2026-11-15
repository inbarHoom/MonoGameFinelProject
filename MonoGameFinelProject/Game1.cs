using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace MonoGameFinelProject;

public class Game1 : Game
{
    public const float LevelWidth = 9600f;
    private Player _player;
    private readonly LevelManager _levelManager = new();
    private const float FloorY = 1100;
    
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
        SpriteManager.AddSprite("EnemyHealthFrame", "UI/boss_health_frame");
        
        SpriteManager.AddSprite("KeyboardABC", "UI/kb_dark_alphanumeric");
        SpriteManager.AddSprite("KeyboardSymbols", "UI/kb_dark_symbols");
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
// DeathBringer
        SpriteManager.AddSprite("DeathBringerSheet", "SpriteSheets/DeathBringer/Bringer-of-Death-SpritSheet", 8, 8);
// FantasyWarrior
        SpriteManager.AddSprite("FantasyWarriorAttack1", "SpriteSheets/FantasyWarrior/Attack1" , 1 ,7);
        SpriteManager.AddSprite("FantasyWarriorAttack2", "SpriteSheets/FantasyWarrior/Attack2" , 1 ,7);
        SpriteManager.AddSprite("FantasyWarriorAttack3", "SpriteSheets/FantasyWarrior/Attack3" , 1 ,8);
        SpriteManager.AddSprite("FantasyWarriorDeath", "SpriteSheets/FantasyWarrior/Death" , 1 ,7);
        SpriteManager.AddSprite("FantasyWarriorIdle", "SpriteSheets/FantasyWarrior/Idle" , 1 ,10);
        SpriteManager.AddSprite("FantasyWarriorRun", "SpriteSheets/FantasyWarrior/Run" , 1 ,8);
        SpriteManager.AddSprite("FantasyWarriorHit", "SpriteSheets/FantasyWarrior/Take hit" , 1 ,3);
// King
        SpriteManager.AddSprite("KingAttack1", "SpriteSheets/King/Attack1" , 1 ,4);
        SpriteManager.AddSprite("KingAttack2", "SpriteSheets/King/Attack2" , 1 ,4);
        SpriteManager.AddSprite("KingAttack3", "SpriteSheets/King/Attack3" , 1 ,4);
        SpriteManager.AddSprite("KingDeath", "SpriteSheets/King/Death" , 1 ,6);
        SpriteManager.AddSprite("KingIdle", "SpriteSheets/King/Idle" , 1 ,8);
        SpriteManager.AddSprite("KingRun", "SpriteSheets/King/Run" , 1 ,8);
        SpriteManager.AddSprite("KingHit", "SpriteSheets/King/Take Hit - white silhouette" , 1 ,4);
// Knight
        SpriteManager.AddSprite("KnightAttack1", "SpriteSheets/Knight/Attack1" , 1 ,7);
        SpriteManager.AddSprite("KnightAttack2", "SpriteSheets/Knight/Attack2" , 1 ,7);
        SpriteManager.AddSprite("KnightDeath", "SpriteSheets/Knight/Death" , 1 ,11);
        SpriteManager.AddSprite("KnightIdle", "SpriteSheets/Knight/Idle" , 1 ,11);
        SpriteManager.AddSprite("KnightRun", "SpriteSheets/Knight/Run" , 1 ,8);
        SpriteManager.AddSprite("KnightHit", "SpriteSheets/Knight/Take Hit" , 1 ,4);
        
       // Terrain
       SpriteManager.AddSprite("TerrainSky", "Terrain/sky");
       SpriteManager.AddSprite("TerrainHills", "Terrain/Hills");
       SpriteManager.AddSprite("TerrainBackgroundTrees", "Terrain/tree_cluster_background1");
       SpriteManager.AddSprite("TerrainGround", "Terrain/Ground");
       SpriteManager.AddSprite("TerrainMidgroundTree", "Terrain/tree_cluster_midground1");
       SpriteManager.AddSprite("TerrainForegroundTrees", "Terrain/tree_cluster_foreground1");
       SpriteManager.AddSprite("TerrainForegroundFoliage", "Terrain/foliage_cluster_foreground1");
        
        Init();
    }

    void Init()
    {
        SceneManager.Add(new ParallaxLayer("TerrainSky", 0, 1f, 0f));
        SceneManager.Add(new ParallaxLayer("TerrainHills", 350, 1f, 0.12f));
        SceneManager.Add(new ParallaxLayer("TerrainBackgroundTrees", 420, 1f, 0.25f));
        SceneManager.Add(new ParallaxLayer(
            "TerrainGround",
            y: 900,
            scale: 0.3f,       // Reduce this if the ground tile looks too large.
            parallax: 1f));
        SpawnLevel();
        var floor = new Sprite("Pixel");
        floor._transform._position = new Vector2(0, 940);
        floor._transform._scale = new Vector2(LevelWidth, 20);
        floor._color = Color.Transparent;
        SceneManager.Add(floor);

        var floorCollider = new Collider
        {
            Parent = floor,
            colliderType = Collider.e_ColliderType.Ground,
        };
        SceneManager.Add(floorCollider);
        
        _player = SceneManager.Create<Player>();
        _player.CreateCollider();
        _player._transform._position = new Vector2(180, 500);
       
       
       // SceneManager.Add(new ParallaxLayer("TerrainForegroundTrees", 620, 0.45f, 0.6f, 900f));
        SceneManager.Add(new ParallaxLayer("TerrainForegroundFoliage", 1010, 0.2f, 0.85f , 1080));
        
       // HUD
       const float uiScale = 3.5f;
       const int uiX = 15;
       const int uiY = 15;

       

       SceneManager.Add(new PlayerHealthBar(_player, new Vector2(uiX, uiY), uiScale));
       
       var staminaBackground = new Sprite("BackgroundBar");
       staminaBackground._sourceRectangle = new Rectangle(31, 8, 71, 3);
       staminaBackground._transform._position =
           new Vector2(uiX + 31 * uiScale, uiY + 18 * uiScale);
       staminaBackground._transform._scale = new Vector2(uiScale, uiScale);
       AddHudSprite(staminaBackground);

       AddHudSprite(new PlayerStaminaBar(_player, new Vector2(uiX, uiY), uiScale));
       
       var barUI = new Sprite("BarUI");
       barUI._transform._position = new Vector2(uiX, uiY);
       barUI._transform._scale = new Vector2(uiScale, uiScale);
       AddHudSprite(barUI);
       
       var coinIcon = new Sprite("Money");
       coinIcon._transform._position = new Vector2(
           uiX + 31 * uiScale,
           uiY + 32 * uiScale -10);
       coinIcon._transform._scale = new Vector2(1.2f);
       AddHudSprite(coinIcon);
       
       Text coinCount = new Text();
        coinCount._transform._position = new Vector2(
            coinIcon._transform._position.X * 1.5f, coinIcon._transform._position.Y + 30);
        coinCount._font = dungeonFont;
        coinCount._textScale = new Vector2(2.5f, 2.5f);
        coinCount._text = "0";
        SceneManager.Add(coinCount);
       
        _player.ItemPickedUp += item =>
        {
            if (item.ItemID == "Gold_Coins" || item.ItemType == Item.e_ItemType.Coin)
            {
                int totalCoins = _player.Inventory.GetQuantity("Gold_Coins") * item.ItemPrice;
                coinCount._text = totalCoins.ToString();
            }
        };
        
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
        const float rightMargin = 80f;

        float slotY = ScreenSize.Y - slotSize - bottomMargin;
        float leftSlotX = leftMargin; 
        float rightSlotX = leftSlotX + slotSize + attackSlotGap; 
        float pickupSlotX = rightSlotX + slotSize + attackSlotGap; 
        
        float inventorySlotX = ScreenSize.X - slotSize - rightMargin;
       var leftSlot = new Sprite("ItemSlot");
       leftSlot._transform._position = new Vector2(leftSlotX, slotY);
       leftSlot._transform._scale = new Vector2(itemSlotScale);
       AddHudSprite(leftSlot);
       var rightSlot = new Sprite("ItemSlot");
       rightSlot._transform._position = new Vector2(rightSlotX, slotY);
       rightSlot._transform._scale = new Vector2(itemSlotScale);
       AddHudSprite(rightSlot);
       var pickupSlot = new Sprite("ItemSlot");
       pickupSlot._transform._position = new Vector2(pickupSlotX, slotY);
       pickupSlot._transform._scale = new Vector2(itemSlotScale);
       AddHudSprite(pickupSlot);
       var inventorySlot = new Sprite("ItemSlot");
       inventorySlot._transform._position = new Vector2(inventorySlotX, slotY);
       inventorySlot._transform._scale = new Vector2(itemSlotScale);
       AddHudSprite(inventorySlot);
       SceneManager.Add(new InventorySlotUI(_player, new Vector2(inventorySlotX, slotY), slotSize, dungeonFont));
       var lightIcon = new Sprite("LightAttackUI");
       lightIcon._transform._position= new Vector2(leftSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       lightIcon._transform._scale = new Vector2(iconScale);
       AddHudSprite(lightIcon);
       var heavyIcon = new Sprite("HeavyAttackUI");
       heavyIcon._transform._position = new Vector2(rightSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       heavyIcon._transform._scale = new Vector2(iconScale);
       AddHudSprite(heavyIcon);
       float pickupIconScale = iconSize / 32f;
       var pickupIcon = new Sprite("PickupIcon");
       pickupIcon._transform._position = new Vector2(pickupSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       pickupIcon._transform._scale = new Vector2(pickupIconScale);
       AddHudSprite(pickupIcon);
       Vector2 lightIconPosition = new Vector2(
           leftSlotX + (slotWidth - iconSize) / 2, 
           slotY + (slotHeight - iconSize) / 2);
       Vector2 heavyIconPosition = new Vector2(
           rightSlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);
       float keyScale = 4f;
       Vector2 keyOffset = new Vector2(-30, -30);
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
       var lightKeyPrompt = new KeyboardPrompt(
           Keys.J,
           new Rectangle(0, 144, 16, 16), // J released
           new Rectangle(16, 144, 16, 16), // J held
           lightKeyPosition,
           keyScale);
       AddHudSprite(lightKeyPrompt);
       var heavyPrompt  = new KeyboardPrompt(
           Keys.K,
           new Rectangle(0, 160, 16, 16), // K released
           new Rectangle(16, 160, 16, 16), // K held
           heavyKeyPosition,
           keyScale);
       AddHudSprite(heavyPrompt);
       var pickupPrompt =new KeyboardPrompt(
           Keys.E,
           new Rectangle(0, 64, 16, 16), // E released
           new Rectangle(16, 64, 16, 16), // E held
           pickupSlotPosition + keyOffset,
           keyScale);
       AddHudSprite(pickupPrompt);
       var interactPrompt =new KeyboardPrompt(
           Keys.F,
           new Rectangle(0, 80, 16, 16), // F released
           new Rectangle(16, 80, 16, 16), // F held
           inventorySlotPosition + keyOffset,
           keyScale);
       AddHudSprite(interactPrompt);
      var invLeft =new KeyboardPrompt( 1,
           Keys.OemComma,
           new Rectangle(0, 270, 16, 16), // < released
           new Rectangle(16, 270, 16, 16),  // < pressed
           new Vector2(inventorySlot._transform._position.X + keyOffset.X - 25,inventorySlot._transform._position.Y + keyOffset.Y + 50 )
           ,keyScale); 
      AddHudSprite(invLeft);
       var invRight = new KeyboardPrompt( 1,
           Keys.OemPeriod,
           new Rectangle(0, 254, 16, 16), // < released
           new Rectangle(16, 254, 16, 16),  // < pressed
           new Vector2(inventorySlot._transform._position.X - keyOffset.X + 60,inventorySlot._transform._position.Y + keyOffset.Y + 50 )
           ,keyScale); 
       AddHudSprite(invRight);
    }
    
    protected override void Update(GameTime gameTime)
    {
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (!GameOver)
        {
            SceneManager.Instance.Update(gameTime);
            _levelManager.Update(_player);
            Camera2D.Follow(_player, LevelWidth, ScreenSize.X);
        }

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
    private static void AddHudSprite(Sprite sprite)
    {
        sprite.UseCamera = false;
        SceneManager.Add(sprite);
    }
    private void SpawnLevel()
{
    const float sectionWidth = LevelManager.SectionWidth;

    var section1 = new LevelSection(0, sectionWidth);
    var section2 = new LevelSection(sectionWidth, sectionWidth * 2);
    var section3 = new LevelSection(sectionWidth * 2, sectionWidth * 3);
    var section4 = new LevelSection(sectionWidth * 3, sectionWidth * 4);

    _levelManager.AddSection(section1);
    _levelManager.AddSection(section2);
    _levelManager.AddSection(section3);
    _levelManager.AddSection(section4);

    // ---- SECTION 1 ----
    var chest1 = SceneManager.Create<Chest>();
    PlaceChest(chest1, 1200);
    chest1.CreateCollider();
    section1.Chests.Add(chest1);

  
    var goblin1 = SceneManager.Create<Goblin>();
    goblin1._transform._position = new Vector2(700, 530);
    goblin1._effects = SpriteEffects.None;
    goblin1.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(goblin1));
    section1.Enemies.Add(goblin1);

   
    var goblin2 = SceneManager.Create<Goblin>();
    goblin2._transform._position = new Vector2(1600, 530);
    goblin2._effects = SpriteEffects.FlipHorizontally;
    goblin2.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(goblin2));
    section1.Enemies.Add(goblin2);

    var goblin3 = SceneManager.Create<Goblin>();
    goblin3._transform._position = new Vector2(1900, 530);
    goblin3._effects = SpriteEffects.FlipHorizontally;
    goblin3.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(goblin3));
    section1.Enemies.Add(goblin3);

    // ---- SECTION 2 ----
    var goblin4 = SceneManager.Create<Goblin>();
    goblin4._transform._position = new Vector2(3000, 530);
    goblin4._effects = SpriteEffects.FlipHorizontally;
    goblin4.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(goblin4));
    section2.Enemies.Add(goblin4);

    var knight1 = SceneManager.Create<Knight>();
    knight1._transform._position = new Vector2(3550, 530);
    knight1._effects = SpriteEffects.FlipHorizontally;
    knight1.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(knight1));
    section2.Enemies.Add(knight1);

    var chest2 = SceneManager.Create<Chest>();
    PlaceChest(chest2, 4500);
    chest2.CreateCollider();
    section2.Chests.Add(chest2);

    // ---- SECTION 3 ----
    var chest3 = SceneManager.Create<Chest>();
    PlaceChest(chest3, 5400);
    chest3.CreateCollider();
    section3.Chests.Add(chest3);

    var chest4 = SceneManager.Create<Chest>();
    PlaceChest(chest4, 5800);
    chest4.CreateCollider();
    section3.Chests.Add(chest4);

    var knight2 = SceneManager.Create<Knight>();
    knight2._transform._position = new Vector2(6300, 530);
    knight2._effects = SpriteEffects.FlipHorizontally;
    knight2.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(knight2));
    section3.Enemies.Add(knight2);

    var fantasyWarrior = SceneManager.Create<FantasyWarrior>();
    fantasyWarrior._transform._position = new Vector2(6700, 530);
    fantasyWarrior._effects = SpriteEffects.FlipHorizontally;
    fantasyWarrior.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(fantasyWarrior));
    section3.Enemies.Add(fantasyWarrior);

    // ---- SECTION 4 ----
    var king = SceneManager.Create<King>();
    king._transform._position = new Vector2(8200, 530);
    king._effects = SpriteEffects.FlipHorizontally;
    king.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(king));
    section4.Enemies.Add(king);

    var deathBringer = SceneManager.Create<DeathBringer>();
    deathBringer._transform._position = new Vector2(8800, 530);
    deathBringer._effects = SpriteEffects.FlipHorizontally;
    deathBringer.CreateCollider();
    SceneManager.Add(new EnemyHealthBar(deathBringer));
    section4.Enemies.Add(deathBringer);

    float gateY = 700f;
    float gateScale = 1.5f;

   
}
    private static void PlaceChest(Chest chest, float worldX)
    {
        float chestHeight =
            chest._sourceRectangle.Height * chest._transform._scale.Y;

        chest._transform._position =
            new Vector2(worldX, FloorY - chestHeight);

        chest.CreateCollider();
    }
}
