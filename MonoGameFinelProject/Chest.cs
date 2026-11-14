using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace MonoGameFinelProject;

public class Chest : Animation
{
    private enum e_ChestState
    {
        Closed,
        Opening,
        MimicAppearing,
        WaitingForMimicDeath,
        Opened
    }
    private const double MimicChance = 0.2;
    private e_ChestState _state = e_ChestState.Closed;
    private bool _lootSpawned;
    private Collider interactCollider;
    private readonly KeyboardPrompt ePrompt;
    private bool touchedThisFrame;
    public bool PlayerInRange {get; private set;}
    public Chest() : base("ChestClosed")
    {
        _transform._scale = Vector2.One * 2.5f;
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
        interactCollider = new Collider
        {
            Parent = this,
            IsEnabled = true,
            isTrigger = true,
            UseCustomBounds = true,
            LocalBounds = new Rectangle(145,150,80,65),
            colliderType = Collider.e_ColliderType.Body,
            showDebugOutline = false
        };
        interactCollider.onTrigger += OnInteractTrigger;
        SceneManager.Add(interactCollider);
    }

    private void OnInteractTrigger(Collider self, Collider other)
    {
        if (other.colliderType == Collider.e_ColliderType.Body && other.Parent is Player)
        {
            touchedThisFrame = true;
            PlayerInRange = true;
        }
    }

    public void Open()
    {
        if(_state !=  e_ChestState.Closed) return;
        if (Random.Shared.NextDouble() < MimicChance)
        {
            SpawnMimic();
            return;
        }
        _state = e_ChestState.Opening;
        StartAnimation("ChestOpening", false,  6,  0);
    }

    private void SpawnMimic()
    {
        _state = e_ChestState.MimicAppearing;

        if (interactCollider != null)
            interactCollider.IsEnabled = false;
        _color = Color.Transparent;
        
        Mimic mimic = SceneManager.Create<Mimic>();
        mimic._transform._position = _transform._position;

        mimic.DeathAnimationFinished += OnMimicDeathAnimationFinished;
        mimic.BeginTransform();
        mimic.CreateCollider();

        _state = e_ChestState.WaitingForMimicDeath;
    }
    private void OnMimicDeathAnimationFinished(Mimic mimic)
    {
        _transform._position = mimic._transform._position;
        SpawnLoot();
        mimic.DeathAnimationFinished -= OnMimicDeathAnimationFinished;
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PlayerInRange = false;
        if (interactCollider != null && interactCollider.IsEnabled)
        {
            for (int i = 0; i < SceneManager.colliders.Count; i++)
            {
                Collider other = SceneManager.colliders[i];
                if (other.IsEnabled && other.colliderType == Collider.e_ColliderType.Body &&
                    other.Parent is Player && interactCollider._rect.Intersects(other._rect))
                {
                    PlayerInRange = true;
                    break;
                }
            }
        }
        bool showPrompt = _state == e_ChestState.Closed && PlayerInRange;
        ePrompt.IsVisible = showPrompt;
        if (showPrompt && interactCollider != null)
        {
            ePrompt._transform._position = new Vector2(
                interactCollider._rect.Left - 20,
                interactCollider._rect.Top - 20);
            ePrompt.Update(gameTime);
        }
        if (_state == e_ChestState.Opening && isFinished)
        {
            ChangeAnimation("ChestOpen");
            SpawnLoot();
            _state = e_ChestState.Opened;
        }
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        ePrompt.Draw(spriteBatch);
    }
    private void SpawnLoot()
    {
        if (_lootSpawned)
            return;

        _lootSpawned = true;

        string[] itemIds = ChestLootTable.RollTwoItems();

        for (int i = 0; i < itemIds.Length; i++)
        {
            Item item = ItemFactory.Create(itemIds[i]);
            item._transform._position = _transform._position + new Vector2(145 + i * 50, 175);
            SceneManager.Add(item);
            item.CreateCollider();
        }
    }
}