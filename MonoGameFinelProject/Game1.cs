using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFinelProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static Vector2 ScreenSize = new Vector2(1920, 1080);
    SpriteManager _spriteManager = null;
    AudioManager _audioManager = null;

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
    //Abilities
        SpriteManager.AddSprite("LightAttackUI", "UI/Abilities/4");
        SpriteManager.AddSprite("HeavyAttackUI", "UI/Abilities/5"); 
    //Hud
        SpriteManager.AddSprite("BackgroundBar", "UI/ui_bar5");
        SpriteManager.AddSprite("HealthBar", "UI/ui_bar2");
        SpriteManager.AddSprite("StaminaBar", "UI/ui_bar6");
        SpriteManager.AddSprite("BarUI", "UI/ui_front_border5");
        SpriteManager.AddSprite("ItemSlot", "UI/ui_select_item3");
        
        SpriteManager.AddSprite("KeyboardABC", "UI/kb_dark_alphanumeric");
// Items
        SpriteManager.AddSprite("ItemsSheet", "SpriteSheets/Items/items 32x32");
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

       var barUI = new Sprite("BarUI");
       barUI._transform._position = new Vector2(uiX, uiY);
       barUI._transform._scale = new Vector2(uiScale, uiScale);
       SceneManager.Add(barUI);

       SceneManager.Add(new PlayerHealthBar(player, new Vector2(uiX, uiY), uiScale));
       
       var staminaBackground = new Sprite("BackgroundBar");
       staminaBackground._sourceRectangle = new Rectangle(31, 8, 71, 3);
       staminaBackground._transform._position =
           new Vector2(uiX + 31 * uiScale, uiY + 18 * uiScale);
       staminaBackground._transform._scale = new Vector2(uiScale, uiScale);
       SceneManager.Add(staminaBackground);

       SceneManager.Add(new PlayerStaminaBar(player, new Vector2(uiX, uiY), uiScale));
       
       const float itemSlotScale = 3f;
       const float iconScale = 0.3f;
       const int bottomMargin = 24;
       const int leftMargin = 35;
       const int slotGap = 25;

       float slotWidth = 32 * itemSlotScale;   
       float slotHeight = 32 * itemSlotScale;
       float iconSize = 256 * iconScale;       

       float slotY = ScreenSize.Y - slotHeight - bottomMargin;
       float leftSlotX = leftMargin;
       float rightSlotX = leftSlotX + slotWidth + slotGap;


       var leftSlot = new Sprite("ItemSlot");
       leftSlot._transform._position = new Vector2(leftSlotX, slotY);
       leftSlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(leftSlot);

       var rightSlot = new Sprite("ItemSlot");
       rightSlot._transform._position = new Vector2(rightSlotX, slotY);
       rightSlot._transform._scale = new Vector2(itemSlotScale);
       SceneManager.Add(rightSlot);


       var lightIcon = new Sprite("LightAttackUI");
       lightIcon._transform._position= new Vector2(leftSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       lightIcon._transform._scale = new Vector2(iconScale);
       SceneManager.Add(lightIcon);

       var heavyIcon = new Sprite("HeavyAttackUI");
       heavyIcon._transform._position = new Vector2(rightSlotX + (slotWidth - iconSize) / 2, slotY + (slotHeight - iconSize) / 2);
       heavyIcon._transform._scale = new Vector2(iconScale);
       SceneManager.Add(heavyIcon);
       
       Vector2 lightIconPosition = new Vector2(
           leftSlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);

       Vector2 heavyIconPosition = new Vector2(
           rightSlotX + (slotWidth - iconSize) / 2,
           slotY + (slotHeight - iconSize) / 2);
       
       float keyScale = 4f;
       Vector2 keyOffset = new Vector2(-30, -30);

       SceneManager.Add(new KeyboardPrompt(
           Keys.J,
           new Rectangle(0, 144, 16, 16), // J released
           new Rectangle(16, 144, 16, 16), // J held
           lightIconPosition + keyOffset,
           keyScale));

       SceneManager.Add(new KeyboardPrompt(
           Keys.K,
           new Rectangle(0, 160, 16, 16), // K released
           new Rectangle(16, 160, 16, 16), // K held
           heavyIconPosition + keyOffset,
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
        _spriteBatch.Begin();

        SceneManager.Instance.Draw(_spriteBatch);
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
