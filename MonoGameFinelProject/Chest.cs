using Microsoft.Xna.Framework;
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
    public bool PlayerInRange {get; private set;}
    public Chest() : base("ChestClosed")
    {
        _transform._scale = Vector2.One * 2.5f;
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
            showDebugOutline = true
        };
        interactCollider.onTrigger += OnInteractTrigger;
        SceneManager.Add(interactCollider);
    }

    private void OnInteractTrigger(Collider self, Collider other)
    {
        if (other.colliderType == Collider.e_ColliderType.Body && other.Parent is Player)
        {
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
        PlayerInRange = false;
        base.Update(gameTime);

        if (_state == e_ChestState.Opening && isFinished)
        {
            ChangeAnimation("ChestOpen");
            SpawnLoot();
            _state = e_ChestState.Opened;
        }
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
            item._transform._position = _transform._position + new Vector2(i * 40, 0);
            item.CreateCollider();
            SceneManager.Add(item);
        }
    }
}