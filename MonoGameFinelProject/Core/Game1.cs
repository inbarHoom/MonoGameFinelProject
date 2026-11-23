using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace MonoGameFinelProject;

public class Game1 : Game
{
    private enum GameScreen
    {
        MainMenu,
        Playing,
        Paused,
        Settings,
        Lobby
    }
    
    private readonly ScreenManager _screenManager = new();
    private GameScreen _currentScreen = GameScreen.MainMenu;
    private GameScreen _settingsReturnScreen;
    private KeyboardState _previousKeyboardState;
    
    private LevelDefinition _activeLevelDefinition;
    private float LevelWidth => _activeLevelDefinition.WorldWidth;
    
    private Player _player;
    private  LevelManager _levelManager = new();
    private readonly List<MerchantSiteInfo> _merchantSites = new();
    private const float FloorY = 1100;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static Vector2 ScreenSize = new Vector2(1920, 1080);
    SpriteManager _spriteManager = null;
    AudioManager _audioManager = null;
    private SpriteFont dungeonFont;
    private Sprite _mouseCursor;
    private MenuButton _resumeButton;
    private MenuButton _menuButton;
    private Sprite optionsIcon;
    private Sprite escPrompt;
    private MenuButton _quitButton;
    private MenuButton _settingsButton;
    private MenuButton _mainMenuButton;
    private bool _waitForMouseReleaseBeforeSliders;
    private VolumeSlider _musicSlider;
    private VolumeSlider _sfxSlider;
   

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
        IsMouseVisible = false;
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
// Audio
    // Music
        AudioManager.AddSong( "ByTheRiver" ,"Audio/Music/Clement Panchout _ 2022 _ By the river");
        AudioManager.AddSong( "Journey" ,"Audio/Music/Clement Panchout _ Journey _ 2017");
    // SFX 
        // General
        AudioManager.AddSoundEffect("CoinUp", "Audio/SFX/dingDingDing");
        AudioManager.AddSoundEffect("ChestOpen", "Audio/SFX/doorOpen");
        AudioManager.AddSoundEffect("PickupItem", "Audio/SFX/pickUp");
        // Player
        AudioManager.AddSoundEffect("BombParry", "Audio/SFX/PlayerSFX/bombParry");
        AudioManager.AddSoundEffect("HitArmor", "Audio/SFX/PlayerSFX/hitArmor");
        AudioManager.AddSoundEffect("PlayerDeath", "Audio/SFX/PlayerSFX/playerDeath");
        AudioManager.AddSoundEffect("LightAttack", "Audio/SFX/PlayerSFX/slash5");
        AudioManager.AddSoundEffect("HeavyAttack", "Audio/SFX/PlayerSFX/slash6");
        // Enemies
        AudioManager.AddSoundEffect("GoblinAttack1", "Audio/SFX/EnemiesSFX/slash1");
        AudioManager.AddSoundEffect("GoblinAttack2", "Audio/SFX/EnemiesSFX/slash2");
        
// UI
        SpriteManager.AddSprite("MainMenuBackground", "UI/BackGroundMainMenu");
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
        SpriteManager.AddSprite("BorderSheet" , "UI/09 Border 03" ,4,5);
        SpriteManager.AddSprite("Cursor", "UI/HandCorsur" ,1,2);
        SpriteManager.AddSprite("Options", "UI/ExtraIcon02");
// Items
        SpriteManager.AddSprite("Potions", "SpriteSheets/Items/items 32x32" , 4 ,4);
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
// Allies
        SpriteManager.AddSprite("Merchant", "SpriteSheets/Merchant/Idle", 1, 8);
        SpriteManager.AddSprite("GoodKing","SpriteSheets/GoodKing/Idle", 1, 6);
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
       
       SpriteManager.AddSprite("Campfire", "SpriteSheets/Decoration/Campfire sheet", 8, 5);
       SpriteManager.AddSprite("CampfireWithFood", "SpriteSheets/Decoration/Campfire with food sheet", 8, 5);
       SpriteManager.AddSprite("SmallTent", "SpriteSheets/Decoration/Small Tent");
       SpriteManager.AddSprite("DecorAtlas", "SpriteSheets/Decoration/Decor");
        
        Init();
    }

    void Init()
    {
        AudioManager.PlaySong("Journey" , true , 0.1f);
        _activeLevelDefinition = FirstLevelDefinition.Create();
        AddParallaxLayers(ParallaxPlacement.Background);
        SpawnLevel();
        var floor = new Sprite("Pixel");
        floor._transform._position = new Vector2(0, _activeLevelDefinition.GroundY);
        floor._transform._scale = new Vector2(_activeLevelDefinition.WorldWidth, 20);
        floor._color = Color.Transparent;
        SceneManager.Add(floor);

        _musicSlider = new VolumeSlider(
            new Rectangle(800, 430, 320, 12),
            "Music",
            AudioManager.SongVolume,
            dungeonFont,
            value => AudioManager.SongVolume = value);

        _sfxSlider = new VolumeSlider(
            new Rectangle(800, 530, 320, 12),
            "Sound Effects",
            AudioManager.SoundEffectVolume,
            dungeonFont,
            value => AudioManager.SoundEffectVolume = value);
        
        var floorCollider = new Collider
        {
            Parent = floor,
            colliderType = Collider.e_ColliderType.Ground,
        };
        SceneManager.Add(floorCollider);
        
        _player = SceneManager.Create<Player>();
        _player.CreateCollider();
        _player._transform._position = new Vector2(180, 500);
       
        AddParallaxLayers(ParallaxPlacement.Foreground);
        
        var merchantShop = new MerchantShopSystem(
            _player,
            dungeonFont,
            _merchantSites);
        _player.MerchantShop = merchantShop;
        SceneManager.Add(merchantShop);
        
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
       
        _player.WalletChanged += balance =>
        {
            coinCount._text = balance.ToString();
        };
        _player.NotifyWalletChanged();

        _player.ItemPickedUp += item =>
        {
            if (item.ItemType == Item.e_ItemType.Coin)
                AudioManager.PlaySoundEffect("CoinUp", false, 0.5f);
        };
        
       const float itemSlotScale = 3f;
       const float iconScale = 0.3f;
       const int bottomMargin = 24;
       const int leftMargin = 60;

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
       
       int pauseButtonX = (int)ScreenSize.X - 100;
       int pauseButtonY = 20;
       
       Spritesheet borderSheet = SpriteManager.GetSprite("BorderSheet");

       Rectangle menuBorder = borderSheet[1, 3]; // menu border
       
       Spritesheet optionsSheet = SpriteManager.GetSprite("Options");

       _menuButton = new MenuButton(
           new Rectangle(pauseButtonX, pauseButtonY, 80, 80),
           "",
           dungeonFont,
           () => _currentScreen = GameScreen.Paused,
           optionsSheet.texture,
           optionsSheet[0, 0]);
       
       
        escPrompt = new KeyboardPrompt(
           1, // selects the KeyboardSymbols sheet
           Keys.Escape,
           new Rectangle(0, 0, 16, 16),   // ESC released
           new Rectangle(16, 0, 16, 16),  // ESC pressed
           new Vector2(pauseButtonX - 20f, pauseButtonY + 52f),
           3f,
           true)
       {
           FlickerInterval = 1f
       };
       AddHudSprite(escPrompt);
       
       _resumeButton = new MenuButton(
           new Rectangle(820, 420, 280, 80),
           "Resume",
           dungeonFont,
           () => _currentScreen = GameScreen.Playing,
           2f);
       
       _settingsButton = new MenuButton(
           new Rectangle(820, 520, 280, 80),
           "Settings",
           dungeonFont,
           () =>  {
               _settingsReturnScreen = GameScreen.Paused;
               _currentScreen = GameScreen.Settings;
               _waitForMouseReleaseBeforeSliders = true;
           },
           2f);
       
       _mainMenuButton = new MenuButton(
           new Rectangle(820, 620, 280, 80),
           "Main Menu",
           dungeonFont,
           () =>     _currentScreen = GameScreen.MainMenu,
           2f);

       _quitButton = new MenuButton(
           new Rectangle(820, 720, 280, 80),
           "Quit",
           dungeonFont,
           () => Exit(),
           2f);
       
       _mouseCursor = new Sprite("Cursor")
       {
           UseCamera = false,
           _sourceRectangle = SpriteManager.GetSprite("Cursor")[0, 0]
       };
       _mouseCursor._transform._scale = new Vector2(2f);
       
       _screenManager.ChangeScreen(new MainMenuScreen(dungeonFont, 
           onPlay: () =>
           {
               _currentScreen = GameScreen.Playing;
           },
           onSettings: () =>
           {
               _settingsReturnScreen = GameScreen.MainMenu;
               _currentScreen = GameScreen.Settings;
               _waitForMouseReleaseBeforeSliders = true;
           },
           onQuit: () => Exit()));
    }
    
    protected override void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        GameScreen screenAtStart = _currentScreen;
        
        _mouseCursor._transform._position = new Vector2(mouse.X, mouse.Y);
        _mouseCursor._sourceRectangle = SpriteManager.GetSprite("Cursor")[mouse.LeftButton == ButtonState.Pressed ? 1 : 0, 0];
        
        KeyboardState keyboard = Keyboard.GetState();

        bool escapePressed =
            keyboard.IsKeyDown(Keys.Escape) &&
            !_previousKeyboardState.IsKeyDown(Keys.Escape);
     
        if (escapePressed)
        {
            if (_currentScreen == GameScreen.Playing)
                _currentScreen = GameScreen.Paused;
            else if (_currentScreen == GameScreen.Paused)
                _currentScreen = GameScreen.Playing;
            else if (_currentScreen == GameScreen.Settings)
            {
                _currentScreen = _settingsReturnScreen;
            }
        }
        if (_currentScreen == GameScreen.MainMenu ||
            _currentScreen == GameScreen.Lobby)
        {
            _screenManager.Update(gameTime);
        }
        if(_currentScreen == GameScreen.Settings && screenAtStart == GameScreen.Settings)
        {
            if (_waitForMouseReleaseBeforeSliders)
            {
                if (mouse.LeftButton == ButtonState.Released)
                    _waitForMouseReleaseBeforeSliders = false;
            }
            else
            {
                _musicSlider.Update();
                _sfxSlider.Update();
            }
        }
        
        if (_currentScreen == GameScreen.Paused)
        {
            _resumeButton.Update();
            _settingsButton.Update();
            _mainMenuButton.Update();
            _quitButton.Update();
        }
        if (_currentScreen == GameScreen.Playing)
            _menuButton.Update();
        
        if (_currentScreen == GameScreen.Playing && !GameOver)
        {
            SceneManager.Instance.Update(gameTime);
            _levelManager.Update(_player);
            Camera2D.Follow(_player, LevelWidth, ScreenSize.X);
        }
        
        _previousKeyboardState = keyboard;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

       
        bool showingGameplay =
            _currentScreen == GameScreen.Playing ||
            _currentScreen == GameScreen.Paused || 
            _currentScreen == GameScreen.Lobby ||
            (_currentScreen == GameScreen.Settings &&
             _settingsReturnScreen == GameScreen.Paused);

        if (showingGameplay)
            SceneManager.Instance.Draw(_spriteBatch);
        else
            _screenManager.Draw(_spriteBatch);

        if (_currentScreen == GameScreen.Playing)
        {
            _menuButton.Draw(_spriteBatch);
            escPrompt.Draw(_spriteBatch);
        }
        
        if (_currentScreen == GameScreen.Paused ||
            _currentScreen == GameScreen.Settings)
        {
            Texture2D pixel = SpriteManager.GetSprite("Pixel").texture;
            _spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y),
                Color.Black * 0.65f);

            Spritesheet borders = SpriteManager.GetSprite("BorderSheet");
            Rectangle panelBounds = new Rectangle(620, 180, 680, 720);

            _spriteBatch.Draw(
                borders.texture,
                panelBounds,
                borders[1, 3],
                Color.White);
        }
        
        if (_currentScreen == GameScreen.Paused)
        {
            _resumeButton.Draw(_spriteBatch);
            _settingsButton.Draw(_spriteBatch);
            _mainMenuButton.Draw(_spriteBatch);
            _quitButton.Draw(_spriteBatch);
        }

        if (_currentScreen == GameScreen.Settings)
        {
            _spriteBatch.DrawString(dungeonFont, "Settings", new Vector2(910, 270), Color.White, 
                0f, Vector2.Zero, new Vector2(2f), SpriteEffects.None, 0f);
            _musicSlider.Draw(_spriteBatch);
            _sfxSlider.Draw(_spriteBatch);
        }

        _mouseCursor.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    
    private void AddParallaxLayers(ParallaxPlacement placement)
    {
        foreach (ParallaxDefinition layer in _activeLevelDefinition.ParallaxLayers)
        {
            if (layer.Placement != placement)
                continue;

            SceneManager.Add(new ParallaxLayer(
                layer.SpriteName,
                layer.Y,
                layer.Scale,
                layer.Parallax,
                layer.BottomY));
        }
    }
    
    private static void AddHudSprite(Sprite sprite)
    {
        sprite.UseCamera = false;
        SceneManager.Add(sprite);
    }
    
    private static T SpawnEnemy<T>(
        LevelSection section,
        float worldX,
        SpriteEffects effects = SpriteEffects.FlipHorizontally)
        where T : Enemy, new()
    {
        var enemy = SceneManager.Create<T>();
        float enemyWidth =
            enemy._sourceRectangle.Width * enemy._transform._scale.X;
        float safeWorldX = MathHelper.Clamp(
            worldX,
            section.Left,
            section.Right - enemyWidth);
        enemy._transform._position = new Vector2(safeWorldX, 530);
        enemy._effects = effects;
        enemy.CreateCollider();

        SceneManager.Add(new EnemyHealthBar(enemy));
        section.Enemies.Add(enemy);

        return enemy;
    }
    
    private void SpawnLevel() 
    { 
        var builder = new LevelBuilder();

        _levelManager = builder.Build(_activeLevelDefinition, out List<MerchantSiteInfo> sites);
        _merchantSites.AddRange(sites);
        
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
