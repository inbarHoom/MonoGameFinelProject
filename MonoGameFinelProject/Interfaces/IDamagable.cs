using Microsoft.Xna.Framework;

namespace MonoGameFinelProject;

public interface IDamagable
{
    void TakeDamage(int amount);
}
public interface IHealable
{
    void Heal(int amount);
}
public interface IPickupable
{
    bool TryPickup(Player player);
}
public interface IThrowable
{
    void Throw(Sprite thrower,Vector2 startPos, Vector2 targetPos, int damage);
}
public interface ISellable
{
    void Sell(Item item);
}