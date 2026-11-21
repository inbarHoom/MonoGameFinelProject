using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameFinelProject;

public sealed class LobbyScreen : IGameScreen
{
    public void Enter()
    {
        LevelDefinition lobbyDefinition = LobbyLevelDefinition.Create();

        SceneManager.Clear();

        foreach (ParallaxDefinition layer in lobbyDefinition.ParallaxLayers)
        {
            SceneManager.Add(new ParallaxLayer(
                layer.SpriteName,
                layer.Y,
                layer.Scale,
                layer.Parallax,
                layer.BottomY));
        }

        var floor = new Sprite("Pixel");
        floor._transform._position = new Vector2(
            0,
            lobbyDefinition.GroundY);

        floor._transform._scale = new Vector2(
            lobbyDefinition.WorldWidth,
            20);

        floor._color = Color.Transparent;
        SceneManager.Add(floor);

        var floorCollider = new Collider
        {
            Parent = floor,
            colliderType = Collider.e_ColliderType.Ground
        };
        SceneManager.Add(floorCollider);

        var player = SceneManager.Create<Player>();
        player.CreateCollider();
        player._transform._position = new Vector2(lobbyDefinition.PlayerLocalX, 500);

        var king = new KingNpc(player, new Vector2(lobbyDefinition.GoodKingLocalX, lobbyDefinition.GroundY));

        SceneManager.AddBefore(king, player);
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
    public void Exit() { }
    
}