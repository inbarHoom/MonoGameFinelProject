namespace MonoGameFinelProject;

public interface IDamagable
{
    void TakeDamage(int amount);
}
public interface IHealable
{
    void Heal(int amount);
}