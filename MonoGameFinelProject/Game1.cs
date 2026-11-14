using System.Collections.Generic;
using System.IO;
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
    private AudioManager _audioManager = null;

    private Dictionary<Vector2, int> testMap;
    
    private Dictionary<Vector2, int> level_1;



    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.PreferredBackBufferWidth = 1920;

        ScreenSize.X = _graphics.PreferredBackBufferWidth;
        ScreenSize.Y = _graphics.PreferredBackBufferHeight;
    }

    public static bool GameOver { get; set; }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    private Dictionary<Vector2, int> LoadMap(string filePath)
    {
        Dictionary<Vector2, int> result = new();
        
        StreamReader streamReader = new(filePath);

        int y = 0;
        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
            string[] values = line.Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (int.TryParse(values[x], out int value))
                {
                    if (value > -1)
                    {
                        result[new Vector2(x, y)] = value;
                    }
                }
            }

            y++;
        }
        
        return result;
    }

    protected override void LoadContent()
    {
        _spriteManager = new SpriteManager(Content);
        _audioManager = new AudioManager(Content); 
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
//Texture Atlas
        SpriteManager.AddSprite("Map","SpriteSheets/Enviroment/Enviroment",43,57);
        
//Map
        level_1= LoadMap("Content/MapSheets/level 8.csv");

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
        SpriteManager.AddSprite("Bomb", "SpriteSheets/Goblin/Bomb_sprite", 1, 8);
// Enemies:
// Goblin
        SpriteManager.AddSprite("GoblinIdle", "SpriteSheets/Goblin/Idle", 1, 4);
        SpriteManager.AddSprite("GoblinRun", "SpriteSheets/Goblin/Run", 1, 8);
        SpriteManager.AddSprite("GoblinAttack1", "SpriteSheets/Goblin/Attack", 1, 8);
        SpriteManager.AddSprite("GoblinAttack2", "SpriteSheets/Goblin/Attack2", 1, 8);
        SpriteManager.AddSprite("GoblinAttack3", "SpriteSheets/Goblin/Attack", 1, 12);
        SpriteManager.AddSprite("GoblinHit", "SpriteSheets/Goblin/Take Hit", 1, 4);
        SpriteManager.AddSprite("GoblinDeath", "SpriteSheets/Goblin/Death", 1, 4);
        
        Init();
    }

    void Init()
    {
       var player = SceneManager.Create<Player>();
       var goblin = SceneManager.Create<Goblin>();
       
       player.CreateCollider();
       goblin.CreateCollider();
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

        int display_tilesize = 64;
        int num_tiles_per_row = 57;
        int pixel_tilesize = 16;
        
            foreach (var tile in level_1)
            {
                Rectangle dest = new(
                    (int)(tile.Key.X * display_tilesize),
                    (int)(tile.Key.Y * display_tilesize),
                    display_tilesize,
                    display_tilesize);

                int x = tile.Value % num_tiles_per_row;
                int y = tile.Value / num_tiles_per_row;

                Rectangle src = new(
                    x * pixel_tilesize,
                    y * pixel_tilesize,
                    pixel_tilesize,
                    pixel_tilesize);

                _spriteBatch.Draw(SpriteManager.GetSprite("Map").texture, dest, src, Color.White);
            }
        

        SceneManager.Instance.Draw(_spriteBatch);
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    

}
